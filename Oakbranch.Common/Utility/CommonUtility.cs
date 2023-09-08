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
    public static class CommonUtility
    {
        private const int c_MaximumBufferSize = 65536;

        /// <summary>
        /// Returns true if both values reference the same array instance,
        /// or the arrays referenced by these values have the same content, 
        /// or both values are null.
        /// <para>Otherwise returns false.</para>
        /// </summary>
        /// <param name="a">The first reference to a byte array.</param>
        /// <param name="b">The second reference to a byte array.</param>
        /// <returns></returns>
        public static bool Equal(byte[] a, byte[] b)
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
        /// Returns true if both values reference the same collection instance,
        /// or the arrays referenced by these values have the same content, 
        /// or both values are null.
        /// <para>Otherwise returns false.</para>
        /// </summary>
        /// <param name="a">The first reference to a byte collection.</param>
        /// <param name="b">The second reference to a byte collection.</param>
        /// <returns></returns>
        public static bool Equal(IEnumerable<byte> a, IEnumerable<byte> b)
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

        /// <summary>
        /// Returns the age of a person with the specified birth date.
        /// </summary>
        /// <param name="birthDate">The birth date of a person.</param>
        /// <returns>The age of the person.</returns>
        public static int CalculateAge(DateTime birthDate)
        {
            DateTime now = DateTime.Now;
            int years = now.Year - birthDate.Year;

            if (now.Month < birthDate.Month ||
                (now.Month == birthDate.Month && now.Day < birthDate.Day))
                --years;

            return years;
        }

        public static void AddToMask(byte value, ref byte mask)
        {
            mask |= value;
        }

        public static void RemoveFromMask(byte value, ref byte mask)
        {
            mask &= (byte)(255 - value);
        }

        public static bool IsMaskMarked(byte value, byte mask)
        {
            return (mask & value) != 0;
        }

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

        public static T[] CreateCopy<T>(this T[] source, int startIndex, int length)
        {
            T[] result = new T[length];
            for (int i = 0; i != length;)
            {
                result[i++] = source[startIndex++];
            }
            return result;
        }

        public static T[] CreateHomogenous<T>(int length, T value)
        {
            T[] result = new T[length];
            for (int i = 0; i != length;)
            {
                result[i++] = value;
            }
            return result;
        }

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
        /// Returns the index of the first digit encountered in the passed string
        /// starting from the specified index.
        /// </summary>
        /// <param name="s">The string to search digits in.</param>
        /// <param name="startIdx">The index to start search from.</param>
        /// <returns>The index of the first digit encountered in the specified string.</returns>
        public static int IndexOfFirstDigit(string s, int startIdx)
        {
            int length = s.Length;
            for (int i = startIdx; i != length; ++i)
            {
                if (Char.IsDigit(s[i])) return i;
            }
            return -1;
        }

        public static void Clear<T>(ref T[] array)
        {
            if (array != null)
            {
                Array.Clear(array, 0, array.Length);
                array = null;
            }
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
            int bufferSize = (int)Math.Min(count, c_MaximumBufferSize), bytesRead;
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
            int bufferSize = (int)Math.Min(count, c_MaximumBufferSize), bytesRead;
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
            int bufferSize = (int)Math.Min(count, c_MaximumBufferSize), bytesToRead;
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

        public static ReadOnlyMemory<byte> ToMemory(this MemoryStream source)
        {
            return new ReadOnlyMemory<byte>(source.GetBuffer(), 0, (int)source.Length);
        }

        public static ReadOnlySpan<byte> ToSpan(this MemoryStream source)
        {
            return new ReadOnlySpan<byte>(source.GetBuffer(), 0, (int)source.Length);
        }

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

            return fi is object;
        }

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

            return di is object;
        }

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
    }
}
