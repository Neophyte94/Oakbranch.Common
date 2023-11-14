using System;
using System.Collections.Generic;

namespace Oakbranch.Common.Utility
{
    /// <summary>
    /// Provides functions for writing individual bits into a packed byte array. Each byte may contain up to 8 bits.
    /// </summary>
    public class BitStreamWriter
    {
        private readonly Queue<byte> _buffer;
        private int _currentByte;
        private int _currentBitIdx;

        /// <summary>
        /// Gets the current length of a stream in bits.
        /// </summary>
        public int Length
        {
            get
            {
                return 8 * _buffer.Count + (7 - _currentBitIdx);
            }
        }

        /// <summary>
        /// Creates a new writing bit stream.
        /// </summary>
        /// <param name="bufferSize">A size of the initial buffer (in bits)</param>
        public BitStreamWriter(int bufferSize)
        {
            if (bufferSize < 1)
            {
                bufferSize = 1;
            }

            _buffer = new Queue<byte>(bufferSize % 8 == 0 ? bufferSize >> 3 : (bufferSize >> 3) + 1);
            _currentBitIdx = 7;
        }

        public void Write(bool bit)
        {
            if (bit)
            {
                _currentByte |= 1 << _currentBitIdx;
            }
            if (--_currentBitIdx == -1)
            {
                StoreByte();
                _currentBitIdx = 7;
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
            {
                throw new ArgumentNullException(nameof(content));
            }
            if (count < 0 || count > content.Length * 8)
            {
                throw new ArgumentException("The specified number of bits to write is invalid.");
            }

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
            _buffer.Enqueue((byte)_currentByte);
            _currentByte = 0;
        }

        public byte[] ToArray()
        {
            if (_currentBitIdx != 7)
            {
                StoreByte();
            }

            return _buffer.ToArray();
        }
    }
}
