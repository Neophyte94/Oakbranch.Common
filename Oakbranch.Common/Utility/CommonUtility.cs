using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Threading;

namespace Oakbranch.Common.Utility
{
    /// <summary>
    /// A helper class that provides a set of methods for various common operations.
    /// </summary>
    public static class CommonUtility
    {
        #region Constants

        private const int MaximumBufferSize = 65536;

        #endregion

        #region Static methods

        /// <summary>
        /// Determines whether two byte arrays are identical.
        /// </summary>
        /// <param name="a">The first byte array to compare.</param>
        /// <param name="b">The second byte array to compare.</param>
        /// <returns><see langword="true"/> if both values reference the same instance, or have identical content, or both are <see langword="null"/>; otherwise, <see langword="false"/>.</returns>
        public static bool AreEqual(byte[] a, byte[] b)
        {
            if (a == null)
            {
                return b == null;
            }
            else if (b == null)
            {
                return false;
            }
            else
            {
                int length = a.Length;
                if (length != b.Length) return false;
                for (int i = 0; i != length; ++i)
                {
                    if (a[i] != b[i]) return false;
                }
                return true;
            }
        }

        /// <summary>
        /// Determines whether two byte collections are identical.
        /// </summary>
        /// <param name="a">The first byte collection to compare.</param>
        /// <param name="b">The second byte collection to compare.</param>
        /// <returns><see langword="true"/> if both values reference the same instance, or have identical content, or both are <see langword="null"/>; otherwise, <see langword="false"/>.</returns>
        public static bool AreEqual(IEnumerable<byte> a, IEnumerable<byte> b)
        {
            if (a == null)
            {
                return b == null;
            }
            else if (b == null)
            {
                return false;
            }
            else
            {
                IEnumerator<byte> aEnum = a.GetEnumerator(),
                    bEnum = b.GetEnumerator();
                bool aNext, bNext;
                do
                {
                    aNext = aEnum.MoveNext();
                    bNext = bEnum.MoveNext();
                    if (!aNext || !bNext) break;
                    if (aEnum.Current != bEnum.Current) return false;
                } while (true);
                return aNext == bNext;
            }
        }

        [Obsolete("This method's signature is obsolete. Use the 'AreEqual()' method instead.", true)]
        public static bool Equal(byte[] a, byte[] b) => AreEqual(a, b);

        [Obsolete("This method's signature is obsolete. Use the 'AreEqual()' method instead.", true)]
        public static bool Equal(IEnumerable<byte> a, IEnumerable<byte> b) => AreEqual(a, b);

        /// <summary>
        /// Creates a new array that is a shallow copy of the provided array.
        /// </summary>
        /// <typeparam name="T">The type of elements in the array.</typeparam>
        /// <param name="source">The source array to create a copy from.</param>
        /// <returns>A new array that is a shallow copy of the source array.</returns>
        public static T[] CreateCopy<T>(this T[] source)
        {
            int length = source.Length;
            T[] result = new T[length];
            for (int i = 0; i != length; ++i)
            {
                result[i] = source[i];
            }
            return result;
        }

        /// <summary>
        /// Creates a new array that is a shallow copy of a portion of the provided array.
        /// </summary>
        /// <typeparam name="T">The type of elements in the array.</typeparam>
        /// <param name="source">The source array to create a copy from.</param>
        /// <param name="startIndex">The index in the source array at which to start copying.</param>
        /// <param name="length">The number of elements to copy from the source array.</param>
        /// <returns>A new array that is a shallow copy of a portion of the source array.</returns>
        public static T[] CreateCopy<T>(this T[] source, int startIndex, int length)
        {
            T[] result = new T[length];
            for (int i = 0; i != length;)
            {
                result[i++] = source[startIndex++];
            }
            return result;
        }

        /// <summary>
        /// Creates a new array of a specified length, where all elements have the same value.
        /// </summary>
        /// <typeparam name="T">The type of elements in the array.</typeparam>
        /// <param name="length">The length of the array to create.</param>
        /// <param name="value">The value to initialize all elements of the array.</param>
        /// <returns>A new array of the specified length with all elements set to the specified value.</returns>
        public static T[] CreateHomogenous<T>(int length, T value)
        {
            T[] result = new T[length];
            for (int i = 0; i != length;)
            {
                result[i++] = value;
            }
            return result;
        }

