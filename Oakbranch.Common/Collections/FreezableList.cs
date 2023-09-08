using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Oakbranch.Common.Collections
{
    public sealed class FreezableList<T> : IList<T>
    {
        #region Instance members

        private T[] m_Items;

        private int m_Count;
        public int Count => m_Count;

        private bool m_IsReadOnly;
        public bool IsReadOnly => m_IsReadOnly;

        private int m_Version = int.MinValue;

#if DEBUG
        private readonly int m_InitialCapacity;
#endif

#endregion

        #region Instance indexers

        public T this[int index]
        {
            get
            {
                return m_Items[index];
            }
            set
            {
                Insert(index, value);
            }
        }

        #endregion

        #region Instance constructors

        public FreezableList()
        {
            m_Items = new T[4];
        }

        public FreezableList(int capacity)
        {
            m_Items = new T[Math.Max(capacity, 1)];
#if DEBUG
            m_InitialCapacity = capacity;
#endif
        }

        public FreezableList(T[] items)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            m_Count = items.Length;
            m_Items = new T[Math.Max(m_Count, 1)];
            for (int i = 0; i != m_Count; ++i)
            {
                m_Items[i] = items[i];
            }
        }

        public FreezableList(ICollection<T> items)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            m_Items = new T[Math.Max(items.Count, 1)];
            foreach (T item in items)
            {
                m_Items[m_Count++] = item;
            }
        }

        public FreezableList(IEnumerable<T> items)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            m_Items = items.ToArray();
            m_Count = m_Items.Length;
            if (m_Count == 0)
                m_Items = new T[1];
        }

        #endregion

        #region Instance methods

        public void Add(T item)
        {
            ThrowExceptionIfFrozen();
            EnsureCapacity(m_Count + 1);
            m_Items[m_Count++] = item;
            ++m_Version;
        }

        public void Clear()
        {
            ThrowExceptionIfFrozen();
            Array.Clear(m_Items, 0, m_Count);
            m_Count = 0;
            ++m_Version;
        }

        public bool Contains(T item)
        {
            if (item == null) 
            {
                for (int i = 0; i < m_Count;)
                {
                    if (m_Items[i++] == null) return true;
                }
                return false;
            }
            else
            {
                for (int i = 0; i < m_Count;)
                {
                    if (item.Equals(m_Items[i++])) return true;
                }
                return false;
            }
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));

            int count = m_Count;
            if (array.Length - arrayIndex < count)
                throw new ArgumentException($"The specified array is not large enough ({array.Length}) " +
                    $"to contain {count} items from the index {arrayIndex}.");

            for (int i = 0, j = arrayIndex; i != count;)
            {
                array[j++] = m_Items[i++];
            }
        }

        public int IndexOf(T item)
        {
            if (item == null)
            {
                for (int i = 0; i < m_Count; ++i)
                {
                    if (m_Items[i] == null) return i;
                }
                return -1;
            }
            else
            {
                for (int i = 0; i < m_Count; ++i)
                {
                    if (item.Equals(m_Items[i])) return i;
                }
                return -1;
            }
        }

        public void Insert(int index, T item)
        {
            if (index == m_Count)
            {
                Add(item);
                return;
            }

            ThrowExceptionIfFrozen();
            if (index < 0 || index > m_Count)
                throw new IndexOutOfRangeException();

            m_Items[index] = item;            
            ++m_Version;
        }

        public bool Remove(T item)
        {
            ThrowExceptionIfFrozen();

            int idx = IndexOf(item);
            if (idx == -1) return false;

            RemoveAt(idx);
            return true;
        }

        public void RemoveAt(int index)
        {
            ThrowExceptionIfFrozen();
            if (index < 0 || index >= m_Count)
                throw new IndexOutOfRangeException();

            for (int i = index + 1; i < m_Count; ++i)
            {
                m_Items[index - 1] = m_Items[index];
            }
            m_Items[index] = default;
            --m_Count;
            ++m_Version;
        }

        private void EnsureCapacity(int neededCapacity)
        {
            if (m_Items.Length < neededCapacity)
            {
                T[] buffer = new T[m_Items.Length << 1];
                m_Items.CopyTo(buffer, 0);
                m_Items = buffer;
            }
        }

        public void Freeze()
        {
            m_IsReadOnly = true;
        }

        private void ThrowExceptionIfFrozen()
        {
            if (m_IsReadOnly)
                throw new InvalidOperationException("A list cannot be muted any more since it has been frozen.");
        }

        public IEnumerator<T> GetEnumerator()
        {
            int version = m_Version;
            for (int i = 0; i != m_Count;)
            {
                if (version != m_Version)
                    throw new Exception("A collection was modified during enumeration.");
                yield return m_Items[i++];
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<T>)this).GetEnumerator();

        #endregion
    }
}
