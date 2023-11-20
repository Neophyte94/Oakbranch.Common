using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Oakbranch.Common.Collections
{
    /// <summary>
    /// Representa a light-weight list of elements that allows tracking changes to it.
    /// <para>Implements <see cref="IObservableList{T}"/> and <see cref="IDisposable"/>.</para>
    /// </summary>
    /// <typeparam name="T">The type of the elements.</typeparam>
    public class ObservableList<T> : IObservableList<T>, IDisposable
    {
        #region Instance props & fields

        private List<T> _items;

        public int Count => _items.Count;

        public int Capacity { get => _items.Capacity; set => _items.Capacity = value; }

        public bool IsReadOnly => false;

        protected virtual bool IsChangesAware => false;

        private bool _isDisposed;
        protected bool IsDisposed => _isDisposed;

        #endregion

        #region Instance indexers

        public T this[int index]
        {
            get
            {
                return _items[index];
            }
            set
            {
                if (index == _items.Count)
                {
                    Add(value);
                }
                else
                {
                    T itemToRemove = _items[index];
                    if (Equals(itemToRemove, value)) return;
                    _items[index] = value;
                    if (IsChangesAware)
                    {
                        if (itemToRemove != null) OnItemRemoved(itemToRemove);
                        if (value != null) OnItemAdded(value);
                    }
                    RaiseChangeNotificationEvents(new NotifyCollectionChangedEventArgs(
                        NotifyCollectionChangedAction.Replace, value, itemToRemove, index));
                }
            }
        }

        #endregion

        #region Instance events

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

        #endregion

        #region Instance constructors

        public ObservableList()
        {
            _items = new List<T>();
        }

        public ObservableList(int capacity)
        {
            _items = new List<T>(capacity);
        }

        public ObservableList(IEnumerable<T> collection)
        {
            _items = new List<T>(collection);
            if (IsChangesAware)
            {
                foreach (T item in _items)
                {
                    if (item != null) OnItemAdded(item);
                }
            }
        }

        #endregion

        #region Instance methods

        // List implementation.
        public virtual void Add(T item)
        {
            _items.Add(item);
            if (IsChangesAware && item != null)
            {
                OnItemAdded(item);
            }
            RaiseChangeNotificationEvents(new NotifyCollectionChangedEventArgs(
                NotifyCollectionChangedAction.Add, item, _items.Count - 1));
        }

        public virtual void AddRange(IEnumerable<T> items)
        {
            if (items == null) throw new ArgumentNullException(nameof(items));
            int startIdx = Count;
            _items.AddRange(items);
            if (IsChangesAware)
            {
                int count = Count;
                for (int i = startIdx; i != count; ++i)
                {
                    if (_items[i] != null) OnItemAdded(_items[i]);
                }
            }
            RaiseChangeNotificationEvents(new NotifyCollectionChangedEventArgs(
                NotifyCollectionChangedAction.Add, new List<T>(items), startIdx));
        }

        public virtual void Clear()
        {
            if (_items.Count == 0) return;
            List<T> oldList = _items;
            _items = new List<T>(oldList.Capacity);
            if (IsChangesAware)
            {
                foreach(T item in oldList)
                {
                    if (item != null) OnItemRemoved(item);
                }
            }
            RaiseChangeNotificationEvents(new NotifyCollectionChangedEventArgs(
                NotifyCollectionChangedAction.Reset));
        }

        public virtual bool Contains(T item)
        {
            return _items.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _items.CopyTo(array, arrayIndex);
        }

        public int IndexOf(T item)
        {
            return _items.IndexOf(item);
        }

        public virtual void Insert(int index, T item)
        {
            _items.Insert(index, item);
            if (IsChangesAware && item != null)
            {
                OnItemAdded(item);
            }
            RaiseChangeNotificationEvents(new NotifyCollectionChangedEventArgs(
                NotifyCollectionChangedAction.Add, item, index));
        }

        public virtual bool Remove(T item)
        {
            int index = _items.IndexOf(item);
            if (index == -1) return false;
            _items.RemoveAt(index);
            if (IsChangesAware)
            {
                OnItemRemoved(item);
            }
            RaiseChangeNotificationEvents(new NotifyCollectionChangedEventArgs(
                NotifyCollectionChangedAction.Remove, item, index));
            return true;
        }

        public virtual void RemoveAt(int index)
        {
            T item = _items[index];
            _items.RemoveAt(index);
            if (IsChangesAware && item != null)
            {
                OnItemRemoved(item);
            }
            RaiseChangeNotificationEvents(new NotifyCollectionChangedEventArgs(
                NotifyCollectionChangedAction.Remove, item, index));
        }

        public virtual void RemoveRange(int index, int count)
        {
            T[] items = new T[count];
            for (int i = 0, j = index; i != count;)
            {
                items[j++] = _items[i++];
            }

            _items.RemoveRange(index, count);

            if (IsChangesAware)
            {
                for (int i = 0; i != count;)
                {
                    OnItemAdded(items[i++]);
                }
            }

            RaiseChangeNotificationEvents(new NotifyCollectionChangedEventArgs(
                NotifyCollectionChangedAction.Remove, items, index));
        }

        public void Sort(Comparison<T> comparison)
        {
            if (_items.Count > 1)
            {
                _items.Sort(comparison);
                RaiseChangeNotificationEvents(new NotifyCollectionChangedEventArgs(
                    NotifyCollectionChangedAction.Reset));
            }
        }

        public void Sort(IComparer<T> comparer)
        {
            if (_items.Count > 1)
            {
                _items.Sort(comparer);
                RaiseChangeNotificationEvents(new NotifyCollectionChangedEventArgs(
                    NotifyCollectionChangedAction.Reset));
            }
        }

        public void TrimExcess()
        {
            _items.TrimExcess();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        // Derived-class customizables.
        protected virtual void OnItemAdded(T item) { }

        protected virtual void OnItemRemoved(T item) { }

        // Miscellaneous.
        private void RaiseChangeNotificationEvents(NotifyCollectionChangedEventArgs e)
        {
            if (e.Action != NotifyCollectionChangedAction.Move &&
                e.Action != NotifyCollectionChangedAction.Replace)
            {
                _propertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Count)));
            }
            _collectionChanged?.Invoke(this, e);
        }

        protected void ThrowIfDisposed()
        {
            if (_isDisposed)
                throw new ObjectDisposedException(GetType().Name);
        }

        /// <summary>
        /// Clears event handlers. Keeps the collection of items.
        /// </summary>
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

        ~ObservableList()
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