        /// <summary>
        /// Searches for the specified byte value within the array, starting from the specified index.
        /// </summary>
        /// <param name="source">The source byte array to search within.</param>
        /// <param name="value">The byte value to locate within the array.</param>
        /// <param name="startIndex">The index in the array at which to begin the search.</param>
        /// <returns>The index of the first occurrence of the value within the array, if found; otherwise, -1.</returns>
        public static int IndexOf(this byte[] source, byte value, int startIndex)
        {
            int length = source.Length;
            if (startIndex >= length)
                throw new IndexOutOfRangeException(
                    $"The specified start index ({startIndex}) is out the accepted range [0 ; {length - 1}].");
            while (startIndex != length)
            {
                if (source[startIndex] == value) return startIndex;
                ++startIndex;
            }
            return -1;
        }

        /// <summary>
        /// Searches for the specified byte value within the read-only span, starting from the specified index.
        /// </summary>
        /// <param name="source">The read-only span of bytes to search within.</param>
        /// <param name="value">The byte value to locate within the span.</param>
        /// <param name="startIndex">The index in the span at which to begin the search.</param>
        /// <returns>The index of the first occurrence of the value within the span, if found; otherwise, -1.</returns>
        public static int IndexOf(this ReadOnlySpan<byte> source, byte value, int startIndex)
        {
            int length = source.Length;
            if (startIndex >= length)
                throw new IndexOutOfRangeException(
                    $"The specified start index ({startIndex}) is out the accepted range [0 ; {length - 1}].");
            while (startIndex != length)
            {
                if (source[startIndex] == value) return startIndex;
                ++startIndex;
            }
            return -1;
        }

        /// <summary>
        /// Finds the index of the first digit in the provided string, starting from the specified index.
        /// </summary>
        /// <param name="s">The string to search for a digit.</param>
        /// <param name="startIdx">The index in the string at which to begin the search.</param>
        /// <returns>The index of the first digit in the string, if found; otherwise, -1.</returns>
        public static int IndexOfFirstDigit(string s, int startIdx)
        {
            int length = s.Length;
            for (int i = startIdx; i != length; ++i)
            {
                if (Char.IsDigit(s[i])) return i;
            }
            return -1;
        }

        /// <summary>
        /// Resets the content of the array referenced by the provided value, and sets the reference to <see langword="null"/>.
        /// </summary>
        /// <typeparam name="T">The type of elements in the array.</typeparam>
        /// <param name="array">The reference to the array to be cleared and set to <see langword="null"/>.</param>
        public static void Clear<T>(ref T[] array)
        {
            if (array != null)
            {
                Array.Clear(array, 0, array.Length);
                array = null;
            }
        }

        /// <summary>
        /// Calculates the age based on the provided birth date and the system time. 
        /// </summary>
        /// <param name="birthDate">The date of birth to calculate the age from.</param>
        /// <returns>The calculated age in years.</returns>
        public static int CalculateAge(DateTime birthDate)
        {
            DateTime now = DateTime.Now;
            int years = now.Year - birthDate.Year;

            if (now.Month < birthDate.Month ||
                (now.Month == birthDate.Month && now.Day < birthDate.Day))
                --years;

            return years;
        }

