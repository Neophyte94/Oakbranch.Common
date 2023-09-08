using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.Json;
using Oakbranch.Common.Numerics;

namespace Oakbranch.Common.Utility
{
    public static class SerializationUtility
    {
        private const byte StringItemDiscr = 1;
        private const byte DoubleItemDiscr = 2;
        private const byte Int64ItemDiscr = 3;
        private const byte Int32ItemDiscr = 4;
        private const byte StringsListItemDiscr = 5;
        private const byte IntRangeDiscr = 6;
        private const byte DoubleRangeDiscr = 7;

        private static readonly BinaryFormatter s_BinaryFormatter = new BinaryFormatter();

        // The variable NET_7_0 is defined in the project settings.
#if !NET_7_0
        /// <summary>
        /// Returns the array of bytes containing the serialized passed object.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte[] SerializeObject(object value)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                s_BinaryFormatter.Serialize(ms, value);
                return ms.ToArray();
            }
        }

        /// <summary>
        /// Returns the object deserialized from the passed bytes array.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static object DeserializeObject(byte[] data)
        {
            using (MemoryStream ms = new MemoryStream(data, false))
            {
                return s_BinaryFormatter.Deserialize(ms);
            }
        }
#endif

        public static void WriteByteArray(this Stream outputStream, byte[] value)
        {
            if (value == null)
            {
                byte[] buffer = new byte[4];
                outputStream.Write(buffer, 0, 4);
            }
            else
            {
                byte[] buffer = BitConverter.GetBytes(value.Length);
                outputStream.Write(buffer, 0, 4);
                outputStream.Write(value, 0, value.Length);
            }
        }

        public static bool TryReadByteArray(this Stream inputStream, out byte[] value)
        {
            value = null;
            try
            {
                byte[] buffer = new byte[4];
                int bytesRead = inputStream.Read(buffer, 0, 4);
                if (bytesRead != 4) return false;
                int length = BitConverter.ToInt32(buffer, 0);
                if (length > 0)
                {
                    value = new byte[length];
                    bytesRead = inputStream.Read(value, 0, length);
                    if (bytesRead != length)
                    {
                        value = null;
                        return false;
                    }
                }
                return true;
            }
            catch (Exception exc)
            {
#if DEBUG
                Console.WriteLine(
                    "An error occurred while reading a serialized byte array. " +
                    "The error's description: \n{0}.", exc);
#endif
                return false;
            }
        }

        public static void WriteString(this Stream outputStream, string value)
        {
            WriteByteArray(outputStream, String.IsNullOrEmpty(value) ?
                null : Encoding.UTF8.GetBytes(value));
        }

        public static bool TryReadString(this Stream inputStream, out string value)
        {
            if (TryReadByteArray(inputStream, out byte[] data))
            {
                value = data == null ? null : Encoding.UTF8.GetString(data);
                return true;
            }
            else
            {
                value = null;
                return false;
            }
        }

        public static bool TryReadByte(this Stream inputStream, out byte value)
        {
            value = 0;
            try
            {
                int intVal = inputStream.ReadByte();
                if (intVal != -1)
                {
                    value = (byte)intVal;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception exc)
            {
#if DEBUG
                Console.WriteLine(
                    "An error occurred while reading a serialized byte. " +
                    "The error's description: \n{0}.", exc);
#endif
                return false;
            }
        }

        public static bool TryReadInt32(this Stream inputStream, out int value)
        {
            value = 0;
            try
            {
                byte[] buffer = new byte[4];
                if (inputStream.Read(buffer, 0, 4) == 4)
                {
                    value = BitConverter.ToInt32(buffer, 0);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception exc)
            {
#if DEBUG
                Console.WriteLine(
                    "An error occurred while reading a serialized 32-bit integer. " +
                    "The error's description: \n{0}.", exc);
#endif
                return false;
            }
        }

        public static bool TryReadInt64(this Stream inputStream, out long value)
        {
            value = 0;
            try
            {
                byte[] buffer = new byte[8];
                if (inputStream.Read(buffer, 0, 8) == 8)
                {
                    value = BitConverter.ToInt64(buffer, 0);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception exc)
            {
#if DEBUG
                Console.WriteLine(
                    "An error occurred while reading a serialized 64-bit integer. " +
                    "The error's description: \n{0}.", exc);
#endif
                return false;
            }
        }

        public static void WriteDictionary(Utf8JsonWriter writer, IReadOnlyCollection<KeyValuePair<string, object>> content, string propertyName)
        {
            if (writer == null)
                throw new ArgumentNullException(nameof(writer));
            if (content == null)
                throw new ArgumentNullException(nameof(content));
            if (String.IsNullOrWhiteSpace(propertyName))
                throw new ArgumentNullException(nameof(propertyName));
  
            writer.WriteStartArray(propertyName);
            writer.WriteNumberValue(content.Count);
            foreach (KeyValuePair<string, object> pair in content)
            {
                if (pair.Value == null) continue;
                writer.WriteStringValue(pair.Key);
                switch (pair.Value)
                {
                    case string strVal:
                        writer.WriteNumberValue(StringItemDiscr);
                        writer.WriteStringValue(strVal);
                        break;
                    case double dVal:
                        writer.WriteNumberValue(DoubleItemDiscr);
                        writer.WriteNumberValue(dVal);
                        break;
                    case long longVal:
                        writer.WriteNumberValue(Int64ItemDiscr);
                        writer.WriteNumberValue(longVal);
                        break;
                    case int intVal:
                        writer.WriteNumberValue(Int32ItemDiscr);
                        writer.WriteNumberValue(intVal);
                        break;
                    case List<string> lsVal:
                        writer.WriteNumberValue(StringsListItemDiscr);
                        writer.WriteStartArray();
                        writer.WriteNumberValue(lsVal.Count);
                        foreach (string strItem in lsVal)
                        {
                            writer.WriteStringValue(strItem);
                        }
                        writer.WriteEndArray();
                        break;
                    case IntRange intRange:
                        writer.WriteNumberValue(IntRangeDiscr);
                        writer.WriteStartArray();
                        writer.WriteNumberValue(intRange.Floor);
                        writer.WriteNumberValue(intRange.Ceil);
                        writer.WriteEndArray();
                        break;
                    case DoubleRange dRange:
                        writer.WriteNumberValue(DoubleRangeDiscr);
                        writer.WriteStartArray();
                        writer.WriteNumberValue(dRange.Floor);
                        writer.WriteNumberValue(dRange.Ceil);
                        writer.WriteEndArray();
                        break;
                    default:
                        throw new NotSupportedException($"Values of the type {pair.Value.GetType().Name} are not supported.");
                }
            }
            writer.WriteEndArray();
        }

        public static List<KeyValuePair<string, object>> ReadDictionary(ref Utf8JsonStreamReader reader)
        {
            if (reader.TokenType != JsonTokenType.StartArray)
                throw new JsonException($"An array's start was expected but \"{reader.TokenType}\" encountered.");

            if (!reader.Read() || reader.TokenType != JsonTokenType.Number)
                throw new JsonException($"A timeseries count value was expected but \"{reader.TokenType}\" encountered.");

            int count = reader.GetInt32();
            string key;
            byte discr;
            List<KeyValuePair<string, object>> content = new List<KeyValuePair<string, object>>(count);

            while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
            {
                if (reader.TokenType != JsonTokenType.String)
                    throw new JsonException($"A pair key was expected but \"{reader.TokenType}\" encountered.");

                key = reader.GetString();

                if (!reader.Read() || reader.TokenType != JsonTokenType.Number)
                    throw new JsonException($"A pair discriminator was expected but \"{reader.TokenType}\" encountered.");
                discr = reader.GetByte();

                if (!reader.Read())
                    throw new JsonException($"A pair value was expected but \"{reader.TokenType}\" encountered.");
                if (discr == StringItemDiscr)
                {
                    content.Add(new KeyValuePair<string, object>(key, reader.GetString()));
                }
                else if (discr == DoubleItemDiscr)
                {
                    content.Add(new KeyValuePair<string, object>(key, reader.GetDouble()));
                }
                else if (discr == Int64ItemDiscr)
                {
                    content.Add(new KeyValuePair<string, object>(key, reader.GetInt64()));
                }
                else if (discr == Int32ItemDiscr)
                {
                    content.Add(new KeyValuePair<string, object>(key, reader.GetInt32()));
                }
                else if (discr == StringsListItemDiscr)
                {
                    if (reader.TokenType != JsonTokenType.StartArray)
                        throw new JsonException($"The start of a string items array was expected but \"{reader.TokenType}\" encountered.");

                    if (!reader.Read() || reader.TokenType != JsonTokenType.Number)
                        throw new JsonException($"The size of a string items array was expected but \"{reader.TokenType}\" encountered.");
                    List<string> items = new List<string>(reader.GetInt32());

                    while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
                    {
                        items.Add(reader.GetString());
                    }

                    if (reader.TokenType != JsonTokenType.EndArray)
                        throw new JsonException($"The end of the string items array was expected but \"{reader.TokenType}\" encountered.");
                    content.Add(new KeyValuePair<string, object>(key, items));
                }
                else if (discr == IntRangeDiscr)
                {
                    if (reader.TokenType != JsonTokenType.StartArray)
                        throw new JsonException($"The start of a numbers array was expected but \"{reader.TokenType}\" encountered.");

                    if (!reader.Read() || reader.TokenType != JsonTokenType.Number)
                        throw new JsonException($"The floor number was expected but \"{reader.TokenType}\" encountered.");
                    int floor = reader.GetInt32();

                    if (!reader.Read() || reader.TokenType != JsonTokenType.Number)
                        throw new JsonException($"The ceil number was expected but \"{reader.TokenType}\" encountered.");
                    int ceil = reader.GetInt32();

                    if (!reader.Read() || reader.TokenType != JsonTokenType.EndArray)
                        throw new JsonException($"The end of the numbers array was expected but \"{reader.TokenType}\" encountered.");
                    content.Add(new KeyValuePair<string, object>(key, new IntRange(floor, ceil)));
                }
                else if (discr == DoubleRangeDiscr)
                {
                    if (reader.TokenType != JsonTokenType.StartArray)
                        throw new JsonException($"The start of a numbers array was expected but \"{reader.TokenType}\" encountered.");

                    if (!reader.Read() || reader.TokenType != JsonTokenType.Number)
                        throw new JsonException($"The floor number was expected but \"{reader.TokenType}\" encountered.");
                    double floor = reader.GetDouble();

                    if (!reader.Read() || reader.TokenType != JsonTokenType.Number)
                        throw new JsonException($"The ceil number was expected but \"{reader.TokenType}\" encountered.");
                    double ceil = reader.GetDouble();

                    if (!reader.Read() || reader.TokenType != JsonTokenType.EndArray)
                        throw new JsonException($"The end of the numbers array was expected but \"{reader.TokenType}\" encountered.");
                    content.Add(new KeyValuePair<string, object>(key, new DoubleRange(floor, ceil)));
                }
            }

            return content;
        }
    }
}
