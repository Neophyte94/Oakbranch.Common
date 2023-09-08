using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Oakbranch.Common.Collections
{
    /// <summary>
    /// Represents an abstraction for a collection of elements that allows tracking changes to it.
    /// </summary>
    /// <typeparam name="T">The type of the elements.</typeparam>
    public interface IObservableCollection<T> : 
        ICollection<T>,
        IReadOnlyCollection<T>,
        INotifyPropertyChanged,
        INotifyCollectionChanged
    {
        // The property is declared new to remove ambiguity between two base interfaces when accessing Count.
        new int Count { get; }
    }
}
