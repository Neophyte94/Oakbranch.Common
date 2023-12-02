using System.Collections.Generic;

namespace Oakbranch.Common.Collections
{
    /// <summary>
    /// Represents an abstraction for an ordered collection of elements that allows tracking changes to it.
    /// </summary>
    /// <typeparam name="T">The type of the elements.</typeparam>
    public interface IObservableList<T> : 
        IReadOnlyObservableList<T>,
        IObservableCollection<T>, 
        IList<T>
    {
        // The property is declared new to remove ambiguity between two base interfaces when accessing Count.
        new int Count { get; }
    }
}