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

        public static DoubleRange Undefined => new DoubleRange(double.NegativeInfinity, double.PositiveInfinity);

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
        /// Gets a boolean value indicating whether both bounds of a range are real (non-infinite) numbers.
        /// </summary>
        public bool IsDetermined
        {
            get
            {
                return !double.IsInfinity(Floor) && !double.IsInfinity(Ceil);
            }
        }

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

        public static bool operator ==(DoubleRange x, DoubleRange y)
        {
            return MathUtility.ApprEqual(in x.Floor, in y.Floor) && MathUtility.ApprEqual(in x.Ceil, in y.Ceil);
        }

        public static bool operator !=(DoubleRange x, DoubleRange y)
        {
            return !(x == y);
        }

        public override string ToString()
        {
            return $"[{Floor} ; {Ceil}]";
        }
    }
}
