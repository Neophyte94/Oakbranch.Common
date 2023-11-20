using System;

namespace Oakbranch.Common.Numerics
{
    /// <summary>
    /// A structure representing a range of consequent integer indices.
    /// </summary>
    public readonly struct IndicesRange
    {
        #region Instance props & fields

        /// <summary>
        /// The first integer number in a range.
        /// </summary>
        /// <value>
        /// The first integer number in a range.
        /// </value>
        public readonly int Index;

        /// <summary>
        /// The length of a range.
        /// </summary>
        /// <value>
        /// The length of a range.
        /// </value>
        public readonly int Count;

        /// <summary>
        /// Returns a value indicating whether an <see cref="IndicesRange"/> instance has both <see cref="Index"/> and <see cref="Count"/> undefined.
        /// </summary>
        /// <seealso cref="Undefined">Undefined</seealso>
        public bool IsUndefined => Index == int.MinValue && Count == int.MinValue;

        /// <summary>
        /// Returns a value indicating whether an <see cref="IndicesRange"/> instance has only its <see cref="Index"/> defined.
        /// </summary>
        /// <seealso cref="CreateContainingOnlyStart_i">CreateContainingOnlyStart_i</seealso>
        public bool IsIndexOnlyDefined => Index != int.MinValue && Count == int.MinValue;

        #endregion

        #region Instance constructors

        /// <summary>Initializes a new instance of the <see cref="IndicesRange"/> structure 
        /// with the specified <see cref="Index"/> and <see cref="Count"/>.</summary>
        /// <param name="start_i">The Start_i of the DoubleRange</param>
        /// <param name="count">The Count of the DoubleRange</param>
        public IndicesRange(int index, int count)
        {
            Index = index;
            Count = count;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IndicesRange"/> structure 
        /// with specified <see cref="Index"/> and undefined <see cref="Count"/>.
        /// <para>An instance with the undefined <see cref="Count"/> can be distinguished with the <see cref="IsIndexOnlyDefined(IndicesRange)"/> method.</para>
        /// </summary>
        /// <seealso cref="IsIndexOnlyDefined"></seealso>
        public IndicesRange(int index)
        {
            Index = index;
            Count = int.MinValue;
        }

        #endregion

        #region Static methods

        /// <summary>
        /// Returns an <see cref="IndicesRange"/> instance which is denoted as Undefined.
        /// </summary>
        /// <seealso cref="IsUndefined">IsUndefined</seealso>
        public static IndicesRange Undefined => new IndicesRange(int.MinValue, int.MinValue);

        #endregion

        #region Instance methods

        public bool DoesEnclose(int index)
        {
            return index >= Index && index < Index + Count;
        }

        public override bool Equals(object other)
        {
            if (other is IndicesRange range2)
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
            return Index.GetHashCode() ^ Count.GetHashCode();
        }

        #endregion

        #region Operators

        public static bool operator ==(IndicesRange x, IndicesRange y)
        {
            return x.Index == y.Index && x.Count == y.Count;
        }

        public static bool operator !=(IndicesRange x, IndicesRange y)
        {
            return x.Index != y.Index || x.Count != y.Count;
        }

        #endregion
    }
}
