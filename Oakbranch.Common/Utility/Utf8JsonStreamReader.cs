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

            private SequenceSegment _previous;
            public SequenceSegment Previous => _previous;

            public SequenceSegment(int size, SequenceSegment previous)
            {
                Buffer = new byte[size];
                _previous = previous;

                Memory = new ReadOnlyMemory<byte>(Buffer);
                RunningIndex = previous?.RunningIndex + previous?.Memory.Length ?? 0;
            }

            public void SetNext(SequenceSegment next) => Next = next;

            public void Dispose()
            {
                if (_previous != null)
                {
                    _previous?.Dispose();
                    _previous = null;
                }
            }
        }

        private readonly Stream _stream;
        private readonly int _bufferSize;

        private SequenceSegment _firstSegment;
        private int _firstSegmentStartIndex;
        private SequenceSegment _lastSegment;
        private int _lastSegmentEndIndex;

        private Utf8JsonReader _reader;
        private bool _isFinalBlock;

        public int CurrentDepth => _reader.CurrentDepth;
        public bool HasValueSequence => _reader.HasValueSequence;
        public long TokenStartIndex => _reader.TokenStartIndex;
        public JsonTokenType TokenType => _reader.TokenType;
        public ReadOnlySequence<byte> ValueSequence => _reader.ValueSequence;
        public ReadOnlySpan<byte> ValueSpan => _reader.ValueSpan;

        public Utf8JsonStreamReader(Stream stream, int bufferSize, JsonReaderOptions options)
        {
            _stream = stream ?? throw new ArgumentNullException(nameof(stream));
            _bufferSize = Math.Max(bufferSize, 16);

            _firstSegment = null;
            _firstSegmentStartIndex = 0;
            _lastSegment = null;
            _lastSegmentEndIndex = -1;

            _reader = new Utf8JsonReader(null, false, new JsonReaderState(options));
            _isFinalBlock = false;
        }

        public bool Read()
        {
            // Read could be unsuccessful due to insufficient buffer size,
            // retrying in loop with additional buffer segments.
            while (!_reader.Read())
            {
                if (_isFinalBlock)
                {
                    return false;
                }

                MoveNext();
            }

            return true;
        }

        private void MoveNext()
        {
            var firstSegment = _firstSegment;
            _firstSegmentStartIndex += (int)_reader.BytesConsumed;

            // Release previous segments if possible.
            while (firstSegment?.Memory.Length <= _firstSegmentStartIndex)
            {
                _firstSegmentStartIndex -= firstSegment.Memory.Length;
                firstSegment.Dispose();
                firstSegment = (SequenceSegment)firstSegment.Next;
            }

            // Create a new segment.
            var newSegment = new SequenceSegment(_bufferSize, _lastSegment);

            if (firstSegment != null)
            {
                _firstSegment = firstSegment;
                _lastSegment?.SetNext(newSegment);
                _lastSegment = newSegment;
            }
            else
            {
                _firstSegment = _lastSegment = newSegment;
                _firstSegmentStartIndex = 0;
            }

            // Read data from stream.
            _lastSegmentEndIndex = _stream.Read(newSegment.Buffer, 0, _bufferSize);
            _isFinalBlock = _lastSegmentEndIndex < newSegment.Buffer.Length;
            _reader = new Utf8JsonReader(
                new ReadOnlySequence<byte>(_firstSegment, _firstSegmentStartIndex, _lastSegment, _lastSegmentEndIndex),
                _isFinalBlock,
                _reader.CurrentState);
        }


        public void Skip()
        {
            switch (_reader.TokenType)
            {
                case JsonTokenType.StartArray:
                    JsonUtility.SkipCurrentArray(ref this);
                    break;
                case JsonTokenType.StartObject:
                    JsonUtility.SkipCurrentObject(ref this);
                    break;
                case JsonTokenType.PropertyName:
                    if (!Read())
                        throw new JsonException($"The current JSON token is {_reader.TokenType} and cannot be skipped.");
                    break;
                case JsonTokenType.None:
                    throw new JsonException("The current JSON token is None and cannot be skipped.");
                default:
                    break;
            }
        }

        public bool TrySkip()
        {
            switch (_reader.TokenType)
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

        public bool GetBoolean() => _reader.GetBoolean();
        public byte GetByte() => _reader.GetByte();
        public byte[] GetBytesFromBase64() => _reader.GetBytesFromBase64();
        public string GetComment() => _reader.GetComment();
        public DateTime GetDateTime() => _reader.GetDateTime();
        public DateTimeOffset GetDateTimeOffset() => _reader.GetDateTimeOffset();
        public decimal GetDecimal() => _reader.GetDecimal();
        public double GetDouble() => _reader.GetDouble();
        public Guid GetGuid() => _reader.GetGuid();
        public short GetInt16() => _reader.GetInt16();
        public int GetInt32() => _reader.GetInt32();
        public long GetInt64() => _reader.GetInt64();
        public sbyte GetSByte() => _reader.GetSByte();
        public float GetSingle() => _reader.GetSingle();
        public string GetString() => _reader.GetString();
        public uint GetUInt32() => _reader.GetUInt32();
        public ulong GetUInt64() => _reader.GetUInt64();
        public bool TryGetDecimal(out byte value) => _reader.TryGetByte(out value);
        public bool TryGetBytesFromBase64(out byte[] value) => _reader.TryGetBytesFromBase64(out value);
        public bool TryGetDateTime(out DateTime value) => _reader.TryGetDateTime(out value);
        public bool TryGetDateTimeOffset(out DateTimeOffset value) => _reader.TryGetDateTimeOffset(out value);
        public bool TryGetDecimal(out decimal value) => _reader.TryGetDecimal(out value);
        public bool TryGetDouble(out double value) => _reader.TryGetDouble(out value);
        public bool TryGetGuid(out Guid value) => _reader.TryGetGuid(out value);
        public bool TryGetInt16(out short value) => _reader.TryGetInt16(out value);
        public bool TryGetInt32(out int value) => _reader.TryGetInt32(out value);
        public bool TryGetInt64(out long value) => _reader.TryGetInt64(out value);
        public bool TryGetSByte(out sbyte value) => _reader.TryGetSByte(out value);
        public bool TryGetSingle(out float value) => _reader.TryGetSingle(out value);
        public bool TryGetUInt16(out ushort value) => _reader.TryGetUInt16(out value);
        public bool TryGetUInt32(out uint value) => _reader.TryGetUInt32(out value);
        public bool TryGetUInt64(out ulong value) => _reader.TryGetUInt64(out value);

        public void Dispose()
        {
            if (_lastSegment != null)
            {
                _lastSegment?.Dispose();
            }
        }
    }
}
