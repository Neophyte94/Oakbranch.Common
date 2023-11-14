using System;
using System.Security.Cryptography;

namespace Oakbranch.Common.Utility
{
    public static class RSAParametersExtensions
    {
        public static void Clear(this RSAParameters parameters)
        {
            void EnsureClean(byte[] array)
            {
                if (array != null)
                {
                    Array.Clear(array, 0, array.Length);
                }
            }

            EnsureClean(parameters.D);
            parameters.D = null;

            EnsureClean(parameters.DP);
            parameters.DP = null;

            EnsureClean(parameters.DQ);
            parameters.DQ = null;

            EnsureClean(parameters.Exponent);
            parameters.Exponent = null;

            EnsureClean(parameters.InverseQ);
            parameters.InverseQ = null;

            EnsureClean(parameters.Modulus);
            parameters.Modulus = null;

            EnsureClean(parameters.P);
            parameters.P = null;

            EnsureClean(parameters.Q);
            parameters.Q = null;
        }

        public static RSAParameters Clone(this RSAParameters source)
        {
            RSAParameters target = new RSAParameters
            {
                Modulus = source.Modulus.CreateCopy(),
                Exponent = source.Exponent.CreateCopy()
            };

            if (source.P != null)
            {
                target.P = source.P.CreateCopy();
                target.Q = source.Q.CreateCopy();
                target.D = source.D.CreateCopy();
                target.DP = source.DP.CreateCopy();
                target.DQ = source.DQ.CreateCopy();
                target.InverseQ = source.InverseQ.CreateCopy();
            }

            return target;
        }
    }
}
