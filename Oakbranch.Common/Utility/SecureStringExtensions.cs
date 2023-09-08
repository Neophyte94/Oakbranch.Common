using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;
using System.Text;

namespace Oakbranch.Common.Utility
{
    public static class SecureStringExtensions
    {
        public static bool AreEqual(SecureString a, SecureString b)
        {
            if (a == null)
            {
                return b == null;
            }
            if (b == null)
            {
                return false;
            }

            if (a.Length != b.Length)
            {
                return false;
            }

            IntPtr ss_bstr1_ptr = IntPtr.Zero;
            IntPtr ss_bstr2_ptr = IntPtr.Zero;

            try
            {
                ss_bstr1_ptr = Marshal.SecureStringToBSTR(a);
                ss_bstr2_ptr = Marshal.SecureStringToBSTR(b);

                String str1 = Marshal.PtrToStringBSTR(ss_bstr1_ptr);
                String str2 = Marshal.PtrToStringBSTR(ss_bstr2_ptr);

                return str1.Equals(str2);
            }
            finally
            {
                if (ss_bstr1_ptr != IntPtr.Zero)
                {
                    Marshal.ZeroFreeBSTR(ss_bstr1_ptr);
                }

                if (ss_bstr2_ptr != IntPtr.Zero)
                {
                    Marshal.ZeroFreeBSTR(ss_bstr2_ptr);
                }
            }
        }

        public static IEnumerable<short> GetEnumerator(this SecureString value)
        {
            IntPtr valuePtr = IntPtr.Zero;
            try
            {
                valuePtr = Marshal.SecureStringToGlobalAllocUnicode(value);
                for (int i = 0; i < value.Length; i++)
                {
                    yield return Marshal.ReadInt16(valuePtr, i * 2);
                }
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(valuePtr);
            }
        }

        // Gets the hash of the specified secure string.
        public static byte[] GetBytes(this SecureString value)
        {
            // Convert the input password into the byte array.
            char[] passwordContent = null;
            byte[] passwordBytes = null;
            try
            {
                passwordContent = new char[value.Length];
                int idx = 0;
                foreach (short a in value.GetEnumerator())
                {
                    passwordContent[idx++] = (char)a;
                }
                passwordBytes = Encoding.UTF8.GetBytes(passwordContent);
            }
            finally
            {
                if (passwordContent != null)
                {
                    Array.Clear(passwordContent, 0, passwordContent.Length);
                    passwordContent = new char[0];
                }
            }

            // Compute the hash sum of the byte array representing the input password.
            byte[] passwordHash = null;
            SHA256 hashGen = null;
            try
            {
                hashGen = SHA256.Create();
                passwordHash = hashGen.ComputeHash(passwordBytes);
            }
            finally
            {
                Array.Clear(passwordBytes, 0, passwordBytes.Length);
                passwordBytes = new byte[0];
                if (hashGen != null)
                {
                    hashGen.Clear();
                    hashGen.Dispose();
                }
            }

            return passwordHash;
        }
    }
}
