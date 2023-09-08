using System;
using System.Text.Json;

namespace Oakbranch.Common.Utility
{
    public interface IJsonConverter<T>
    {
        T Read(ref Utf8JsonStreamReader reader);
        void Write(Utf8JsonWriter writer, T value);
    }
}
