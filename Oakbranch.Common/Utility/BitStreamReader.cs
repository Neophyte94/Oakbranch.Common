using System;

namespace Oakbranch.Common.Utility
{
    /// <summary>
    /// Provides functions for reading individual bits from a predefined source.
    /// </summary>
    public class BitStreamReader
    {
        private readonly byte[] _data;
        private int _currentByteIdx;
        private int _currentBitIdx;

        private bool _hasReachedEnd;
        public bool HasReachedEnd => _hasReachedEnd;

        /// <summary>
        /// Creates a reading bit stream from a specified source bytes array.
        /// </summary>
        /// <param name="data">A source array of bytes. Each byte represents up to 8 consecutive bits.</param>
        public BitStreamReader(byte[] data)
        {
            _data = data;

            if (_data == null || _data.Length == 0)
            {
                _hasReachedEnd = true;
            }

            _currentByteIdx = -1;
            _currentBitIdx = -1;
        }

        /// <summary>
        /// Tries to read the next bit. If the bit is successfully read then the method returns its value (0 or 1). 
        /// If the end of the stream is reached the method returns -1.
        /// </summary>
        /// <returns>A value of bit (0 or 1), or -1 if the end of the stream is reached.</returns>
        private int ReadNext()
        {
            if (_hasReachedEnd)
            {
                return -1;
            }

            if (_currentBitIdx == -1)
            {
                _currentBitIdx = 7;
                if (++_currentByteIdx == _data.Length)
                {
                    _hasReachedEnd = true;
                    return -1;
                }
            }

            byte source = _data[_currentByteIdx];
            return (source & (1 << _currentBitIdx--)) == 0 ? 0 : 1;
        }

        /// <summary>
        /// Reads one bit and stores a boolean value represented by it into the out variable.
        /// </summary>
        /// <param name="val">A variable to store a result in.</param>
        /// <returns><see langword="true"/> if a boolean value was successfully read, otherwise <see langword="false"/>.</returns>
        public bool ReadBool(out bool val)
        {
            val = false;
            if (_hasReachedEnd)
            {
                return false;
            }

            int a = ReadNext();
            if (a == -1)
            {
                return false;
            }

            val = a != 0;
            return true;
        }

        /// <summary>
        /// Reads the specified number of bits from the stream and saves them to the buffer.
        /// </summary>
        /// <param name="buffer">A target array to save bits into. Each byte of the array represents up to 8 consecutive bits.</param>
        /// <param name="count">A number of bits to read.</param>
        /// <returns>A number of bits read.</returns>
        public int ReadBits(byte[] buffer, int count)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }
            if (count < 1)
            {
                throw new ArgumentException(
                    $"The specified number of bits to read ({count}) is invalid.",
                    nameof(count));
            }
            if (buffer.Length * 8 < count)
            {
                throw new ArgumentException(
                    $"The passed buffer has insufficient size ({buffer.Length}) " +
                    $"to contain the specified number of bits ({count}).");
            }

            if (_hasReachedEnd)
            {
                return 0;
            }

            int bitsRead = 0, currentByte = 0, targetByte = 0, targetBit = 7;
            do
            {
                int a = ReadNext();
                if (a == -1)
                {
                    return bitsRead;
                }

                if (a == 1)
                {
                    currentByte |= (1 << targetBit);
                }

                if (--targetBit == -1 | ++bitsRead == count)
                {
                    buffer[targetByte++] = (byte)currentByte;
                    if (bitsRead == count)
                    {
                        break;
                    }

                    currentByte = 0;
                    targetBit = 7;
                }
            } while (true);

            return bitsRead;
        }
    }
}
