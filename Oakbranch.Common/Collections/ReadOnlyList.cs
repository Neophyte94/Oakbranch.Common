using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Oakbranch.Common.Collections
{
    public class ReadOnlyList<T> : IReadOnlyList<T>
    {
        #region Instance members

        private readonly T[] m_Items;

        private readonly int m_Count;
        public int Count => m_Count;

        #endregion

        #region Instance indexers

        public T this[int index]
        {
            get
            {
                if (index < 0 || index >= m_Count)
                    throw new IndexOutOfRangeException();
                return m_Items[index];
            }
        }

        #endregion

        #region Instance constructors

        public ReadOnlyList()
        {
            m_Items = null;
            m_Count = 0;
        }

        public ReadOnlyList(T item, int count)
        {
            m_Items = new T[count];
            for (int i = 0; i != count;)
            {
                m_Items[i++] = item;
            }
            m_Count = count;
        }

        public ReadOnlyList(T[] items)
        {
            if (items == null) 
                throw new ArgumentNullException(nameof(items));
            m_Count = items.Length;
            if (m_Count > 0)
            {
                m_Items = new T[m_Count];
                for (int i = 0; i != m_Count; ++i) m_Items[i] = items[i];
            }
        }

        public ReadOnlyList(IEnumerable<T> items)
        {
            if (items == null) 
                throw new ArgumentNullException(nameof(items));
            m_Items = items.ToArray();
            m_Count = m_Items.Length;
            if (m_Count == 0) m_Items = null;
        }

        public ReadOnlyList(IEnumerable<T> items, int count)
        {
            if (items == null) 
                throw new ArgumentNullException(nameof(items));

            m_Count = count;
            m_Items = count != 0 ? new T[count] : null;
            if (count != 0)
            {
                int idx = 0;
                foreach (T item in items)
                {
                    m_Items[idx++] = item;
                }
                if (count != idx)
                {
                    throw new ArgumentException($"The specifed count value ({count}) is different " +
                        $"from the real number of items in the specified collection ({idx}).");
                }
            }
        }

        public ReadOnlyList(IReadOnlyCollection<T> items)
        {
            if (items == null) 
                throw new ArgumentNullException(nameof(items));
            m_Count = items.Count;
            if (m_Count > 0)
            {
                m_Items = new T[m_Count];
                int idx = 0;
                foreach (T item in items) m_Items[idx++] = item;
            }
        }

        public ReadOnlyList(IList<T> items)
        {
            if (items == null) throw new ArgumentNullException(nameof(items));
            m_Count = items.Count;
            if (m_Count > 0)
            {
                m_Items = new T[m_Count];
                for (int i = 0; i != m_Count; ++i) m_Items[i] = items[i];
            }
        }

        #endregion

        #region Instance methods

        public bool Contains(T item)
        {
            if (item == null) return false;
            for (int i = 0; i != m_Count; ++i)
            {
                if (item.Equals(m_Items[i])) return true;
            }
            return false;
        }

        public IEnumerator<T> GetEnumerator()
        {
            if (m_Count == 0) yield break;
            for (int i = 0; i != m_Count; ++i)
            {
                yield return m_Items[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public T[] ToArray()
        {
            if (m_Count == 0) return null;
            T[] result = new T[m_Count];
            m_Items.CopyTo(result, 0);
            return result;
        }

        public T[] CreateCopy(int startIndex, int length)
        {
            if (startIndex < 0 || startIndex >= m_Items.Length)
                throw new ArgumentOutOfRangeException("The specified start index is out of the acceptable range.");
            if (length < 0 || startIndex + length > m_Items.Length)
                throw new ArgumentOutOfRangeException("The specified length is out of the valid range.");
            T[] result = new T[length];
            for (int i = 0, j = startIndex; i != length; ++i, ++j)
            {
                result[i] = m_Items[j];
            }
            return result;
        }

        public ReadOnlySpan<T> ToSpan()
        {
            if (m_Count == 0) return new ReadOnlySpan<T>();
            return new ReadOnlySpan<T>(m_Items);
        }

        #endregion
    }
}
