using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace Oakbranch.Common.Collections
{
    /// <summary>
    /// Represents a light-weight immutable collection of elements that can be accessed by index, and can formally be observed.
    /// <para>Implements <see cref="IReadOnlyList{T}"/>, <see cref="INotifyCollectionChanged"/>, 
    /// <see cref="INotifyPropertyChanged"/> and <see cref="IDisposable"/>.</para>
    /// </summary>
    /// <typeparam name="T">The type of the elements.</typeparam>
    public class ReadOnlyObservableList<T> :
        ImmutableList<T>,
        IReadOnlyObservableList<T>,
        IDisposable
    {
        #region Instance props & fields

        private bool _isDisposed;

        #endregion

        #region Instance events

        public event NotifyCollectionChangedEventHandler CollectionChanged
        {
            add
            {
                ThrowIfDisposed();
                return;
            }
            remove
            {
                return;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged
        {
            add
            {
                ThrowIfDisposed();
                return;
            }
            remove
            {
                return;
            }
        }

        #endregion

        #region Instance constructors

        public ReadOnlyObservableList() : base() { }

        public ReadOnlyObservableList(T[] items) : base(items) { }

        public ReadOnlyObservableList(IEnumerable<T> items) : base(items) { }

        public ReadOnlyObservableList(IList<T> items) : base(items) { }

        #endregion

        #region Instance methods

        protected void ThrowIfDisposed()
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
        }

        public void Dispose()
        {
            if (!_isDisposed)
            {
                OnDisposing(true);
                _isDisposed = true;
                GC.SuppressFinalize(this);
            }
        }

        protected virtual void OnDisposing(bool releaseManaged) { }

        #endregion

        #region Destructor

        ~ReadOnlyObservableList()
        {
            if (!_isDisposed)
            {
                OnDisposing(false);
                _isDisposed = true;
            }
        }

        #endregion
    }
}
