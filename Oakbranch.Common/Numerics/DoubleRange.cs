using System;
using Oakbranch.Common.Utility;

namespace Oakbranch.Common.Numerics
{
    /// <summary>
    /// A structure representing a range of double floating point numbers.
    /// </summary>
    public readonly struct DoubleRange
    {
        #region Static members

        /// <summary>
        /// Gets the instance of <see cref="DoubleRange"/> representing an undefined range.
        /// </summary>
        public static DoubleRange Undefined => new DoubleRange(double.NaN, double.NaN);

        /// <summary>
        /// Gets the instance of <see cref="DoubleRange"/> representing an unbounded range.
        /// </summary>
        public static DoubleRange Infinite => new DoubleRange(double.NegativeInfinity, double.PositiveInfinity);

        #endregion

        #region Instance members

        /// <summary>
        /// The lower inclusive bound of a range.
        /// </summary>
        public readonly double Floor;

        /// <summary>
        /// The upper inclusive bound of a range.
        /// </summary>
        public readonly double Ceil;

        /// <summary>
        /// Gets the difference between <see cref="Ceil"/> and <see cref="Floor"/>. 
        /// </summary>
        public double Span => Ceil - Floor;

        /// <summary>
        /// Gets a boolean value indicating whether both bounds of the range are finite numbers.
        /// </summary>
        public bool IsFinite
        {
            get
            {
#if NET_7_0
                return double.IsFinite(Floor) && double.IsFinite(Ceil);
#else
                return MathUtility.IsReal(Floor) && MathUtility.IsReal(Ceil);
#endif
            }
            
        }

        /// <summary>
        /// Gets a boolean value indicating whether both bounds of the range are normal numbers.
        /// <para>A number is considered normal if it's finite and not zero.</para>
        /// </summary>
        public bool IsNormal
        {
            get
            {
#if NET_7_0
                return double.IsNormal(Floor) && double.IsNormal(Ceil);
#else
                return IsFinite && !MathUtility.ApprZero(Floor) && !MathUtility.ApprZero(Ceil);
#endif
            }

        }

        /// <summary>
        /// Gets a boolean value indicating whether the range's span is approximately zero.
        /// </summary>
        public bool IsEmpty => Span.ApprZero();

#endregion

        #region Instance constructors

        /// <summary>Initializes a new instance of the <see cref="DoubleRange"/> structure 
        /// with the specified <see cref="Floor"/> and <see cref="Ceil"/> values.</summary>
        /// <param name="floor">The lower inclusive bound of a range.</param>
        /// <param name="ceil">The upper inclusive bound of a range.</param>
        public DoubleRange(double floor, double ceil)
        {
            if (double.IsNaN(floor))
                throw new ArgumentException("A floor value must be a number.");
            if (double.IsNaN(ceil))
                throw new ArgumentException("A ceil value must be a number.");
            if (ceil < floor)
            {
                throw new ArgumentException(
                    $"The specified ceil of an integers range ({ceil}) is less than the floor ({floor}).");
            }

            Floor = floor;
            Ceil = ceil;
        }

        #endregion

        #region Instance methods

        public override bool Equals(object other)
        {
            if (other is DoubleRange range2)
            {
                return this == range2;
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return (unchecked(-458777235 * -1521134295) + Floor.GetHashCode()) * -1521134295 + Ceil.GetHashCode();
        }

        public override string ToString()
        {
            return $"[{Floor} ; {Ceil}]";
        }

        #endregion

        #region Operators

        public static bool operator ==(DoubleRange x, DoubleRange y)
        {
            return MathUtility.ApprEqual(in x.Floor, in y.Floor) && MathUtility.ApprEqual(in x.Ceil, in y.Ceil);
        }

        public static bool operator !=(DoubleRange x, DoubleRange y)
        {
            return !(x == y);
        }

        #endregion
    }
}
