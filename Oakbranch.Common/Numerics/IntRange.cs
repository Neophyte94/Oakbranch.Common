using System;

namespace Oakbranch.Common.Numerics
{
    /// <summary>
    /// A structure representing a range of integer numbers.
    /// </summary>
    public readonly struct IntRange
    {
        #region Instance props & fields

        /// <summary>
        /// The lower inclusive bound of a range.
        /// </summary>
        public readonly int Floor;

        /// <summary>
        /// The upper inclusive bound of a range.
        /// </summary>
        public readonly int Ceil;

        #endregion

        #region Instance constructors

        /// <summary>Initializes a new instance of the <see cref="IntRange"/> structure 
        /// with the specified <see cref="Floor"/> and <see cref="Ceil"/> values.</summary>
        /// <param name="floor">The lower inclusive bound of a range.</param>
        /// <param name="ceil">The upper inclusive bound of a range.</param>
        public IntRange(int floor, int ceil)
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
            if (other is IntRange range2)
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
            return Floor.GetHashCode() ^ Ceil.GetHashCode();
        }

        public static bool operator ==(IntRange x, IntRange y)
        {
            return x.Floor == y.Floor && x.Ceil == y.Ceil;
        }

        public static bool operator !=(IntRange x, IntRange y)
        {
            return x.Floor != y.Floor || x.Ceil != y.Ceil;
        }

        public override string ToString()
        {
            return $"[{Floor} ; {Ceil}]";
        }

        #endregion
    }
}
