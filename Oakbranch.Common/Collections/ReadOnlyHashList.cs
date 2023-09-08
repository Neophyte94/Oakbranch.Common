using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace Oakbranch.Common.Collections
{
    public class ReadOnlyHashList<T> : IReadOnlyList<T>
    {
        private const int HashSetThreshold = 10;

        private readonly T[] m_Items;

        private readonly int m_Count;
        public int Count => m_Count;

        public T this[int index]
        {
            get
            {
                if (index < 0 || index >= m_Count)
                    throw new IndexOutOfRangeException();
                return m_Items[index];
            }
        }

        public ReadOnlyHashList()
        {
            m_Items = null;
            m_Count = 0;
        }

        public ReadOnlyHashList(T item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            m_Items = new T[1] { item };
            m_Count = 1;
        }

        public ReadOnlyHashList(T[] items)
        {
            int count = items.Length;
            m_Items = new T[count];
            T item;
            HashSet<T> hashSet = null;
            for (int i = 0; i != count;)
            {
                item = items[i++];
                if (item == null) throw new ArgumentException("Null references are not allowed.");
                if (!Contains(ref hashSet, item))
                {
                    m_Items[m_Count++] = item;
                }
            }
        }

        public ReadOnlyHashList(IEnumerable<T> items)
        {
            m_Items = new T[4];
            HashSet<T> hashSet = null;
            foreach (T item in items)
            {
                if (item == null) throw new ArgumentException("Null references are not allowed.");
                if (!Contains(ref hashSet, item))
                {
                    EnsureCapacity(ref m_Items, m_Count + 1);
                    m_Items[m_Count++] = item;
                }
            }
        }

        public ReadOnlyHashList(IReadOnlyList<T> items)
        {
            if (items == null) throw new ArgumentNullException(nameof(items));
            int count = items.Count;
            m_Items = new T[count];
            HashSet<T> hashSet = null;
            T item;
            for (int i = 0; i != count;)
            {
                item = items[i++];
                if (item == null) throw new ArgumentException("Null references are not allowed.");
                if (!Contains(ref hashSet, item))
                {
                    m_Items[m_Count++] = item;
                }
            }
        }

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
                if (m_Count != 0) Array.Copy(buffer, temp, m_Count);
                buffer = temp;
            }
        }

        private bool Contains(ref HashSet<T> hashSet, T item)
        {
            if (hashSet == null && HashSetThreshold < m_Count)
            {
                hashSet = new HashSet<T>(m_Items);
            }
            if (hashSet == null)
            {
                for (int i = 0; i != m_Count; ++i)
                {
                    if (Equals(m_Items[i], item)) return true;
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
    }
}
