using System;
using System.Collections;
using System.Collections.Generic;

namespace Oakbranch.Common.Collections
{
    public sealed class ReadOnlyBuffer<T> : IReadOnlyList<T>
    {
        private readonly T[] m_Buffer;
        private readonly int m_FromIdx;
        private readonly int m_ToIdx;

        private readonly int m_Count;
        public int Count => m_Count;

        public T this[int index]
        {
            get
            {
                if (index > -1 && index < m_Count)
                    return m_Buffer[m_FromIdx + index];
                else
                    throw new IndexOutOfRangeException(
                        $"The specified index {index} is out of the acceptable range [0 ; {m_ToIdx - 1}].");
            }
        }

        public ReadOnlyBuffer(T[] source, int fromIdx, int toIdx)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (fromIdx < 0)
                throw new ArgumentOutOfRangeException(nameof(fromIdx));
            if (toIdx > source.Length)
                throw new ArgumentOutOfRangeException(nameof(toIdx));

            m_Buffer = source;
            m_FromIdx = fromIdx;
            m_ToIdx = toIdx;

            m_Count = toIdx - fromIdx;
            if (m_Count < 0)
                throw new ArgumentException($"The specified indices range [{fromIdx} ; {toIdx}) is invalid.");
        }

        public IEnumerator<T> GetEnumerator()
        {
            for (int i = m_FromIdx; i != m_ToIdx; ++i)
            {
                yield return m_Buffer[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => (this as IEnumerable<T>).GetEnumerator();
    }
}
