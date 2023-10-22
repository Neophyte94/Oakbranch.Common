using System;
using System.Collections.Generic;
using System.Globalization;
using Oakbranch.Common.Numerics;

namespace Oakbranch.Common.Utility
{
    /// <summary>
    /// A helper class that provides a set of different mathematical methods.
    /// </summary>
    public static class MathUtility
    {
        /// <summary>
        /// Defines the multiplier that converts degress to radians.
        /// </summary>
        public const double DegToRadMultiplier = Math.PI / 180.0;
        /// <summary>
        /// Defines the multiplier that converts radians to degrees.
        /// </summary>
        public const double RadToDegMultiplier = 180.0 / Math.PI;
        /// <summary>
        /// Defines the multiplier that converts seconds to minutes.
        /// </summary>
        public const float SecToMinsMultiplier = (float)(1.0 / 60.0);
        /// <summary>
        /// Defines the multiplier that converts milliseconds to minutes.
        /// </summary>
        public const float MsToMinsMultiplier = (float)(1.0 / 60000.0);
        /// <summary>
        /// Represents the value of the inverted normal distribution function with the standard parameters for the probability 0.999.
        /// </summary>
        public const float InvStdNormDistribution0999 = 3.090232306f;
        /// <summary>
        /// Represents the value of the inverted normal distribution function with the standard parameters for the probability 0.995.
        /// </summary>
        public const float InvStdNormDistribution0995 = 2.575829304f;
        /// <summary>
        /// Represents the value of the inverted normal distribution function with the standard parameters for the probability 0.990.
        /// </summary>
        public const float InvStdNormDistribution0990 = 2.326347874f;
        /// <summary>
        /// Represents the value of the inverted normal distribution function with the standard parameters for the probability 0.980.
        /// </summary>
        public const float InvStdNormDistribution0980 = 2.053748911f;
        /// <summary>
        /// Represents the value of the inverted normal distribution function with the standard parameters for the probability 0.950.
        /// </summary>
        public const float InvStdNormDistribution0950 = 1.644853627f;