        /// <summary>
        /// Checks whether the provided string is a valid email address.
        /// </summary>
        /// <param name="email">The string to validate as an email address.</param>
        /// <returns><see langword="true"/> if the string is a valid email address; otherwise, <see langword="false"/>.</returns>
        public static bool IsValidEmail(string email)
        {
            try
            {
                var addr = new MailAddress(email);
                return String.Equals(addr.Address, email, StringComparison.InvariantCultureIgnoreCase);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Checks whether the specified string value is a valid file path.
        /// </summary>
        /// <param name="path">The path to validate.</param>
        /// <returns><see langword="true"/> if the path is a valid file path; otherwise, <see langword="false"/>.</returns>
        public static bool IsValidFilePath(string path)
        {
            FileInfo fi = null;
            try
            {
                fi = new FileInfo(path);
            }
            catch (ArgumentException) { }
            catch (PathTooLongException) { }
            catch (NotSupportedException) { }

#if NET_7_0
            return fi is not null;
#else
            return fi is object;
#endif
        }

        /// <summary>
        /// Checks whether the specified string value is a valid directory path.
        /// </summary>
        /// <param name="path">The path to validate.</param>
        /// <returns><see langword="true"/> if the path is a valid directory path; otherwise, <see langword="false"/>.</returns>
        public static bool IsValidDirectoryPath(string path)
        {
            DirectoryInfo di = null;
            try
            {
                di = new DirectoryInfo(path);
            }
            catch (ArgumentException) { }
            catch (PathTooLongException) { }
            catch (NotSupportedException) { }

#if NET_7_0
            return di is not null;
#else
            return di is object;
#endif
        }

        /// <summary>
        /// Adds a value to the given byte mask.
        /// </summary>
        /// <param name="value">The byte value to add to the mask.</param>
        /// <param name="mask">The reference to the variable storing the byte mask.</param>
        public static void AddToMask(byte value, ref byte mask)
        {
            mask |= value;
        }

        /// <summary>
        /// Adds a value to the given integer mask.
        /// </summary>
        /// <param name="value">The integer value to add to the mask.</param>
        /// <param name="mask">The reference to the variable storing the integer mask.</param>
        public static void AddToMask(int value, ref int mask)
        {
            mask |= value;
        }

        /// <summary>
        /// Removes a value from the given byte mask.
        /// </summary>
        /// <param name="value">The byte value to remove from the mask.</param>
        /// <param name="mask">The reference to the variable storing the integer mask.</param>
        public static void RemoveFromMask(byte value, ref byte mask)
        {
            mask &= (byte)(255 - value);
        }

        /// <summary>
        /// Removes a value from the given integer mask.
        /// </summary>
        /// <param name="value">The integer value to remove from the mask.</param>
        /// <param name="mask">The reference to the variable storing the integer mask.</param>
        public static void RemoveFromMask(int value, ref int mask)
        {
            mask &= ~value;
        }

        /// <summary>
        /// Checks whether a specific value is contained in the provided byte mask.
        /// </summary>
        /// <param name="value">The byte value to check in the mask.</param>
        /// <param name="mask">The byte mask to check for the specified value.</param>
        /// <returns><see langword="true"/> if the mask contains the specified value; otherwise, <see langword="false"/>.</returns>
        public static bool IsMaskMarked(byte value, byte mask)
        {
            return (mask & value) != 0;
        }

        /// <summary>
        /// Checks whether a specific value is contained in the provided integer mask.
        /// </summary>
        /// <param name="value">The integer value to check in the mask.</param>
        /// <param name="mask">The integer mask to check for the specified value.</param>
        /// <returns><see langword="true"/> if the mask contains the specified value; otherwise, <see langword="false"/>.</returns>
        public static bool IsMarkMarked(int value, int mask)
        {
            return (mask & value) != 0;
        }

        /// <summary>
        /// Copies the specified number of bytes from a source stream to a target stream.
        /// <para>This method is useful for stream with a length greater than <see cref="Int32.MaxValue"/>.</para>
        /// </summary>
        /// <param name="from">A stream to read from.</param>
        /// <param name="to">A stream to write to.</param>
        /// <param name="count">A number of bytes to copy.</param>
        /// <returns></returns>
        public static long Copy(Stream from, Stream to, long count)
        {
            if (count <= 0)
                throw new ArgumentException(
                    $"The specified number of bytes to read is invalid ({count}).",
                    nameof(count));
            long bytesLeft = count;
            int bufferSize = (int)Math.Min(count, MaximumBufferSize), bytesRead;
            byte[] buffer = new byte[bufferSize];
            do
            {
                bytesRead = from.Read(buffer, 0, bufferSize);
                to.Write(buffer, 0, bytesRead);
                bytesLeft -= bytesRead;
            } while (bytesRead == bufferSize && bytesLeft != 0);
            return count - bytesLeft;
        }

        /// <summary>
        /// Copies the specified number of bytes from a source stream to a target stream using the existing buffer.
        /// <para>This method is useful for stream with a length greater than <see cref="Int32.MaxValue"/>.</para>
        /// </summary>
        /// <param name="from">A stream to read from.</param>
        /// <param name="to">A stream to write to.</param>
        /// <param name="count">A number of bytes to copy.</param>
        /// <param name="buffer">A buffer to use for reading / writing. 
        /// <para>If the specified reference is null or the array behind it is not large enough 
        /// then a new buffer will be created on this reference.</para></param>
        /// <returns></returns>
        public static long Copy(Stream from, Stream to, long count, ref byte[] buffer)
        {
            if (count <= 0)
                throw new ArgumentException(
                    $"The specified number of bytes to read is invalid ({count}).",
                    nameof(count));
            long bytesLeft = count;
            int bufferSize = (int)Math.Min(count, MaximumBufferSize), bytesRead;
            if (buffer == null || buffer.Length < bufferSize) buffer = new byte[bufferSize];
            do
            {
                bytesRead = from.Read(buffer, 0, bufferSize);
                to.Write(buffer, 0, bytesRead);
                bytesLeft -= bytesRead;
            } while (bytesRead == bufferSize && bytesLeft != 0);
            return count - bytesLeft;
        }

        /// <summary>
        /// Copies the specified number of bytes from a memory to a target stream.
        /// </summary>
        /// <param name="from">A stream to read from.</param>
        /// <param name="to">A stream to write to.</param>
        /// <param name="count">A number of bytes to copy.</param>
        /// <returns></returns>
        public static int CopyTo(this ReadOnlyMemory<byte> from, Stream to)
        {
            int count = from.Length, idx = 0;
            int bufferSize = (int)Math.Min(count, MaximumBufferSize), bytesToRead;
            byte[] buffer = new byte[bufferSize];
            while (idx != count)
            {
                bytesToRead = Math.Min(bufferSize, count - idx);
                from.Slice(idx, bytesToRead).CopyTo(buffer);
                to.Write(buffer, 0, bytesToRead);
                idx += bytesToRead;
            }
            return idx;
        }

        /// <summary>
        /// Converts the contents of the given memory stream to a region of read-only byte memory.
        /// </summary>
        /// <param name="source">The memory stream to convert.</param>
        /// <returns>A region of read-only byte memory containing the contents of the specified memory stream.</returns>
        public static ReadOnlyMemory<byte> ToMemory(this MemoryStream source)
        {
            return new ReadOnlyMemory<byte>(source.GetBuffer(), 0, (int)source.Length);
        }

        /// <summary>
        /// Converts the contents of the given memory stream to a read-only span of bytes.
        /// </summary>
        /// <param name="source">The memory stream to convert.</param>
        /// <returns>A read-only span of bytes containing the contents of the specified memory stream.</returns>
        public static ReadOnlySpan<byte> ToSpan(this MemoryStream source)
        {
            return new ReadOnlySpan<byte>(source.GetBuffer(), 0, (int)source.Length);
        }

        /// <summary>
        /// Converts an array of bytes to a string representing its content in the hexadecimal format.
        /// </summary>
        /// <param name="array">The array of bytes to format.</param>
        /// <returns>A string representing the content of the specified array in the hexadecimal format.</returns>
        public static string BytesToHex(byte[] array)
        {
            int length = array.Length;
            StringBuilder sb = new StringBuilder(length * 5);
            bool isFirst = true;
            foreach (byte b in array)
            {
                if (isFirst)
                    isFirst = false;
                else
                    sb.Append(",");
                sb.AppendFormat("0x{0:X2}", b);
            }
            return sb.ToString();
        }

        /// <summary>
        /// Converts an array of bytes to a string representing its content in the decimal format.
        /// </summary>
        /// <param name="array">The array of bytes to format.</param>
        /// <returns>A string representing the content of the specified array.</returns>
        public static string BytesToCode(byte[] array)
        {
            int length = array.Length;
            StringBuilder sb = new StringBuilder(length * 5);
            sb.Append('{');
            --length;
            for (int i = 0; i != length; ++i)
            {
                sb.Append(array[i]);
                sb.Append(", ");
            }
            sb.Append(array[length]);
            sb.Append('}');
            return sb.ToString();
        }

        /// <summary>
        /// Gets the deterministic hash code for the specified string value.
        /// <para>The deterministic result is independent of the processor architecture of the executing machine.</para>
        /// </summary>
        /// <param name="value">The string value to calculate the hash code for.</param>
        /// <returns>The calculated deterministic hash code for the input string.</returns>
        public static int GetDeterministicHashCode(this string value)
        {
            unchecked
            {
                int hash1 = (2386 << 16) + 5690;
                int hash2 = hash1;

                for (int i = 0; i != value.Length; i += 2)
                {
                    hash1 = ((hash1 << 5) + hash1) ^ value[i];
                    if (i == value.Length - 1)
                        break;
                    hash2 = ((hash2 << 5) + hash2) ^ value[i + 1];
                }

                return hash1 + (hash2 * 92456655);
            }
        }

        /// <summary>
        /// Gets the deterministic hash code for the specified GUID value.
        /// <para>The deterministic result is independent of the processor architecture of the executing machine.</para>
        /// </summary>
        /// <param name="value">The GUID value to calculate the hash code for.</param>
        /// <returns>The calculated deterministic hash code for the input GUID value.</returns>
        public static int GetDeterministicHashCode(this Guid value)
        {
            unchecked
            {
                byte[] data = value.ToByteArray();

                int hash1 = (9845 << 16) + 2143;
                int hash2 = hash1;

                for (int i = 0; i != 16; i += 2)
                {
                    hash1 = ((hash1 << 5) + hash1) ^ data[i];
                    hash2 = ((hash2 << 5) + hash2) ^ data[i + 1];
                }

                return hash1 + (hash2 * 823457);
            }
        }

        /// <summary>
        /// Executes the specified action repeatedly until it succeeds or the timeout is reached.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        /// <param name="msTimeout">The timeout in milliseconds to wait for the action to succeed.</param>
        /// <param name="retryInterval">The interval between each retry attempt (in milliseconds).</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public static async Task ExecuteUntilSucceededAsync(Action action, int msTimeout, int retryInterval = 500)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            if (msTimeout < 10) msTimeout = 10;
            retryInterval = MathUtility.Clamp(retryInterval, 10, msTimeout);
            Exception error;
            Stopwatch sw = Stopwatch.StartNew();
            do
            {
                try
                {
                    action();
                    error = null;
                }
                catch (Exception exc)
                {
                    if (sw.ElapsedMilliseconds > msTimeout) throw exc;
                    error = exc;
                    await Task.Delay(Math.Min(retryInterval, msTimeout - (int)sw.ElapsedMilliseconds)).ConfigureAwait(false);
                }
            } while (error != null);
        }

        /// <summary>
        /// Executes the specified action repeatedly until it succeeds or the cancellation is requested.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        /// <param name="ct">The cancellaton token for the operation.</param>
        /// <param name="retryInterval">The interval between each retry attempt (in milliseconds).</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public static async Task ExecuteUntilSucceededAsync(Action action, CancellationToken ct, int retryInterval = 500)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            retryInterval = MathUtility.Clamp(retryInterval, 10, 15000);
            Exception error;
            do
            {
                try
                {
                    action();
                    error = null;
                }
                catch (Exception exc)
                {
                    error = exc;
                    if (await ct.WaitHandle.WaitOneAsync(retryInterval).ConfigureAwait(false)) throw exc;
                }
            } while (error != null);
        }

