using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Oakbranch.Common.Collections
{
    /// <summary>
    /// Represents a read-only collection of elements that supports tracking changes to it.
    /// </summary>
    /// <typeparam name="T">The type of the elements.</typeparam>
    public interface IReadOnlyObservableCollection<T> :
        INotifyCollectionChanged,
        INotifyPropertyChanged,
        IReadOnlyCollection<T>
    { }
}