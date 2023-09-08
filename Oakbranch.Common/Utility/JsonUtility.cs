using System;
using System.Text.Json;

namespace Oakbranch.Common.Utility
{
    public static class JsonUtility
    {
        public static JsonWriterOptions WriterOptions { get; } = new JsonWriterOptions()
        {
            Indented = false,
#if DEBUG
            SkipValidation = false,
#else
            SkipValidation = true,
#endif
        };

        public static JsonReaderOptions ReaderOptions { get; } = new JsonReaderOptions();

        public static void ReadObjectStart(ref Utf8JsonReader reader)
        {
            if (!reader.Read() || reader.TokenType != JsonTokenType.StartObject)
                throw new JsonException($"An object start was expected but encountered \"{reader.TokenType}\".");
        }

        public static void ReadObjectStart(ref Utf8JsonStreamReader reader)
        {
            if (!reader.Read() || reader.TokenType != JsonTokenType.StartObject)
                throw new JsonException($"An object start was expected but encountered \"{reader.TokenType}\".");
        }

        public static void ReadObjectEnd(ref Utf8JsonReader reader)
        {
            if (!reader.Read() || reader.TokenType != JsonTokenType.EndObject)
                throw new JsonException($"An object end was expected but encountered \"{reader.TokenType}\".");
        }

        public static void ReadObjectEnd(ref Utf8JsonStreamReader reader)
        {
            if (!reader.Read() || reader.TokenType != JsonTokenType.EndObject)
                throw new JsonException($"An object end was expected but encountered \"{reader.TokenType}\".");
        }

        public static void ValidateObjectStartToken(ref Utf8JsonReader reader)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
                throw new JsonException($"An object start was expected but encountered \"{reader.TokenType}\".");
        }

        public static void ValidateObjectStartToken(ref Utf8JsonStreamReader reader)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
                throw new JsonException($"An object start was expected but encountered \"{reader.TokenType}\".");
        }

        public static void ReadArrayStart(ref Utf8JsonReader reader)
        {
            if (!reader.Read() || reader.TokenType != JsonTokenType.StartArray)
                throw new JsonException($"An array's start was expected but encountered \"{reader.TokenType}\".");
        }

        public static void ReadArrayStart(ref Utf8JsonStreamReader reader)
        {
            if (!reader.Read() || reader.TokenType != JsonTokenType.StartArray)
                throw new JsonException($"An array's start was expected but encountered \"{reader.TokenType}\".");
        }

        public static void ReadArrayEnd(ref Utf8JsonReader reader)
        {
            if (!reader.Read() || reader.TokenType != JsonTokenType.EndArray)
                throw new JsonException($"An array's end was expected but encountered \"{reader.TokenType}\".");
        }

        public static void ReadArrayEnd(ref Utf8JsonStreamReader reader)
        {
            if (!reader.Read() || reader.TokenType != JsonTokenType.EndArray)
                throw new JsonException($"An array's end was expected but encountered \"{reader.TokenType}\".");
        }

        public static void ValidateArrayStartToken(ref Utf8JsonReader reader)
        {
            if (reader.TokenType != JsonTokenType.StartArray)
                throw new JsonException($"An array's start was expected but encountered \"{reader.TokenType}\".");
        }

        public static void ValidateArrayStartToken(ref Utf8JsonStreamReader reader)
        {
            if (reader.TokenType != JsonTokenType.StartArray)
                throw new JsonException($"An array's start was expected but encountered \"{reader.TokenType}\".");
        }

        public static string ReadProperty(ref Utf8JsonReader reader)
        {
            if (!reader.Read() || reader.TokenType != JsonTokenType.PropertyName)
                throw new JsonException($"A propertie's name was expected but \"{reader.TokenType}\" encountered.");
            return reader.GetString();
        }

        public static string ReadProperty(ref Utf8JsonStreamReader reader)
        {
            if (!reader.Read() || reader.TokenType != JsonTokenType.PropertyName)
                throw new JsonException($"A propertie's name was expected but \"{reader.TokenType}\" encountered.");
            return reader.GetString();
        }

        public static void ReadExactProperty(ref Utf8JsonReader reader, string propName)
        {
            if (String.IsNullOrWhiteSpace(propName))
                throw new ArgumentNullException("A name of the property to read cannot be empty.");
            if (!reader.Read() || reader.TokenType != JsonTokenType.PropertyName)
                throw new JsonException($"A propertie's name was expected but \"{reader.TokenType}\" encountered.");
            if (reader.GetString() != propName)
                throw new JsonException($"The property \"{propName}\" was expected but \"{reader.GetString()}\" encountered.");
        }

        public static void ReadExactProperty(ref Utf8JsonStreamReader reader, string propName)
        {
            if (String.IsNullOrWhiteSpace(propName))
                throw new ArgumentNullException("A name of the property to read cannot be empty.");
            if (!reader.Read() || reader.TokenType != JsonTokenType.PropertyName)
                throw new JsonException($"A propertie's name was expected but \"{reader.TokenType}\" encountered.");
            if (reader.GetString() != propName)
                throw new JsonException($"The property \"{propName}\" was expected but \"{reader.GetString()}\" encountered.");
        }

        public static void ValidatePropertyNameToken(ref Utf8JsonReader reader)
        {
            if (reader.TokenType != JsonTokenType.PropertyName)
                throw new JsonException($"A propertie's name was expected but \"{reader.TokenType}\" encountered.");
        }

        public static void ValidatePropertyNameToken(ref Utf8JsonStreamReader reader)
        {
            if (reader.TokenType != JsonTokenType.PropertyName)
                throw new JsonException($"A propertie's name was expected but \"{reader.TokenType}\" encountered.");
        }

        public static JsonException GenerateUnknownPropertyException(string propName)
        {
            return new JsonException($"An unknown property was encountered: \"{propName}\".");
        }

        public static JsonException GenerateNoPropertyValueException(string propName)
        {
            return new JsonException($"A value of the property \"{propName}\" was expected but the end of the data was reached.");
        }

        public static void SkipCurrentObject(ref Utf8JsonReader reader)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
                throw new Exception($"The current JSON reader token is not StartObject but {reader.TokenType}.");

            int depth = reader.CurrentDepth;
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject && reader.CurrentDepth <= depth) break;
            }
        }

        public static void SkipCurrentObject(ref Utf8JsonStreamReader reader)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
                throw new Exception($"The current JSON reader token is not StartObject but {reader.TokenType}.");

            int depth = reader.CurrentDepth;
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject && reader.CurrentDepth <= depth) break;
            }
        }

        public static void SkipCurrentArray(ref Utf8JsonReader reader)
        {
            if (reader.TokenType != JsonTokenType.StartArray)
                throw new Exception($"The current JSON reader token is not StartArray but {reader.TokenType}.");

            int depth = reader.CurrentDepth;
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndArray && reader.CurrentDepth <= depth) break;
            }
        }

        public static void SkipCurrentArray(ref Utf8JsonStreamReader reader)
        {
            if (reader.TokenType != JsonTokenType.StartArray)
                throw new Exception($"The current JSON reader token is not StartArray but {reader.TokenType}.");

            int depth = reader.CurrentDepth;
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndArray && reader.CurrentDepth <= depth) break;
            }
        }
    }
}
