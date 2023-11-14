using System;
using System.Collections;
using System.Collections.Generic;

namespace Oakbranch.Common.Collections
{
    /// <summary>
    /// Represents a read-only collection of ordered elements built upon a fixed segment of an array.
    /// <para>Implements <see cref="IReadOnlyList{T}"/>.</para>
    /// </summary>
    /// <typeparam name="T">The type of the elements.</typeparam>
    public sealed class ReadOnlyBuffer<T> : IReadOnlyList<T>
    {
        #region Instance members

        private readonly T[] _buffer;
        private readonly int _fromIdx;
        private readonly int _toIdx;

        private readonly int _count;
        public int Count => _count;

        #endregion

        #region Instance indexers

        public T this[int index]
        {
            get
            {
                if (index > -1 && index < _count)
                {
                    return _buffer[_fromIdx + index];
                }
                else
                {
                    throw new IndexOutOfRangeException(
                        $"The specified index {index} is out of the acceptable range [0 ; {_toIdx - 1}].");
                }
            }
        }

        #endregion

        #region Instance constructors

        public ReadOnlyBuffer(T[] source, int fromIdx, int toIdx)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            if (fromIdx < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(fromIdx));
            }
            if (toIdx > source.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(toIdx));
            }

            _buffer = source;
            _fromIdx = fromIdx;
            _toIdx = toIdx;

            _count = toIdx - fromIdx;
            if (_count < 0)
            {
                throw new ArgumentException($"The specified indices range [{fromIdx} ; {toIdx}) is invalid.");
            }
        }

        #endregion

        #region Instance methods

        public IEnumerator<T> GetEnumerator()
        {
            for (int i = _fromIdx; i != _toIdx; ++i)
            {
                yield return _buffer[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => (this as IEnumerable<T>).GetEnumerator();

        #endregion
    }
}
