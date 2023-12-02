using System;

namespace Oakbranch.Common.Collections
{
    [Obsolete("This class is obsolete. Use 'ImmutableList<T>' instead.")]
    /// <summary>
    /// Represents a light-weight immutable collection of elements that can be accessed by index.
    /// <para>Implements <see cref="IReadOnlyList{T}"/>.</para>
    /// </summary>
    /// <typeparam name="T">The type of the elements.</typeparam>
    public class ReadOnlyList<T> : ImmutableList<T>
    { }
}