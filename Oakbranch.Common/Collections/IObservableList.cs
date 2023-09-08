using System;
using System.Collections.Generic;

namespace Oakbranch.Common.Collections
{
    public interface IObservableList<T> : 
        IObservableCollection<T>, 
        IList<T>, 
        IReadOnlyList<T>
    {
        new int Count { get; }
    }
}
