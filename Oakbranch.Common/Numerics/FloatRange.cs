using System;
using Oakbranch.Common.Utility;

namespace Oakbranch.Common.Numerics
{
    /// <summary>
    /// A structure representing a range of float floating point numbers.
    /// </summary>
    public readonly struct FloatRange
    {
        #region Static members

        /// <summary>
        /// Gets the instance of <see cref="FloatRange"/> representing an undefined range.
        /// </summary>
        public static FloatRange Undefined => new FloatRange(float.NaN, float.NaN);

        /// <summary>
        /// Gets the instance of <see cref="FloatRange"/> representing an unbounded range.
        /// </summary>
        public static FloatRange Infinite => new FloatRange(float.NegativeInfinity, float.PositiveInfinity);

        #endregion

        #region Instance members

        /// <summary>
        /// The lower inclusive bound of a range.
        /// </summary>
        public readonly float Floor;

        /// <summary>
        /// The upper inclusive bound of a range.
        /// </summary>
        public readonly float Ceil;

        /// <summary>
        /// Gets the difference between <see cref="Ceil"/> and <see cref="Floor"/>. 
        /// </summary>
        public float Span => Ceil - Floor;

        /// <summary>
        /// Gets a boolean value indicating whether both bounds of the range are finite numbers.
        /// </summary>
        public bool IsFinite => float.IsFinite(Floor) && float.IsFinite(Ceil);

        /// <summary>
        /// Gets a boolean value indicating whether both bounds of the range are normal numbers.
        /// <para>A number is considered normal if it's finite and not zero.</para>
        /// </summary>
        public bool IsNormal => float.IsNormal(Floor) && float.IsNormal(Ceil);

        /// <summary>
        /// Gets a boolean value indicating whether the range's span is approximately zero.
        /// </summary>
        public bool IsEmpty => Span.ApprZero();

        #endregion

        #region Instance constructors

        /// <summary>Initializes a new instance of the <see cref="FloatRange"/> structure 
        /// with the specified <see cref="Floor"/> and <see cref="Ceil"/> values.</summary>
        /// <param name="floor">The lower inclusive bound of a range.</param>
        /// <param name="ceil">The upper inclusive bound of a range.</param>
        public FloatRange(float floor, float ceil)
        {
            if (float.IsNaN(floor))
                throw new ArgumentException("A floor value must be a number.");
            if (float.IsNaN(ceil))
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
            if (other is FloatRange range2)
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

        public static bool operator ==(FloatRange x, FloatRange y)
        {
            return MathUtility.ApprEqual(in x.Floor, in y.Floor) && MathUtility.ApprEqual(in x.Ceil, in y.Ceil);
        }

        public static bool operator !=(FloatRange x, FloatRange y)
        {
            return !(x == y);
        }

        #endregion
    }
}
