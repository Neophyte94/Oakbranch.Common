using System;
using System.Collections.Generic;

namespace Oakbranch.Common.Utility
{
    /// <summary>
    /// Provides functions for writing individual bits into a packed byte array. Each byte may contain up to 8 bits.
    /// </summary>
    public class BitStreamWriter
    {
        private readonly Queue<byte> m_Buffer;
        private int m_CurrentByte;
        private int m_CurrentBitIdx;

        /// <summary>
        /// Gets the current length of a stream in bits.
        /// </summary>
        public int Length
        {
            get
            {
                return 8 * m_Buffer.Count + (7 - m_CurrentBitIdx);
            }
        }

        /// <summary>
        /// Creates a new writing bit stream.
        /// </summary>
        /// <param name="bufferSize">A size of the initial buffer (in bits)</param>
        public BitStreamWriter(int bufferSize)
        {
            if (bufferSize < 1) 
                bufferSize = 1;
            m_Buffer = new Queue<byte>(bufferSize % 8 == 0 ? bufferSize >> 3 : (bufferSize >> 3) + 1);
            m_CurrentBitIdx = 7;
        }

        public void Write(bool bit)
        {
            if (bit) m_CurrentByte |= 1 << m_CurrentBitIdx;
            if (--m_CurrentBitIdx == -1)
            {
                StoreByte();
                m_CurrentBitIdx = 7;
            }
        }

        /// <summary>
        /// Writes bits from a specified byte array to the stream.
        /// </summary>
        /// <param name="content">A source array to write from. Each byte represents up to 8 individual bits.</param>
        /// <param name="count">A number of bits to write from the source array.</param>
        public void WriteBits(byte[] content, int count)
        {
            if (content == null)
                throw new ArgumentNullException(nameof(content));
            if (count < 0 || count > content.Length * 8)
                throw new ArgumentException("The specified number of bits to write is invalid.");
            int bitsLeft = count, sourceByteIdx = -1, sourceBitIdx = -1;
            byte sourceByte = 0;
            while (bitsLeft-- != 0)
            {
                if (sourceBitIdx == -1)
                {
                    sourceByte = content[++sourceByteIdx];
                    sourceBitIdx = 7;
                }
                Write((sourceByte & (1 << sourceBitIdx--)) != 0);
            }
        }

        private void StoreByte()
        {
            m_Buffer.Enqueue((byte)m_CurrentByte);
            m_CurrentByte = 0;
        }

        public byte[] ToArray()
        {
            if (m_CurrentBitIdx != 7) StoreByte();
            return m_Buffer.ToArray();
        }
    }
}
