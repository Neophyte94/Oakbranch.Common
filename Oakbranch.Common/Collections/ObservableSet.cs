using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Oakbranch.Common.Collections
{
    /// <summary>
    /// Represents a set of unique elements that allows tracking changes to it.
    /// <para>Impements <see cref="IObservableCollection{T}"/> and <see cref="IDisposable"/>.</para>
    /// </summary>
    /// <typeparam name="T">The type of the elements.</typeparam>
    public class ObservableSet<T> : IObservableCollection<T>, IDisposable
    {
        #region Instance props & fields

        private readonly HashSet<T> _items;

        public int Count => _items.Count;

        public bool IsReadOnly => false;

        private bool _isDisposed;
        protected bool IsDisposed => _isDisposed;

        #endregion

        #region Instance events

        private NotifyCollectionChangedEventHandler _collectionChanged;
        public event NotifyCollectionChangedEventHandler CollectionChanged
        {
            add
            {
                ThrowIfDisposed();
                _collectionChanged += value;
            }
            remove
            {
                if (_isDisposed) return;
                _collectionChanged -= value;
            }
        }

        private PropertyChangedEventHandler _propertyChanged;
        public event PropertyChangedEventHandler PropertyChanged
        {
            add
            {
                ThrowIfDisposed();
                _propertyChanged += value;
            }
            remove
            {
                if (_isDisposed) return;
                _propertyChanged -= value;
            }
        }

        #endregion

        #region Instance constructors

        public ObservableSet()
        {
            _items = new HashSet<T>();
        }

        public ObservableSet(IEnumerable<T> collection)
        {
            _items = new HashSet<T>(collection);
        }

        public ObservableSet(IEqualityComparer<T> equalityComparer)
        {
            _items = new HashSet<T>(equalityComparer);
        }

        public ObservableSet(IEnumerable<T> collection, IEqualityComparer<T> equalityComparer)
        {
            _items = new HashSet<T>(collection, equalityComparer);
        }

        #endregion

        #region Instance methods

        // Collection implementation.
        public void Add(T item)
        {
            ThrowIfDisposed();
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }
            if (!_items.Add(item))
            {
                throw new ArgumentException("The collection already contains the specified item.");
            }

            RaiseChangeNotificationEvents(
                new NotifyCollectionChangedEventArgs(
                    NotifyCollectionChangedAction.Add,
                    item));
        }

        public void Clear()
        {
            ThrowIfDisposed();
            if (_items.Count == 0)
            {
                return;
            }

            _items.Clear();

            RaiseChangeNotificationEvents(
                new NotifyCollectionChangedEventArgs(
                    NotifyCollectionChangedAction.Reset));
        }

        public bool Contains(T item)
        {
            return _items.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            ThrowIfDisposed();
            if (array == null)
            {
                throw new ArgumentNullException(nameof(array));
            }
            if (array.Length - arrayIndex < Count)
            {
                throw new ArgumentException("The specified array is too small.");
            }

            foreach (T item in _items)
            {
                array[arrayIndex++] = item;
            }
        }

        public bool Remove(T item)
        {
            ThrowIfDisposed();

            if (_items.Remove(item))
            {
                RaiseChangeNotificationEvents(
                    new NotifyCollectionChangedEventArgs(
                        NotifyCollectionChangedAction.Remove, item));
                return true;
            }
            else
            {
                return false;
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            ThrowIfDisposed();
            return _items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() => (this as IEnumerable<T>).GetEnumerator();

        // Miscellaneous.
        private void RaiseChangeNotificationEvents(NotifyCollectionChangedEventArgs e)
        {
            _propertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Count)));
            _collectionChanged?.Invoke(this, e);
        }

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

        protected virtual void OnDisposing(bool releaseManaged)
        {
            if (releaseManaged)
            {
                _propertyChanged = null;
                _collectionChanged = null;
            }
        }

        #endregion

        #region Destructor

        ~ObservableSet()
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