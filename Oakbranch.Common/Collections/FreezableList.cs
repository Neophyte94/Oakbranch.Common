using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Oakbranch.Common.Collections
{
    /// <summary>
    /// Represents a list that can be made immutable at some point.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class FreezableList<T> : IList<T>
    {
        #region Instance props & fields

        private T[] _items;

        private int _count;
        public int Count => _count;

        private bool _isReadOnly;
        public bool IsReadOnly => _isReadOnly;

        private int _version = int.MinValue;

        #endregion

        #region Instance indexers

        public T this[int index]
        {
            get
            {
                return _items[index];
            }
            set
            {
                ThrowIfFrozen();
                Insert(index, value);
            }
        }

        #endregion

        #region Instance constructors

        public FreezableList()
        {
            _items = new T[4];
        }

        public FreezableList(int capacity)
        {
            _items = new T[Math.Max(capacity, 1)];
        }

        public FreezableList(T[] items)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            _count = items.Length;
            _items = new T[Math.Max(_count, 1)];

            for (int i = 0; i != _count; ++i)
            {
                _items[i] = items[i];
            }
        }

        public FreezableList(ICollection<T> items)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            _items = new T[Math.Max(items.Count, 1)];

            foreach (T item in items)
            {
                _items[_count++] = item;
            }
        }

        public FreezableList(IEnumerable<T> items)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            _items = items.ToArray();
            _count = _items.Length;

            if (_count == 0)
            {
                _items = new T[1];
            }
        }

        #endregion

        #region Instance methods

        /// <summary>
        /// Adds the given item to the end of the list.
        /// <para>Throws <see cref="InvalidOperationException"/> if the list has been frozen.</para>
        /// </summary>
        /// <param name="item">The item to add.</param>
        /// <exception cref="InvalidOperationException"/>
        public void Add(T item)
        {
            ThrowIfFrozen();
            EnsureCapacity(_count + 1);
            _items[_count++] = item;
            ++_version;
        }

        /// <summary>
        /// Clears the content of the list.
        /// <para>Throws <see cref="InvalidOperationException"/> if the list has been frozen.</para>
        /// </summary>
        /// <exception cref="InvalidOperationException"/>
        public void Clear()
        {
            ThrowIfFrozen();
            Array.Clear(_items, 0, _count);
            _count = 0;
            ++_version;
        }

        public bool Contains(T item)
        {
            if (item == null) 
            {
                for (int i = 0; i < _count;)
                {
                    if (_items[i++] == null)
                    {
                        return true;
                    }
                }

                return false;
            }
            else
            {
                for (int i = 0; i < _count;)
                {
                    if (item.Equals(_items[i++]))
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            if (array == null)
            {
                throw new ArgumentNullException(nameof(array));
            }

            int count = _count;
            if (array.Length - arrayIndex < count)
            {
                throw new ArgumentException(
                    $"The specified array is not large enough ({array.Length}) " +
                    $"to contain {count} items from the index {arrayIndex}.");
            }


            for (int i = 0, j = arrayIndex; i != count;)
            {
                array[j++] = _items[i++];
            }
        }

        public int IndexOf(T item)
        {
            if (item == null)
            {
                for (int i = 0; i < _count; ++i)
                {
                    if (_items[i] == null)
                    {
                        return i;
                    }
                }

                return -1;
            }
            else
            {
                for (int i = 0; i < _count; ++i)
                {
                    if (item.Equals(_items[i]))
                    {
                        return i;
                    }
                }

                return -1;
            }
        }

        /// <summary>
        /// Inserts the given item at the specified index.
        /// <para>Throws <see cref="InvalidOperationException"/> if the list has been frozen.</para>
        /// </summary>
        /// <param name="index">The index to insert the item at.</param>
        /// <param name="item">The item to insert.</param>
        /// <exception cref="IndexOutOfRangeException"></exception>
        /// <exception cref="InvalidOperationException"/>
        public void Insert(int index, T item)
        {
            if (index == _count)
            {
                Add(item);
                return;
            }
            else if (index > _count || index < 0)
            {
                throw new IndexOutOfRangeException();
            }

            ThrowIfFrozen();
            if (index < 0 || index > _count)
            {
                throw new IndexOutOfRangeException();
            }

            _items[index] = item;            
            ++_version;
        }

        /// <summary>
        /// Removes the first occurrence of the given item from the list.
        /// <para>Throws <see cref="InvalidOperationException"/> if the list has been frozen.</para>
        /// </summary>
        /// <param name="item">The item to remove.</param>
        /// <returns><see langword="true"/> if the item has been removed, <see langword="false"/> if it has not been found.</returns>
        /// <exception cref="InvalidOperationException"/>
        public bool Remove(T item)
        {
            ThrowIfFrozen();

            int idx = IndexOf(item);
            if (idx == -1) return false;

            RemoveAt(idx);
            return true;
        }

        /// <summary>
        /// Removes the item at the specified index from the list.
        /// <para>Throws <see cref="InvalidOperationException"/> if the list has been frozen.</para>
        /// </summary>
        /// <param name="index">The zero-based index to remove an item at.</param>
        /// <exception cref="IndexOutOfRangeException"/>
        /// <exception cref="InvalidOperationException"/>
        public void RemoveAt(int index)
        {
            ThrowIfFrozen();
            if (index < 0 || index >= _count)
            {
                throw new IndexOutOfRangeException();
            }

            for (int i = index + 1; i < _count; ++i)
            {
                _items[index - 1] = _items[index];
            }

            _items[index] = default;
            --_count;
            ++_version;
        }

        private void EnsureCapacity(int neededCapacity)
        {
            if (_items.Length < neededCapacity)
            {
                T[] buffer = new T[_items.Length << 1];
                _items.CopyTo(buffer, 0);
                _items = buffer;
            }
        }

        /// <summary>
        /// Makes the list immutable.
        /// <para>The value of <see cref="IsReadOnly"/> constantly becomes <see langword="true"/> after the call.</para>
        /// </summary>
        public void Freeze()
        {
            _isReadOnly = true;
        }

        private void ThrowIfFrozen()
        {
            if (_isReadOnly)
            {
                throw new InvalidOperationException("This list cannot be modified any more since it has been frozen.");
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            int version = _version;
            for (int i = 0; i != _count;)
            {
                if (version != _version)
                {
                    throw new Exception("The collection was modified during enumeration.");
                }

                yield return _items[i++];
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => (this as IEnumerable<T>).GetEnumerator();

        #endregion
    }
}
