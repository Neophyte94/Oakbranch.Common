using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Oakbranch.Common.Collections
{
    /// <summary>
    /// Represents a light-weight immutable collection of elements that can be accessed by index.
    /// <para>Implements <see cref="ImmutableList{T}"/>.</para>
    /// </summary>
    /// <typeparam name="T">The type of the elements.</typeparam>
    public class ImmutableList<T> : IReadOnlyList<T>
    {
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

        public ImmutableList()
        {
            _items = null;
            _count = 0;
        }

        public ImmutableList(T item, int count)
        {
            _items = new T[count];
            for (int i = 0; i != count;)
            {
                _items[i++] = item;
            }
            _count = count;
        }

        public ImmutableList(T[] items)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            _count = items.Length;

            if (_count > 0)
            {
                _items = new T[_count];
                for (int i = 0; i != _count; ++i)
                {
                    _items[i] = items[i];
                }
            }
        }

        public ImmutableList(IEnumerable<T> items)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            _items = items.ToArray();
            _count = _items.Length;
            if (_count == 0)
            {
                _items = null;
            }
        }

        public ImmutableList(IReadOnlyCollection<T> items)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            _count = items.Count;

            if (_count > 0)
            {
                _items = new T[_count];

                int idx = 0;
                foreach (T item in items)
                {
                    _items[idx++] = item;
                }
            }
        }

        public ImmutableList(IList<T> items)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            _count = items.Count;

            if (_count > 0)
            {
                _items = new T[_count];
                for (int i = 0; i != _count; ++i)
                {
                    _items[i] = items[i];
                }
            }
        }

        #endregion

        #region Instance methods

        public bool Contains(T item)
        {
            if (item == null)
            {
                return false;
            }

            for (int i = 0; i != _count; ++i)
            {
                if (item.Equals(_items[i]))
                {
                    return true;
                }
            }

            return false;
        }

        public T[] CreateCopy(int startIndex, int length)
        {
            if (startIndex < 0 || startIndex >= _items.Length)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(startIndex),
                    $"The specified start index ({startIndex}) is out of the acceptable range.");
            }
            if (length < 0 || startIndex + length > _items.Length)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(length),
                    $"The specified length ({length}) is out of the valid range.");
            }

            T[] result = new T[length];
            for (int i = 0, j = startIndex; i != length; ++i, ++j)
            {
                result[i] = _items[j];
            }

            return result;
        }

        public T[] ToArray()
        {
            if (_count == 0)
            {
#if NET_7_0
                return Array.Empty<T>();
#else
                return new T[0];
#endif
            }

            T[] result = new T[_count];
            _items.CopyTo(result, 0);

            return result;
        }

        public ReadOnlySpan<T> ToSpan()
        {
            if (_count == 0)
            {
                return new ReadOnlySpan<T>();
            }

            return new ReadOnlySpan<T>(_items);
        }

        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i != _count; ++i)
            {
                yield return _items[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => (this as IEnumerable<T>).GetEnumerator();

#endregion
    }
}
