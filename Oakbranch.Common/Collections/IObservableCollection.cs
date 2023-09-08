using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Oakbranch.Common.Collections
{
    public interface IObservableCollection<T> : 
        ICollection<T>,
        IReadOnlyCollection<T>,
        INotifyPropertyChanged,
        INotifyCollectionChanged
    {
        new int Count { get; }
    }
}
