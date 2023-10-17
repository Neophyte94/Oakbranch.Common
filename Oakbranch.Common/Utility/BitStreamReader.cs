using System;

namespace Oakbranch.Common.Utility
{
    /// <summary>
    /// Provides functions for reading individual bits from a predefined source.
    /// </summary>
    public class BitStreamReader
    {
        private readonly byte[] m_Data;
        private int m_CurrentByteIdx;
        private int m_CurrentBitIdx;
        private bool m_HasReachedEnd;
        public bool HasReachedEnd => m_HasReachedEnd;

        /// <summary>
        /// Creates a reading bit stream from a specified source bytes array.
        /// </summary>
        /// <param name="data">A source array of bytes. Each byte represents up to 8 consecutive bits.</param>
        public BitStreamReader(byte[] data)
        {
            m_Data = data;
            if (m_Data == null || m_Data.Length == 0)
            {
                m_HasReachedEnd = true;
            }

            m_CurrentByteIdx = -1;
            m_CurrentBitIdx = -1;
        }

        /// <summary>
        /// Tries to read the next bit. If the bit is successfully read then the method returns its value (0 or 1). 
        /// If the end of the stream is reached the method returns -1.
        /// </summary>
        /// <returns>A value of bit (0 or 1), or -1 if the end of the stream is reached.</returns>
        private int ReadNext()
        {
            if (m_HasReachedEnd) return -1;
            if (m_CurrentBitIdx == -1)
            {
                m_CurrentBitIdx = 7;
                if (++m_CurrentByteIdx == m_Data.Length)
                {
                    m_HasReachedEnd = true;
                    return -1;
                }
            }
            byte source = m_Data[m_CurrentByteIdx];
            return (source & (1 << m_CurrentBitIdx--)) == 0 ? 0 : 1;
        }

        /// <summary>
        /// Reads one bit and stores a boolean value represented by it into the out variable.
        /// </summary>
        /// <param name="val">A variable to store a result in.</param>
        /// <returns><see langword="true"/> if a boolean value was successfully read, otherwise <see langword="false"/>.</returns>
        public bool ReadBool(out bool val)
        {
            val = false;
            if (m_HasReachedEnd) return false;
            int a = ReadNext();
            if (a == -1) return false;
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
                throw new ArgumentNullException(
                    nameof(buffer));
            if (count < 1)
                throw new ArgumentException(String.Format(
                    "The specified number of bits to read ({0}) is invalid.", count),
                    nameof(count));
            if (buffer.Length * 8 < count)
                throw new ArgumentException(String.Format(
                    "The passed buffer has insufficient size ({0}) " +
                    "to contain the specified number of bits ({1}).",
                    buffer.Length, count));
            if (m_HasReachedEnd) return 0;
            int bitsRead = 0, currentByte = 0, targetByte = 0, targetBit = 7;
            do
            {
                int a = ReadNext();
                if (a == -1) return bitsRead;
                if (a == 1) currentByte |= (1 << targetBit);
                if (--targetBit == -1 | ++bitsRead == count)
                {
                    buffer[targetByte++] = (byte)currentByte;
                    if (bitsRead == count) break;
                    currentByte = 0;
                    targetBit = 7;
                }
            } while (true);
            return bitsRead;
        }
    }
}
