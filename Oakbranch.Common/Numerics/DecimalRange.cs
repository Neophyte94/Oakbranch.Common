using System;

namespace Oakbranch.Common.Numerics
{
    /// <summary>
    /// A structure representing a range of decimal floating point numbers.
    /// </summary>
    public readonly struct DecimalRange
    {
        #region Instance props & fields

        /// <summary>
        /// The lower inclusive bound of a range.
        /// </summary>
        public readonly decimal Floor;

        /// <summary>
        /// The upper inclusive bound of a range.
        /// </summary>
        public readonly decimal Ceil;

        /// <summary>
        /// Gets a boolean value indicating whether both bounds of a range are real (non-infinite) numbers.
        /// </summary>
        public bool IsNotNull => Floor != 0.0M && Ceil != 0.0M;

        #endregion

        #region Instance constructors

        /// <summary>Initializes a new instance of the <see cref="DecimalRange"/> structure 
        /// with the specified <see cref="Floor"/> and <see cref="Ceil"/> values.</summary>
        /// <param name="floor">The lower inclusive bound of a range.</param>
        /// <param name="ceil">The upper inclusive bound of a range.</param>
        public DecimalRange(decimal floor, decimal ceil)
        {
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
            if (other is DecimalRange range2)
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

        public static bool operator ==(DecimalRange x, DecimalRange y)
        {
            return decimal.Equals(x.Floor, y.Floor) && decimal.Equals(x.Ceil, y.Ceil);
        }

        public static bool operator !=(DecimalRange x, DecimalRange y)
        {
            return !(x == y);
        }

        #endregion
    }
}