        /// <summary>
        /// Retrieves the name of the speified generic type.
        /// </summary>
        /// <param name="type">The generic type to retrieve the name for.</param>
        /// <returns>A string representing the name of the specified generic type.</returns>
        public static string GetGenericTypeName(Type type)
        {
            if (type.IsGenericType)
            {
                string genericArguments = type.GetGenericArguments()
                    .Select(x => x.Name)
                    .Aggregate((x1, x2) => $"{x1}, {x2}");
                return $"{type.Name.Substring(0, type.Name.IndexOf("`"))}<" + genericArguments + ">";
            }
            else
            {
                return type.Name;
            }
        }

        /// <summary>
        /// Retrieves a list of individual values from the specified aggregate value of a flagged enumeration.
        /// </summary>
        /// <typeparam name="TEnum">The enumeration type with <see cref="FlagsAttribute"/>.</typeparam>
        /// <param name="aggregateValue">The aggregate enumeration value to extract individual values from.</param>
        /// <returns>A list of enumeration values present in the specified aggregate value.</returns>
        public static List<TEnum> GetEnumFlags<TEnum>(TEnum aggregateValue) where TEnum : struct, Enum
        {
            TEnum[] allValues;
#if NET_7_0
            allValues = Enum.GetValues<TEnum>();
#else
            allValues = (TEnum[])Enum.GetValues(typeof(TEnum));
#endif

            List<TEnum> result = new List<TEnum>(allValues.Length);
            foreach (TEnum val in allValues)
            {
                if (aggregateValue.HasFlag(val))
                {
                    result.Add(val);
                }
            }

            return result;
        }

        #endregion
    }
}
