using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Oakbranch.Common.Collections
{
    public interface IReadOnlyObservableDictionary<TKey, TValue> : 
        IReadOnlyDictionary<TKey, TValue>,
        INotifyPropertyChanged,
        INotifyCollectionChanged
    {

    }
}
