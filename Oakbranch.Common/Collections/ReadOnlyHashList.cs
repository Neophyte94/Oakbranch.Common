using System;
using System.Collections;
using System.Collections.Generic;

namespace Oakbranch.Common.Collections
{
    /// <summary>
    /// Represents a read-only collection of ordered unique elements.
    /// <para>Implements <see cref="IReadOnlyList{T}"/>.</para>
    /// </summary>
    /// <typeparam name="T">The type of the elements.</typeparam>
    public class ReadOnlyHashList<T> : IReadOnlyList<T>
    {
        #region Constants

        private const int HashSetThreshold = 10;

        #endregion

        #region Instance props & fields

        private readonly T[] _items;

        private readonly int _count;
        public int Count => _count;

        #endregion

        #region Instance indexers

        public T this[int index]
        {
            get
            {
                if (index < 0 || index >= _count)
                {
                    throw new IndexOutOfRangeException();
                }

                return _items[index];
            }
        }

        #endregion

        #region Instance constructors

        public ReadOnlyHashList()
        {
            _items = null;
            _count = 0;
        }

        public ReadOnlyHashList(T item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            _items = new T[1] { item };
            _count = 1;
        }

        public ReadOnlyHashList(T[] items)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }
            
            int count = items.Length;
            _items = new T[count];
            T item;
            HashSet<T> hashSet = null;

            for (int i = 0; i != count;)
            {
                item = items[i++];
                if (item == null)
                {
                    throw new ArgumentException("Null elements are not allowed.");
                }

                if (!Contains(ref hashSet, item))
                {
                    _items[_count++] = item;
                }
            }
        }

        public ReadOnlyHashList(IEnumerable<T> items)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            _items = new T[4];
            HashSet<T> hashSet = null;

            foreach (T item in items)
            {
                if (item == null)
                {
                    throw new ArgumentException("Null elements are not allowed.");
                }

                if (!Contains(ref hashSet, item))
                {
                    EnsureCapacity(ref _items, _count + 1);
                    _items[_count++] = item;
                }
            }
        }

        public ReadOnlyHashList(IReadOnlyList<T> items)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            int count = items.Count;
            _items = new T[count];
            HashSet<T> hashSet = null;
            T item;

            for (int i = 0; i != count;)
            {
                item = items[i++];
                if (item == null)
                {
                    throw new ArgumentException("Null elements are not allowed.");
                }
                if (!Contains(ref hashSet, item))
                {
                    _items[_count++] = item;
                }
            }
        }

        #endregion

        #region Instance methods

        private void EnsureCapacity(ref T[] buffer, int capacity)
        {
            if (buffer.Length < capacity)
            {
                int targetLength = buffer.Length * 2;
                while (targetLength < capacity)
                {
                    targetLength *= 2;
                }

                T[] temp = new T[targetLength];
                if (_count != 0)
                {
                    Array.Copy(buffer, temp, _count);
                }

                buffer = temp;
            }
        }

        private bool Contains(ref HashSet<T> hashSet, T item)
        {
            if (hashSet == null && HashSetThreshold < _count)
            {
                hashSet = new HashSet<T>(_items);
            }
            if (hashSet == null)
            {
                for (int i = 0; i != _count; ++i)
                {
                    if (Equals(_items[i], item))
                    {
                        return true;
                    }
                }

                return false;
            }
            else
            {
                return hashSet.Contains(item);
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            if (_count == 0) yield break;

            for (int i = 0; i != _count; ++i)
            {
                yield return _items[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => (this as IEnumerable<T>).GetEnumerator();

        #endregion
    }
}
