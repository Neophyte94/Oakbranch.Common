using System;
using System.Buffers;
using System.IO;
using System.Text.Json;

namespace Oakbranch.Common.Utility
{
    public ref struct Utf8JsonStreamReader
    {
        private sealed class SequenceSegment : ReadOnlySequenceSegment<byte>, IDisposable
        {
            public byte[] Buffer { get; }

            private SequenceSegment m_Previous;
            public SequenceSegment Previous => m_Previous;

            public SequenceSegment(int size, SequenceSegment previous)
            {
                Buffer = new byte[size];
                m_Previous = previous;

                Memory = new ReadOnlyMemory<byte>(Buffer);
                RunningIndex = previous?.RunningIndex + previous?.Memory.Length ?? 0;
            }

            public void SetNext(SequenceSegment next) => Next = next;

            public void Dispose()
            {
                if (m_Previous != null)
                {
                    m_Previous?.Dispose();
                    m_Previous = null;
                }
            }
        }

        private readonly Stream m_Stream;
        private readonly int m_BufferSize;

        private SequenceSegment m_FirstSegment;
        private int m_FirstSegmentStartIndex;
        private SequenceSegment m_LastSegment;
        private int m_LastSegmentEndIndex;

        private Utf8JsonReader m_Reader;
        private bool m_IsFinalBlock;

        public int CurrentDepth => m_Reader.CurrentDepth;
        public bool HasValueSequence => m_Reader.HasValueSequence;
        public long TokenStartIndex => m_Reader.TokenStartIndex;
        public JsonTokenType TokenType => m_Reader.TokenType;
        public ReadOnlySequence<byte> ValueSequence => m_Reader.ValueSequence;
        public ReadOnlySpan<byte> ValueSpan => m_Reader.ValueSpan;

        public Utf8JsonStreamReader(Stream stream, int bufferSize, JsonReaderOptions options)
        {
            m_Stream = stream ?? throw new ArgumentNullException(nameof(stream));
            m_BufferSize = Math.Max(bufferSize, 16);

            m_FirstSegment = null;
            m_FirstSegmentStartIndex = 0;
            m_LastSegment = null;
            m_LastSegmentEndIndex = -1;

            m_Reader = new Utf8JsonReader(null, false, new JsonReaderState(options));
            m_IsFinalBlock = false;
        }

        public bool Read()
        {
            // read could be unsuccessful due to insufficient buffer size, retrying in loop with additional buffer segments
            while (!m_Reader.Read())
            {
                if (m_IsFinalBlock)
                    return false;
                MoveNext();
            }
            return true;
        }

        private void MoveNext()
        {
            var firstSegment = m_FirstSegment;
            m_FirstSegmentStartIndex += (int)m_Reader.BytesConsumed;

            // release previous segments if possible
            while (firstSegment?.Memory.Length <= m_FirstSegmentStartIndex)
            {
                m_FirstSegmentStartIndex -= firstSegment.Memory.Length;
                firstSegment.Dispose();
                firstSegment = (SequenceSegment)firstSegment.Next;
            }

            // create a new segment
            var newSegment = new SequenceSegment(m_BufferSize, m_LastSegment);

            if (firstSegment != null)
            {
                m_FirstSegment = firstSegment;
                m_LastSegment?.SetNext(newSegment);
                m_LastSegment = newSegment;
            }
            else
            {
                m_FirstSegment = m_LastSegment = newSegment;
                m_FirstSegmentStartIndex = 0;
            }

            // read data from stream
            m_LastSegmentEndIndex = m_Stream.Read(newSegment.Buffer, 0, m_BufferSize);
            m_IsFinalBlock = m_LastSegmentEndIndex < newSegment.Buffer.Length;
            m_Reader = new Utf8JsonReader(
                new ReadOnlySequence<byte>(m_FirstSegment, m_FirstSegmentStartIndex, m_LastSegment, m_LastSegmentEndIndex),
                m_IsFinalBlock,
                m_Reader.CurrentState);
        }

        public bool GetBoolean() => m_Reader.GetBoolean();
        public byte GetByte() => m_Reader.GetByte();
        public byte[] GetBytesFromBase64() => m_Reader.GetBytesFromBase64();
        public string GetComment() => m_Reader.GetComment();
        public DateTime GetDateTime() => m_Reader.GetDateTime();
        public DateTimeOffset GetDateTimeOffset() => m_Reader.GetDateTimeOffset();
        public decimal GetDecimal() => m_Reader.GetDecimal();
        public double GetDouble() => m_Reader.GetDouble();
        public Guid GetGuid() => m_Reader.GetGuid();
        public short GetInt16() => m_Reader.GetInt16();
        public int GetInt32() => m_Reader.GetInt32();
        public long GetInt64() => m_Reader.GetInt64();
        public sbyte GetSByte() => m_Reader.GetSByte();
        public float GetSingle() => m_Reader.GetSingle();
        public string GetString() => m_Reader.GetString();
        public uint GetUInt32() => m_Reader.GetUInt32();
        public ulong GetUInt64() => m_Reader.GetUInt64();
        public bool TryGetDecimal(out byte value) => m_Reader.TryGetByte(out value);
        public bool TryGetBytesFromBase64(out byte[] value) => m_Reader.TryGetBytesFromBase64(out value);
        public bool TryGetDateTime(out DateTime value) => m_Reader.TryGetDateTime(out value);
        public bool TryGetDateTimeOffset(out DateTimeOffset value) => m_Reader.TryGetDateTimeOffset(out value);
        public bool TryGetDecimal(out decimal value) => m_Reader.TryGetDecimal(out value);
        public bool TryGetDouble(out double value) => m_Reader.TryGetDouble(out value);
        public bool TryGetGuid(out Guid value) => m_Reader.TryGetGuid(out value);
        public bool TryGetInt16(out short value) => m_Reader.TryGetInt16(out value);
        public bool TryGetInt32(out int value) => m_Reader.TryGetInt32(out value);
        public bool TryGetInt64(out long value) => m_Reader.TryGetInt64(out value);
        public bool TryGetSByte(out sbyte value) => m_Reader.TryGetSByte(out value);
        public bool TryGetSingle(out float value) => m_Reader.TryGetSingle(out value);
        public bool TryGetUInt16(out ushort value) => m_Reader.TryGetUInt16(out value);
        public bool TryGetUInt32(out uint value) => m_Reader.TryGetUInt32(out value);
        public bool TryGetUInt64(out ulong value) => m_Reader.TryGetUInt64(out value);

        public void Skip()
        {
            switch (m_Reader.TokenType)
            {
                case JsonTokenType.StartArray:
                    JsonUtility.SkipCurrentArray(ref this);
                    break;
                case JsonTokenType.StartObject:
                    JsonUtility.SkipCurrentObject(ref this);
                    break;
                case JsonTokenType.PropertyName:
                    if (!Read())
                        throw new JsonException($"The current JSON token is {m_Reader.TokenType} and cannot be skipped.");
                    break;
                case JsonTokenType.None:
                    throw new JsonException("The current JSON token is None and cannot be skipped.");
                default:
                    break;
            }
        }

        public bool TrySkip()
        {
            switch (m_Reader.TokenType)
            {
                case JsonTokenType.StartArray:
                    JsonUtility.SkipCurrentArray(ref this);
                    return true;
                case JsonTokenType.StartObject:
                    JsonUtility.SkipCurrentObject(ref this);
                    return true;
                case JsonTokenType.PropertyName:
                    return Read();
                case JsonTokenType.None:
                    return false;
                default:
                    return true;
            }
        }

        public void Dispose()
        {
            if (m_LastSegment != null)
            {
                m_LastSegment?.Dispose();
            }
        }
    }
}