        /// <summary>
        /// Determines whether two specified nullable decimals are equal.
        /// </summary>
        /// <returns><see langword="true"/> if both values are either not null and equal, or if both values are null. Otherwise <see langword="false"/>.</returns>
        public static bool Equal(in decimal? a, in decimal? b)
        {
            if (a != null)
            {
                if (b != null)
                {
                    return decimal.Equals(a.Value, b.Value);
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return b == null;
            }
        }

        /// <summary>
        /// Determines whether <paramref name="a"/> is approximately equal to <paramref name="b"/>, and returns the result.
        /// </summary>
        /// <returns><see langword="true"/> if <paramref name="a"/> is approximately equal to <paramref name="b"/>, otherwise <see langword="false"/>.</returns>
        public static bool ApprEqual(in float a, in float b)
        {
            return Math.Abs(a - b) < 0.0000001f;
        }

        /// <summary>
        /// Determines whether both <paramref name="a"/> and <paramref name="b"/> are null 
        /// or their values an approximately equal, and returns the result.
        /// </summary>
        /// <returns><see langword="true"/> if both <paramref name="a"/> and <paramref name="b"/> are null 
        /// or their values are approximately equal, otherwise <see langword="false"/>.</returns>
        public static bool ApprEqual(in float? a, in float? b)
        {
            if (a != null && b != null) return Math.Abs(a.Value - b.Value) < 0.0000001f;
            else return a == b;
        }

        /// <summary>
        /// Determines whether <paramref name="a"/> is approximately equal to <paramref name="b"/>, and returns the result.
        /// </summary>
        /// <returns><see langword="true"/> if <paramref name="a"/> is approximately equal to <paramref name="b"/>, otherwise <see langword="false"/>.</returns>
        public static bool ApprEqual(in double a, in double b)
        {
            return Math.Abs(a - b) < 0.00000000000001d;
        }

        /// <summary>
        /// Determines whether both <paramref name="a"/> and <paramref name="b"/> are null 
        /// or their values an approximately equal, and returns the result.
        /// </summary>
        /// <returns><see langword="true"/> if both <paramref name="a"/> and <paramref name="b"/> are null 
        /// or their values are approximately equal, otherwise <see langword="false"/>.</returns>
        public static bool ApprEqual(in double? a, in double? b)
        {
            if (a != null && b != null) return Math.Abs(a.Value - b.Value) < 0.00000000000001d;
            else return a == b;
        }

        /// <summary>
        /// Determines whether <paramref name="a"/> is greater than or approximately equal to <paramref name="b"/>, 
        /// and returns the result.
        /// </summary>
        /// <returns><see langword="true"/> if <paramref name="a"/> is greater than or approximately equal 
        /// to <paramref name="b"/>, otherwise <see langword="false"/>.</returns>
        public static bool ApprGreaterOrEqual(in float a, in float b)
        {
            return b - a < 0.0000001f;
        }

        /// <summary>
        /// Determines whether <paramref name="a"/> is greater than or approximately equal to <paramref name="b"/>, 
        /// and returns the result.
        /// </summary>
        /// <returns><see langword="true"/> if <paramref name="a"/> is greater than or approximately equal 
        /// to <paramref name="b"/>, otherwise <see langword="false"/>.</returns>
        public static bool ApprGreaterOrEqual(in double a, in double b)
        {
            return b - a < 0.00000000000001d;
        }

        /// <summary>
        /// Returns the biggest of the three specified numbers.
        /// </summary>
        public static float Max(in float a, in float b, in float c)
        {
            if (a < b)
            {
                return b < c ? c : b;
            }
            else if (a < c)
            {
                return c;
            }
            else
            {
                return a;
            }
        }

        /// <summary>
        /// Returns the biggest of the specified numbers.
        /// </summary>
        public static float Max(params float[] values)
        {
            float biggest = values[0];
            for (int i = 1; i != values.Length; ++i)
            {
                if (values[i] > biggest)
                    biggest = values[i];
            }
            return biggest;
        }

        /// <summary>
        /// Returns the biggest of the three specified numbers.
        /// </summary>
        public static double Max(in double a, in double b, in double c)
        {
            if (a < b)
            {
                return b < c ? c : b;
            }
            else if (a < c)
            {
                return c;
            }
            else
            {
                return a;
            }
        }

        /// <summary>
        /// Returns the biggest of the specified numbers.
        /// </summary>
        public static double Max(params double[] values)
        {
            double biggest = values[0];
            for (int i = 1; i != values.Length; ++i)
            {
                if (values[i] > biggest)
                    biggest = values[i];
            }
            return biggest;
        }

        /// <summary>
        /// Returns the biggest of the three specified numbers.
        /// </summary>
        public static decimal Max(in decimal a, in decimal b, in decimal c)
        {
            if (a < b)
            {
                return b < c ? c : b;
            }
            else if (a < c)
            {
                return c;
            }
            else
            {
                return a;
            }
        }

        /// <summary>
        /// Returns the biggest of the specified numbers.
        /// </summary>
        public static decimal Max(params decimal[] values)
        {
            decimal biggest = values[0];
            for (int i = 1; i != values.Length; ++i)
            {
                if (values[i] > biggest)
                    biggest = values[i];
            }
            return biggest;
        }

        /// <summary>
        /// Returns the greater of two specified timespans.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static TimeSpan Max(in TimeSpan a, in TimeSpan b)
        {
            if (b > a)
                return b;
            else
                return a;
        }

        /// <summary>
        /// Returns the greater of two specified date &amp; time values.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static DateTime Max(in DateTime a, in DateTime b)
        {
            if (b > a)
                return b;
            else
                return a;
        }

        /// <summary>
        /// Returns the biggest of the three specified numbers.
        /// </summary>
        public static int Max(in int a, in int b, in int c)
        {
            if (a < b)
            {
                return b < c ? c : b;
            }
            else if (a < c)
            {
                return c;
            }
            else
            {
                return a;
            }
        }

        /// <summary>
        /// Returns the biggest of the specified numbers.
        /// </summary>
        public static int Max(params int[] values)
        {
            int biggest = values[0];
            for (int i = 1; i != values.Length; ++i)
            {
                if (values[i] > biggest)
                    biggest = values[i];
            }
            return biggest;
        }

        /// <summary>
        /// Returns the smallest of the three specified numbers.
        /// </summary>
        public static double Min(in double a, in double b, in double c)
        {
            if (a < b)
            {
                return a < c ? a : c;
            }
            else if (b < c)
            {
                return b;
            }
            else
            {
                return c;
            }
        }

        /// <summary>
        /// Returns the smallest of the specified numbers.
        /// </summary>
        public static double Min(params double[] values)
        {
            double smallest = values[0];
            for (int i = 1; i != values.Length; ++i)
            {
                if (values[i] < smallest)
                    smallest = values[i];
            }
            return smallest;
        }

        /// <summary>
        /// Returns the smallest of the three specified numbers.
        /// </summary>
        public static decimal Min(in decimal a, in decimal b, in decimal c)
        {
            if (a < b)
            {
                return a < c ? a : c;
            }
            else if (b < c)
            {
                return b;
            }
            else
            {
                return c;
            }
        }

        /// <summary>
        /// Returns the smallest of the specified numbers.
        /// </summary>
        public static decimal Min(params decimal[] values)
        {
            decimal smallest = values[0];
            for (int i = 1; i != values.Length; ++i)
            {
                if (values[i] < smallest)
                    smallest = values[i];
            }
            return smallest;
        }

        /// <summary>
        /// Returns the smaller of two specified timespans.
        /// </summary>
        public static TimeSpan Min(in TimeSpan a, in TimeSpan b)
        {
            if (b < a)
                return b;
            else
                return a;
        }

        /// <summary>
        /// Returns the earlier of two specified date &amp; time values.
        /// </summary>
        public static DateTime Min(in DateTime a, in DateTime b)
        {
            if (b < a)
                return b;
            else
                return a;
        }

        /// <summary>
        /// Returns the value linearly interpolated between A and B by the specified percent.
        /// </summary>
        public static float Lerp(in float a, in float b, in float percent)
        {
            const float maxThreshold = 1.0f - float.Epsilon;
            if (percent < float.Epsilon)
            {
                return a;
            }
            else if (percent > maxThreshold)
            {
                return b;
            }
            else
            {
                return a + (b - a) * percent;
            }
        }

        /// <summary>
        /// Returns the value linearly interpolated between A and B by the specified percent.
        /// </summary>
        public static double Lerp(in double a, in double b, in double percent)
        {
            const double maxThreshold = 1.0 - double.Epsilon;
            if (percent < double.Epsilon)
            {
                return a;
            }
            else if (percent > maxThreshold)
            {
                return b;
            }
            else
            {
                return a + (b - a) * percent;
            }
        }

        /// <summary>
        /// Returns the value linearly interpolated between A and B by the specified percent.
        /// </summary>
        [Obsolete("Use the method Lerp() instead.")]
        public static float LinearlyInterpolate(in float a, in float b, in float percent) => Lerp(in a, in b, in percent);

        /// <summary>
        /// Returns the value linearly interpolated between A and B by the specified percent.
        /// </summary>
        [Obsolete("Use the method Lerp() instead.")]
        public static double LinearlyInterpolate(in double a, in double b, in double percent) => Lerp(in a, in b, in percent);

        /// <summary>
        /// Clamps the given value between the given minimum and maximum values. 
        /// <para>
        /// Returns the given value if it is within the min and max range,
        /// or the appropriate bound if the former falls outside the latter.</para>
        /// </summary>
        /// <param name="value"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static float Clamp(in float value, in float min, in float max)
        {
            if (value < min) return min;
            else if (value > max) return max;
            return value;
        }

        /// <summary>
        /// Clamps the given value between the given minimum and maximum values. 
        /// <para>
        /// Returns the given value if it is within the min and max range,
        /// or the appropriate bound if the former falls outside the latter.</para>
        /// </summary>
        /// <param name="value"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static double Clamp(in double value, in double min, in double max)
        {
            if (value < min) return min;
            else if (value > max) return max;
            return value;
        }

        /// <summary>
        /// Clamps the given value between the given minimum and maximum values. 
        /// <para>
        /// Returns the given value if it is within the min and max range,
        /// or the appropriate bound if the former falls outside the latter.</para>
        /// </summary>
        /// <param name="value"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static decimal Clamp(in decimal value, in decimal min, in decimal max)
        {
            if (value < min) return min;
            else if (value > max) return max;
            return value;
        }

        /// <summary>
        /// Clamps the given value between the given minimum and maximum values. 
        /// <para>
        /// Returns the given value if it is within the min and max range,
        /// or the appropriate bound if the former falls outside the latter.</para>
        /// </summary>
        /// <param name="value"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static int Clamp(in int value, in int min, in int max)
        {
            if (value < min) return min;
            else if (value > max) return max;
            return value;
        }

        /// <summary>
        /// Clamps the given value between the given minimum and maximum values. 
        /// <para>
        /// Returns the given value if it is within the min and max range,
        /// or the appropriate bound if the former falls outside the latter.</para>
        /// </summary>
        /// <param name="value"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static long Clamp(in long value, in long min, in long max)
        {
            if (value < min) return min;
            else if (value > max) return max;
            return value;
        }

        /// <summary>
        /// Returns a boolean indicating whether the specified value is within the specified bounds (inclusive).
        /// </summary>
        /// <returns></returns>
        public static bool IsWithinRange(in float value, in float min, in float max)
        {
            return value - min > -0.0000001f && value - max < 0.0000001f;
        }

        /// <summary>
        /// Returns a boolean indicating whether the specified value is within the specified bounds (inclusive).
        /// </summary>
        /// <returns></returns>
        public static bool IsWithinRange(in double value, in double min, in double max)
        {
            return value - min > -0.0000001f && value - max < 0.0000001f;
        }

        /// <summary>
        /// Returns a boolean indicating whether the specified value is within the specified bounds (inclusive).
        /// </summary>
        /// <returns></returns>
        public static bool IsWithinRange(in double value, in DoubleRange range)
        {
            return value - range.Floor > -0.0000001f && value - range.Ceil < 0.0000001f;
        }

        /// <summary>
        /// Returns the smallest integer that is greater or equal to the specified numeric value.
        /// <para>For positive values it truncates towards positive infinity, for negative ones - towards zero.</para>
        /// </summary>
        /// <param name="value">A value to ceil.</param>
        /// <returns>The smallest integer that is greater or equal to the specified value.</returns>
        public static int CeilToInt(in float value)
        {
            if (value > -0.0000001f)
            {
                int iVal = (int)value;
                if (value - iVal > 0.0000001f) return ++iVal;
                else return iVal;
            }
            else
            {
                return (int)value;
            }
        }

        /// <summary>
        /// Returns the smallest integer that is greater or equal to the specified numeric value.
        /// <para>For positive values it truncates towards positive infinity, for negative ones - towards zero.</para>
        /// </summary>
        /// <param name="value">A value to ceil.</param>
        /// <returns>The smallest integer that is greater or equal to the specified value.</returns>
        public static int CeilToInt(in double value)
        {
            if (value > -0.00000000000001)
            {
                int iVal = (int)value;
                if (value - iVal > 0.00000000000001) return ++iVal;
                else return iVal;
            }
            else
            {
                return (int)value;
            }
        }

        /// <summary>
        /// Returns the smallest integer that is greater or equal to the quotient of the specified numbers.
        /// <para>For positive values it truncates towards positive infinity, for negative ones - towards zero.</para>
        /// </summary>
        /// <returns>The smallest integer that is greater or equal to the quotient of the specified numbers.</returns>
        public static int Ceil(in int dividend, in int divisor)
        {
            if (dividend % divisor == 0) 
                return dividend / divisor;
            else 
                return dividend / divisor + 1;
        }

        /// <summary>
        /// Returns the smallest integer that is greater or equal to the quotient of the specified numbers.
        /// <para>For positive values it truncates towards positive infinity, for negative ones - towards zero.</para>
        /// </summary>
        /// <returns>The smallest integer that is greater or equal to the quotient of the specified numbers.</returns>
        public static long Ceil(in long dividend, in long divisor)
        {
            if (dividend % divisor == 0)
                return dividend / divisor;
            else
                return dividend / divisor + 1;
        }

        /// <summary>
        /// Returns the greatest integer that is less or equal to the specified numeric value.
        /// <para>For positive values it truncates towards zero, for negative - towards negative infinity.</para>
        /// </summary>
        /// <param name="value">A value to floor.</param>
        /// <returns>The greatest integer that is less or equal to the specified value.</returns>
        public static int FloorToInt(in float value)
        {
            if (value < -0.0000001f)
            {
                int iVal = (int)value;
                if (value - iVal > 0.0000001f) return --iVal;
                else return iVal;
            }
            else
            {
                return (int)value;
            }
        }

        /// <summary>
        /// Returns the greatest integer that is less or equal to the specified numeric value.
        /// <para>For positive values it truncates towards zero, for negative - towards negative infinity.</para>
        /// </summary>
        /// <param name="value">A value to floor.</param>
        /// <returns>The greatest integer that is less or equal to the specified value.</returns>
        public static int FloorToInt(in double value)
        {
            if (value < -0.00000000000001)
            {
                int iVal = (int)value;
                if (value - iVal > 0.00000000000001) return --iVal;
                else return iVal;
            }
            else
            {
                return (int)value;
            }
        }

        /// <summary>
        /// Returns an integer nearest to the specified numeric value.
        /// </summary>
        /// <param name="value">A value to round.</param>
        /// <returns>The integer that is nearest to the specified numeric value.</returns>
        public static int RoundToInt(in float value)
        {
            int iVal = (int)value;
            if (value < -0.0000001f)
            {
                if (iVal - value > 0.4999999f) return --iVal;
                else return iVal;
            }
            else
            {
                if (value - iVal > 0.4999999f) return ++iVal;
                else return iVal;
            }
        }

        /// <summary>
        /// Returns an integer nearest to the specified numeric value.
        /// </summary>
        /// <param name="value">A value to round.</param>
        /// <returns>The integer that is nearest to the specified numeric value.</returns>
        public static int RoundToInt(in double value)
        {
            int iVal = (int)value;
            if (value < -0.00000000000001)
            {
                if (iVal - value > 0.49999999999999) return --iVal;
                else return iVal;
            }
            else
            {
                if (value - iVal > 0.49999999999999) return ++iVal;
                else return iVal;
            }
        }

        /// <summary>
        /// Returns a 64-bit integer nearest to the specified numeric value.
        /// </summary>
        /// <param name="value">A value to round.</param>
        /// <returns>The integer that is nearest to the specified numeric value.</returns>
        public static long RoundToInt64(in float value)
        {
            long iVal = (long)value;
            if (value < -0.0000001f)
            {
                if (iVal - value > 0.4999999f) return --iVal;
                else return iVal;
            }
            else
            {
                if (value - iVal > 0.4999999f) return ++iVal;
                else return iVal;
            }
        }

        /// <summary>
        /// Returns a 64-bit integer nearest to the specified numeric value.
        /// </summary>
        /// <param name="value">A value to round.</param>
        /// <returns>The integer that is nearest to the specified numeric value.</returns>
        public static int RoundToInt64(in double value)
        {
            int iVal = (int)value;
            if (value < -0.00000000000001)
            {
                if (iVal - value > 0.49999999999999) return --iVal;
                else return iVal;
            }
            else
            {
                if (value - iVal > 0.49999999999999) return ++iVal;
                else return iVal;
            }
        }

        // The variable NET_7_0 is set in the project settings.
#if !NET_7_0
        /// <summary>
        /// Returns true if the specified value presents neither NaN nor infinity.
        /// </summary>
        /// <param name="value">A value to check.</param>
        /// <returns>True if the specified value presents neither NaN nor infinity,
        /// false otherwise.</returns>
        public static bool IsReal(this in float value)
        {
            return !float.IsNaN(value) && !float.IsInfinity(value);
        }

        /// <summary>
        /// Returns true if the specified value presents neither NaN nor infinity.
        /// </summary>
        /// <param name="value">A value to check.</param>
        /// <returns>True if the specified value presents neither NaN nor infinity,
        /// false otherwise.</returns>
        public static bool IsReal(this in double value)
        {
            return !double.IsNaN(value) && !double.IsInfinity(value);
        }
#endif

        /// <summary>
        /// Gets a square of the specified value.
        /// </summary>
        public static double GetSquare(this in double value)
        {
            return value * value;
        }

        /// <summary>
        /// Gets a square of the specified value.
        /// </summary>
        public static float GetSquare(this in float value)
        {
            return value * value;
        }

        /// <summary>
        /// Gets a number of digits in the specified number.
        /// </summary>
        public static int GetDigitsNumber(this in short n)
        {
            if (n >= 0)
            {
                if (n < 10) return 1;
                else if (n < 100) return 2;
                else if (n < 1000) return 3;
                else if (n < 10000) return 4;
                else return 5;
            }
            else
            {
                if (n > -10) return 2;
                else if (n > -100) return 3;
                else if (n > -1000) return 4;
                else if (n > -10000) return 5;
                else return 6;
            }
        }

        /// <summary>
        /// Gets a number of digits in the specified number.
        /// </summary>
        public static int GetDigitsNumber(this in int n)
        {
            if (n >= 0)
            {
                if (n < 10) return 1;
                else if (n < 100) return 2;
                else if (n < 1000) return 3;
                else if (n < 10000) return 4;
                else if (n < 100000) return 5;
                else if (n < 1000000) return 6;
                else if (n < 10000000) return 7;
                else if (n < 100000000) return 8;
                else if (n < 1000000000) return 9;
                else return 10;
            }
            else
            {
                if (n > -10) return 2;
                else if (n > -100) return 3;
                else if (n > -1000) return 4;
                else if (n > -10000) return 5;
                else if (n > -100000) return 6;
                else if (n > -1000000) return 7;
                else if (n > -10000000) return 8;
                else if (n > -100000000) return 9;
                else if (n > -1000000000) return 10;
                else return 11;
            }
        }

        /// <summary>
        /// Gets a number of digits in the specified number.
        /// </summary>
        public static int GetDigitsNumber(this in long n)
        {
            if (n >= 0)
            {
                if (n < 10) return 1;
                else if (n < 100) return 2;
                else if (n < 1000) return 3;
                else if (n < 10000) return 4;
                else if (n < 100000) return 5;
                else if (n < 1000000) return 6;
                else if (n < 10000000) return 7;
                else if (n < 100000000) return 8;
                else if (n < 1000000000) return 9;
                else if (n < 10000000000) return 10;
                else if (n < 100000000000) return 11;
                else if (n < 1000000000000) return 12;
                else if (n < 10000000000000) return 13;
                else if (n < 100000000000000) return 14;
                else if (n < 1000000000000000) return 15;
                else if (n < 10000000000000000) return 16;
                else if (n < 100000000000000000) return 17;
                else if (n < 1000000000000000000) return 18;
                return 19;
            }
            else
            {
                if (n > -10) return 2;
                else if (n > -100) return 3;
                else if (n > -1000) return 4;
                else if (n > -10000) return 5;
                else if (n > -100000) return 6;
                else if (n > -1000000) return 7;
                else if (n > -10000000) return 8;
                else if (n > -100000000) return 9;
                else if (n > -1000000000) return 10;
                else if (n > -10000000000) return 11;
                else if (n > -100000000000) return 12;
                else if (n > -1000000000000) return 13;
                else if (n > -10000000000000) return 14;
                else if (n > -100000000000000) return 15;
                else if (n > -1000000000000000) return 16;
                else if (n > -10000000000000000) return 17;
                else if (n > -100000000000000000) return 18;
                else if (n > -1000000000000000000) return 19;
                else return 20;
            }
        }

        /// <summary>
        /// Gets a sum of all numbers in the specified array.
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public static double GetSum(this double[] values)
        {
            double sum = 0.0;
            for (int i = 0; i != values.Length;)
            {
                sum += values[i++];
            }
            return sum;
        }

        /// <summary>
        /// Gets a power of the specified base raised to the specified exponent.
        /// </summary>
        /// <param name="x">A base number.</param>
        /// <param name="y">An exponent.</param>
        public static int Power(int x, int y)
        {
            if (y == 0)
                return 1;
            else if (y < 0)
                throw new ArgumentOutOfRangeException(nameof(y));
            int power = x;
            while (--y != 0)
            {
                power *= x;
            }
            return power;
        }

        /// <summary>
        /// Gets a power of the specified base raised to the specified exponent.
        /// </summary>
        /// <param name="x">A base number.</param>
        /// <param name="y">An exponent.</param>
        public static long Power(long x, long y)
        {
            if (y == 0)
                return 1;
            else if (y < 0)
                throw new ArgumentOutOfRangeException(nameof(y));
            long power = x;
            while (--y != 0)
            {
                power *= x;
            }
            return power;
        }

        /// <summary>
        /// Gets a power of 10 raised to the specified exponent.
        /// </summary>
        /// <param name="y">An exponent.</param>
        public static int PowerOf10(int y)
        {
            if (y == 0)
                return 1;
            else if (y < 0)
                throw new ArgumentOutOfRangeException(nameof(y));
#if DEBUG
            else if (y > 9)
                throw new ArgumentOutOfRangeException(
                    nameof(y),
                    "An exponent cannot be greater than 9 for the Int32 overload of this method.");
#endif
            int power = 10;
            while (--y != 0)
            {
                power *= 10;
            }
            return power;
        }

        /// <summary>
        /// Gets a power of 10 raised to the specified exponent.
        /// </summary>
        /// <param name="y">An exponent.</param>
        public static long PowerOf10(long y)
        {
            if (y == 0)
                return 1;
            else if (y < 0)
                throw new ArgumentOutOfRangeException(nameof(y));
#if DEBUG
            else if (y > 18)
                throw new ArgumentOutOfRangeException(
                    nameof(y),
                    "An exponent cannot be greater than 18 for the Int64 overload of this method.");
#endif
            long power = 10;
            while (--y != 0)
            {
                power *= 10;
            }
            return power;
        }

        /// <summary>
        /// Gets a number of digits after the decimal point in the specified number.
        /// </summary>
        public static int GetPrecision(this in decimal value)
        {
            CultureInfo culture = CultureInfo.CurrentCulture;
            string s = value.ToString(culture);
            int idx = s.IndexOf(culture.NumberFormat.NumberDecimalSeparator);
            return idx == -1 ? 0 : s.Length - idx - 1;
        }

        /// <summary>
        /// Determines the smallest decimal number with the specified precision after the decimal point.
        /// <para>For example, if the precision is 1 the number returned is 0.1.</para>
        /// </summary>
        public static decimal GetEpsilon(int precision)
        {
            if (precision < 0)
                throw new ArgumentOutOfRangeException(nameof(precision));

            decimal val = 1.0M;
            while (precision-- != 0)
            {
                val *= 0.1M;
            }

            return val;
        }

        /// <summary>
        /// Returns true if the specified value approximately equals zero.
        /// </summary>
        public static bool ApprZero(this in float value)
        {
            return Math.Abs(value) < 0.0000001f;
        }

        /// <summary>
        /// Returns true if the specified value approximately equals zero.
        /// </summary>
        public static bool ApprZero(this in double value)
        {
            return Math.Abs(value) < 0.00000000000001d;
        }

        /// <summary>
        /// Gets a list containing all factors of the specified positive number.
        /// </summary>
        public static List<int> GetFactors(int number)
        {
            if (number < 1)
                throw new ArgumentOutOfRangeException(nameof(number));

            List<int> factors = new List<int>(Convert.ToInt32(10 * Math.Log10(number)));
            Stack<int> mirror = new Stack<int>(factors.Capacity / 2 + 1);
            int midway = (int)Math.Sqrt(number);

            for (int i = 1; i != midway; ++i)
            {
                if ((number % i) == 0)
                {
                    factors.Add(i);
                    mirror.Push(number / i);
                }
            }

            if (midway * midway == number)
            {
                factors.Add(midway);
            }
            else if ((number % midway) == 0)
            {
                factors.Add(midway);
                mirror.Push(number / midway);
            }

            factors.AddRange(mirror);
            return factors;
        }

        /// <summary>
        /// Determines whether the specified number is a prime number.
        /// <para>Throws <see cref="ArgumentOutOfRangeException"/> if the specified number is less than 1.</para>
        /// </summary>
        /// <returns>True if the specified number is prime, otherwise false.</returns>
        public static bool IsPrimeNumber(in int number)
        {
            if (number < 1)
                throw new ArgumentOutOfRangeException(nameof(number));

            if (number < 4) return true;

            for (int j = 2; j != number;)
            {
                if (number % j++ == 0) return false;
            }

            return true;
        }

        /// <summary>
        /// Gets a list of digits comprising the specified non-negative number (in the ascending order of digits rank).
        /// <para>Throws <see cref="ArgumentOutOfRangeException"/> if the specified number is less than 0.</para>
        /// </summary>
        /// <param name="number"></param>
        public static Queue<int> GetDigits(int number)
        {
            if (number < 0)
                throw new ArgumentOutOfRangeException(nameof(number));

            int digitExtractor = 1;
            Queue<int> digits = new Queue<int>(4);

            do
            {
                int rem = number % (10 * digitExtractor);
                digits.Enqueue(rem / digitExtractor);
                digitExtractor *= 10;
            } while (number >= digitExtractor);

            return digits;
        }

        /// <summary>
        /// Computes a recursive sum of digits comprising the specified non-negative number and returns it.
        /// <para>The result is a number in the range [0 ; 9].</para>
        /// <para>Throws <see cref="ArgumentOutOfRangeException"/> if the specified number is less than 0.</para>
        /// </summary>
        public static int GetRecursiveDigitsSum(int number)
        {
            if (number < 0)
                throw new ArgumentOutOfRangeException(nameof(number));
            
            int sum = number;

            while (sum > 9)
            {
                Queue<int> digits = GetDigits(sum);

                while (digits.Count > 1)
                {
                    sum = digits.Dequeue();
                    if (digits.Count != 0)
                        sum += digits.Dequeue();
                    digits.Enqueue(sum);
                }

                sum = digits.Dequeue();
            }

            return sum;
        }

        /// <summary>
        /// Estimates the upper bound of the confident range for the number of event occurrences
        /// given the specified single event probability, number of tests and standard deviation multiplier.
        /// <para>A final number of occurrences will be no greater than a value returned with 
        /// the probability STANDARD_NORMAL_FUNCTION(<paramref name="sigmaMultiplier"/>).</para>
        /// </summary>
        public static int EstimateReliantRangeCeil(
            in int numberOfTests, in float eventProbability,
            in float sigmaMultiplier = InvStdNormDistribution0999)
        {
            if (eventProbability < 0.0f || eventProbability > 1.0f)
                throw new ArgumentOutOfRangeException(nameof(eventProbability));
            if (numberOfTests < 0)
                throw new ArgumentOutOfRangeException(nameof(numberOfTests));
            if (sigmaMultiplier < 0.0f)
                throw new ArgumentOutOfRangeException(nameof(sigmaMultiplier));

            double statError = sigmaMultiplier * Math.Sqrt(eventProbability * (1.0f - eventProbability) * numberOfTests);
            return Math.Min(numberOfTests, CeilToInt(numberOfTests * eventProbability + statError));
        }

        /// <summary>
        /// Estimates the number of tests needed to get at least as much event occurrences 
        /// as the specified number with the probability STANDARD_NORMAL_FUNCTION(<paramref name="sigmaMultiplier"/>).
        /// </summary>
        /// <param name="occurrencesMin">A targeted minimum of the event occurrences number.</param>
        /// <param name="eventProbability">A probability of the event.</param>
        public static int EstimateReliantTestsCount(
            in int occurrencesMin, in float eventProbability,
            in float sigmaMultiplier = InvStdNormDistribution0999)
        {
            if (eventProbability < 0.0001f || eventProbability > 1.0f)
                throw new ArgumentOutOfRangeException(nameof(eventProbability));
            if (sigmaMultiplier < 0.0f)
                throw new ArgumentOutOfRangeException(nameof(sigmaMultiplier));

            if (occurrencesMin < 1)
                return 0;

            double sqrNormQ = sigmaMultiplier * sigmaMultiplier * (1.0 - eventProbability);
            return CeilToInt(
                (eventProbability * Math.Sqrt(sqrNormQ * (4.0 * occurrencesMin + sqrNormQ) / (eventProbability * eventProbability)) +
                 2.0 * occurrencesMin + sqrNormQ)
                / (2.0 * eventProbability));
        }

        /// <summary>
        /// Finds the roots of a quadratic polynomial equation of the form ax^2 + bx + c = 0.
        /// <para>Sets both result variables to the same value, if only one root exists.</para>
        /// <para>Sets both result variables to <see cref="double.NaN"/>, if the roots do not exist,
        /// or the input parameters are not valid numbers.</para>
        /// </summary>
        /// <param name="a">The coefficient of the quadratic term.</param>
        /// <param name="b">The coefficient of the linear term.</param>
        /// <param name="c">The constant term.</param>
        /// <param name="x1">
        /// Output parameter for the first root.
        /// <para>Set to <see cref="double.NaN"/> if the roots do not exist or cannot be found.</para>
        /// </param>
        /// <param name="x2">
        /// Output parameter for the second root.
        /// <para>Set to <see cref="double.NaN"/> if the roots do not exist or cannot be found.</para>
        /// </param>
        public static void FindPolynomialRoots(
            double a, double b, double c, out double x1, out double x2)
        {
            double discr = b * b - 4.0 * a * c;

            if (double.IsNaN(discr) || discr < 0.0)
            {
                x1 = x2 = double.NaN;
                return;
            }
            else if (double.IsPositiveInfinity(discr))
            {
                x1 = double.PositiveInfinity;
                x2 = double.NegativeInfinity;
                return;
            }

            a = 0.5 / a;
            b = -b * a;

            if (discr < 0.0000000000001)
            {
                x1 = x2 = b;
            }
            else
            {
                discr = Math.Sqrt(discr);
                x1 = b + a * discr;
                x2 = b - a * discr;
            }
        }

        /// <summary>
        /// Finds the roots of a cubic polynomial equation of the form ax^3 + bx^2 + cx + d = 0.
        /// <para>Sets one or two result variables to <see cref="double.NaN"/>, if the corresponding roots are complex numbers.</para>
        /// <para>Sets all the result variables to <see cref="double.NaN"/>, if the input parameters are not valid numbers.</para>
        /// </summary>
        /// <param name="a">The coefficient of the cubic term.</param>
        /// <param name="b">The coefficient of the quadratic term.</param>
        /// <param name="c">The coefficient of the linear term.</param>
        /// <param name="d">The constant term.</param>
        /// <param name="x1">
        /// Output parameter for the first root.
        /// <para>Set to <see cref="double.NaN"/> if the roots cannot be found.</para>
        /// </param>
        /// <param name="x2">
        /// Output parameter for the second root.
        /// <para>Set to <see cref="double.NaN"/> if two roots are complex numbers, or the roots cannot be found.</para>
        /// </param>
        /// <param name="x3">
        /// Output parameter for the third root.
        /// <para>Set to <see cref="double.NaN"/> if the third root is a complex number, or the roots cannot be found.</para>
        /// </param>
        public static void FindPolynomialRoots(
            double a, double b, double c, double d, out double x1, out double x2, out double x3)
        {
            // Find the roots of the cubic equation following the formula of Cardano.
            // Note that Jerolamo Cardano is NOT the author of this method! He was only the person who published it.
            // First, the original equation of the form a*x^3 + b*x^2 + c*x + d = 0 is replaced
            // with the canonic equation of the form y^3 + p*y + q = 0,
            // where x = y - (b / 3*a).

            // Calculate the parameters 'p' and 'q'.
            double p = (3.0 * a * c - b * b) / (3.0 * a * a);
            double q = (2.0 * b * b * b - 9.0 * a * b * c + 27.0 * a * a * d) / (27.0 * a * a * a);

            // Check whether the parameters are valid.
            if (double.IsNaN(p) || double.IsNaN(q))
            {
                x1 = x2 = x3 = double.NaN;
                return;
            }

            // Calculate the cubic discriminant 'Q'.
            double Q = p * p * p / 27.0 + q * q * 0.25;

            double y1, y2, y3;
            if (Q.ApprZero())
            {
                double temp = -0.5 * q;
#if NET_7_0
                temp = Math.Cbrt(temp);
#else
                temp = Math.Pow(temp, 0.3333333333333);
#endif

                y1 = 2.0 * temp;
                y2 = -temp;
                y3 = double.NaN;
            }
            else if (Q < 0.0)
            {
                // All the three roots exist.
                double phi;
                if (q.ApprZero())
                {
                    phi = Math.PI * 0.5;
                }
                else
                {
                    phi = Math.Atan(Math.Sqrt(-Q) / (-0.5 * q));
                    if (q > 0.0)
                    {
                        phi += Math.PI;
                    }
                }

                double temp = 2.0 * Math.Sqrt(-p * 0.3333333333333);
                y1 = temp * Math.Cos(phi * 0.3333333333333);
                y2 = temp * Math.Cos((phi + 2.0 * Math.PI) * 0.3333333333333);
                y3 = temp * Math.Cos((phi + 4.0 * Math.PI) * 0.3333333333333);
            }
            else
            {
                // Only one REAL root exist. The other two are complex numbers.
                double rootQ = Math.Sqrt(Q);

                // Calculate the parameters 'alpha' and 'beta'.
                double alpha = -0.5 * q + rootQ;
                double beta = -0.5 * q - rootQ;

#if NET_7_0
                alpha = Math.Cbrt(alpha);
                beta = Math.Cbrt(beta);
#else
                alpha = Math.Pow(alpha, 0.3333333333333);
                beta = Math.Pow(beta, 0.3333333333333);
#endif

                y1 = alpha + beta;
                y2 = double.NaN;
                y3 = double.NaN;
            }

            // Calculate the roots of the original equation.
            double add = -b / (3.0 * a);
            x1 = y1 + add;
            x2 = y2 + add;
            x3 = y3 + add;
        }
    }
}