using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Oakbranch.Common.Collections
{
    public class ReadOnlyObservableList<T> : ReadOnlyList<T>, INotifyCollectionChanged, INotifyPropertyChanged, IDisposable
    {
        public event NotifyCollectionChangedEventHandler CollectionChanged;
        public event PropertyChangedEventHandler PropertyChanged;

        public ReadOnlyObservableList() : base()
        {

        }

        public ReadOnlyObservableList(T[] items) : base(items)
        {

        }

        public ReadOnlyObservableList(IEnumerable<T> items) : base(items)
        {

        }

        public ReadOnlyObservableList(IList<T> items) : base(items)
        {

        }

        public void Dispose()
        {
            CollectionChanged = null;
            PropertyChanged = null;
        }
    }
}
