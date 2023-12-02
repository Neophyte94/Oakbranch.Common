using System.Collections.Generic;

namespace Oakbranch.Common.Collections
{

    /// <summary>
    /// Represents a read-only collection of elements that can be accessed by index,
    /// with a support for tracking changes to it.
    /// </summary>
    /// <typeparam name="T">The type of elements in the read-only list.</typeparam>
    public interface IReadOnlyObservableList<T> :
        IReadOnlyObservableCollection<T>,
        IReadOnlyList<T>
    { }
}