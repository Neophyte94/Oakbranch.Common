using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Oakbranch.Common.Collections
{
    /// <summary>
    /// Represents a list of uniquely identified elements that can be accessed by their keys.
    /// <para>Implements <see cref="IObservableList{T}"/> and <see cref="IDisposable"/>.</para>
    /// </summary>
    /// <typeparam name="TKey">The type of keys that uniquely identify elements in the list.</typeparam>
    /// <typeparam name="TValue">The type of elements in the list.</typeparam>
    public abstract class ObservableKeyedList<TKey, TValue> : IObservableList<TValue>, IDisposable
    {
        #region Constants

        private const int DICTIONARY_THRESHOLD = 10;

        #endregion

        #region Instance props & fields

        private List<TValue> _list;
        private Dictionary<TKey, TValue> _dictionary;

        public int Count => _list.Count;

        public bool IsReadOnly => false;

        protected virtual bool IsChangesAware => false;

        private bool _isDisposed;
        protected bool IsDisposed => _isDisposed;

        #endregion

        #region Instance indexers

        public TValue this[int index]
        {
            get
            {
                return _list[index];
            }
            set
            {
                ThrowIfDisposed();
                if (index < 0 || index >= _list.Count)
                {
                    throw GenerateArgumentOutOfRangeException(nameof(index), index, 0, _list.Count - 1);
                }

                ValidateNewItem(value);
                TValue itemToRemove = _list[index];
                TKey newKey = GetKey(value);
                TKey oldKey = GetKey(itemToRemove);
                if (ReferenceEquals(itemToRemove, value))
                {
                    return;
                }

                if (CheckDictionaryState())
                {
                    _dictionary.Remove(oldKey);
                    _dictionary.Add(newKey, value);
                }
                else
                {
                    int count = _list.Count;
                    for (int i = 0; i != count;)
                    {
                        TKey existingKey = GetKey(_list[i++]);
                        if (existingKey.Equals(newKey) && !existingKey.Equals(oldKey))
                        {
                            throw GenerateItemAlreadyExistsException();
                        }
                    }
                }

                _list[index] = value;

                if (IsChangesAware)
                {
                    OnItemRemoved(itemToRemove);
                    OnItemAdded(value);
                }

                RaiseChangeNotificationEvents(
                    new NotifyCollectionChangedEventArgs(
                        NotifyCollectionChangedAction.Replace,
                        value,
                        itemToRemove,
                        index));
            }
        }

        public TValue this[TKey key]
        {
            get
            {
                int idx = IndexOf(key);
                if (idx == -1)
                    throw new ArgumentException("No item with the specified key is contained in the list.");
                return this[idx];
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

        public ObservableKeyedList()
        {
            _list = new List<TValue>();
        }

        public ObservableKeyedList(int capacity)
        {
            _list = new List<TValue>(capacity);
        }

        public ObservableKeyedList(IEnumerable<TValue> collection)
        {
            _list = new List<TValue>(collection);
            if (!CheckDictionaryState())
            {
                int count = _list.Count;
                for (int i = 0; i != count; ++i)
                {
                    TValue item = _list[i];
                    ValidateNewItem(item);
                    TKey key = GetKey(item);
                    for (int j = 0; j != count; ++j)
                    {
                        if (i == j) continue;
                        if (GetKey(_list[j]).Equals(key))
                            throw new ArgumentException("The specified collection contains duplicate items.");
                    }
                }
            }
            if (IsChangesAware)
            {
                foreach (TValue item in _list)
                {
                    OnItemAdded(item);
                }
            }
        }

        #endregion

        #region Static methods

        private static ArgumentOutOfRangeException GenerateArgumentOutOfRangeException(
            string paramName, int input, int minInclusive, int maxInclusive)
        {
            return new ArgumentOutOfRangeException(
                paramName,
                $"The specified value ({input}) is out of the current acceptable range [{minInclusive} ; {maxInclusive}].");
        }

        private static ArgumentException GenerateItemAlreadyExistsException()
        {
            return new ArgumentException("The specified item has already been added to the keyed list.");
        }

        #endregion

        #region Instance methods

        // List implementation.
        public bool ContainsKey(TKey key)
        {
            if (CheckDictionaryState())
            {
                return _dictionary.ContainsKey(key);
            }
            else
            {
                int count = _list.Count;
                for (int i = 0; i != count; ++i)
                {
                    if (GetKey(this[i]).Equals(key))
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        public bool Contains(TValue item)
        {
            return item != null && ContainsKey(GetKey(item));
        }

        public int IndexOf(TValue item)
        {
            return item == null ? -1 : IndexOf(GetKey(item));
        }

        public int IndexOf(TKey key)
        {
            if (!CheckDictionaryState() || _dictionary.ContainsKey(key))
            {
                int count = _list.Count;
                for (int i = 0; i != count; ++i)
                {
                    if (GetKey(this[i]).Equals(key))
                    {
                        return i;
                    }
                }
            }

            return -1;
        }

        public void Add(TValue item)
        {
            ThrowIfDisposed();
            InsertInternal(item, -1);
        }

        public void AddRange(IEnumerable<TValue> items)
        {
            ThrowIfDisposed();
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            if (!_isDisposed && _dictionary == null)
            {
                int capacity = _list.Count;
                if (items is ICollection<TValue> c)
                {
                    capacity += c.Count;
                }
                else if (items is TValue[] a)
                {
                    capacity += a.Length;
                }
                else
                {
                    capacity <<= 1;
                }

                if (capacity >= DICTIONARY_THRESHOLD)
                {
                    CreateDictionary(capacity);
                }
            }

            int startIdx = _list.Count;
            if (CheckDictionaryState())
            {
                foreach (TValue item in items)
                {
                    TKey key = GetKey(item);
                    _dictionary.Add(key, item);
                }
            }
            else
            {
                foreach (TValue item in items)
                {
                    TKey key = GetKey(item);
                    int count = _list.Count;
                    for (int i = 0; i != count;)
                    {
                        if (GetKey(_list[i++]).Equals(key))
                        {
                            throw GenerateItemAlreadyExistsException();
                        }
                    }
                }
            }

            _list.AddRange(items);

            if (IsChangesAware)
            {
                int count = _list.Count;
                for (int i = startIdx; i != count;)
                {
                    OnItemAdded(_list[i++]);
                }
            }

            RaiseChangeNotificationEvents(
                new NotifyCollectionChangedEventArgs(
                    NotifyCollectionChangedAction.Add,
                    new List<TValue>(items),
                    startIdx));
        }

        public void Clear()
        {
            if (_list.Count == 0)
            {
                return;
            }

            List<TValue> oldList = _list;
            _list = new List<TValue>(oldList.Capacity);
            _dictionary = new Dictionary<TKey, TValue>(oldList.Capacity);

            if (IsChangesAware)
            {
                foreach (TValue item in oldList)
                {
                    OnItemRemoved(item);
                }
            }

            RaiseChangeNotificationEvents(
                new NotifyCollectionChangedEventArgs(
                    NotifyCollectionChangedAction.Reset));
        }

        public void CopyTo(TValue[] array, int arrayIndex)
        {
            _list.CopyTo(array, arrayIndex);
        }

        public void Insert(int index, TValue item)
        {
            ThrowIfDisposed();
            if (index < 0 || index > _list.Count)
            {
                throw GenerateArgumentOutOfRangeException(nameof(index), index, 0, _list.Count);
            }

            InsertInternal(item, index);
        }

        private void InsertInternal(TValue item, int index)
        {
            // Ensure that the new item is considered valid by the derived class.
            ValidateNewItem(item);
            TKey key = GetKey(item);

            // Update the internal dictionary.
            if (CheckDictionaryState())
            {
                _dictionary.Add(key, item);
            }
            else
            {
                int count = _list.Count;
                for (int i = 0; i != count;)
                {
                    if (GetKey(_list[i++]).Equals(key))
                    {
                        throw GenerateItemAlreadyExistsException();
                    }
                }
            }

            // Insert the item into the list.
            if (index == -1)
            {
                _list.Add(item);
                index = _list.Count - 1;
            }
            else
            {
                _list.Insert(index, item);
            }

            // Raise the protected-scope notification.
            if (IsChangesAware)
            {
                OnItemAdded(item);
            }

            // Raise the public-scope notifications.
            RaiseChangeNotificationEvents(
                new NotifyCollectionChangedEventArgs(
                    NotifyCollectionChangedAction.Add,
                    item,
                    index));
        }

        public bool Remove(TValue item)
        {
            ThrowIfDisposed();

            int index = _list.IndexOf(item);
            if (index == -1)
            {
                return false;
            }

            _list.RemoveAt(index);
            if (_dictionary != null && !_isDisposed)
            {
                _dictionary.Remove(GetKey(item));
            }

            if (IsChangesAware)
            {
                OnItemRemoved(item);
            }

            RaiseChangeNotificationEvents(
                new NotifyCollectionChangedEventArgs(
                    NotifyCollectionChangedAction.Remove,
                    item,
                    index));

            return true;
        }

        public void RemoveAt(int index)
        {
            ThrowIfDisposed();
            if (index < 0 || index >= _list.Count)
            {
                throw GenerateArgumentOutOfRangeException(nameof(index), index, 0, _list.Count - 1);
            }

            TValue item = _list[index];
            _list.RemoveAt(index);
            if (_dictionary != null && !_isDisposed)
            {
                _dictionary.Remove(GetKey(item));
            }

            if (IsChangesAware)
            {
                OnItemRemoved(item);
            }

            RaiseChangeNotificationEvents(
                new NotifyCollectionChangedEventArgs(
                    NotifyCollectionChangedAction.Remove,
                    item,
                    index));
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            if (CheckDictionaryState())
            {
                return _dictionary.TryGetValue(key, out value);
            }
            else
            {
                int count = _list.Count;
                for (int i = 0; i != count; ++i)
                {
                    value = this[i];
                    if (GetKey(value).Equals(key))
                    {
                        return true;
                    }
                }

                value = default;
                return false;
            }
        }

        public void Sort(Comparison<TValue> comparison)
        {
            _list.Sort(comparison);
            RaiseChangeNotificationEvents(
                new NotifyCollectionChangedEventArgs(
                    NotifyCollectionChangedAction.Reset));
        }

        public void TrimExcess()
        {
            if (_list.Count < _list.Capacity)
            {
                _list.TrimExcess();
            }
        }

        public IEnumerator<TValue> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() => (this as IEnumerable<TValue>).GetEnumerator();

        // Internal dictionary management.
        private bool CheckDictionaryState()
        {
            if (_dictionary == null)
            {
                if (_isDisposed || _list.Count < DICTIONARY_THRESHOLD)
                {
                    return false;
                }

                CreateDictionary(_list.Count << 1);
                return true;
            }
            else
            {
                return true;
            }
        }

        private void CreateDictionary(int capacity)
        {
            _dictionary = new Dictionary<TKey, TValue>(capacity);
            int count = _list.Count;
            for (int i = 0; i != count; ++i)
            {
                TValue item = _list[i];
                _dictionary.Add(GetKey(item), item);
            }
        }

        // Derived-class customizables.
        protected abstract TKey GetKey(TValue item);

        protected virtual void ValidateNewItem(TValue item) { }

        protected virtual void OnItemAdded(TValue item) { }

        protected virtual void OnItemRemoved(TValue item) { }

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
            {
                throw new ObjectDisposedException(GetType().Name);
            }
        }

        /// <summary>
        /// Clears event handlers. Keeps the collection of items.
        /// </summary>
        public virtual void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool releaseManaged)
        {
            if (_isDisposed)
            {
                return;
            }

            if (releaseManaged)
            {
                _propertyChanged = null;
                _collectionChanged = null;
            }

            OnDisposing(releaseManaged);

            _isDisposed = true;
        }

        protected virtual void OnDisposing(bool releaseManaged) { }

        #endregion

        #region Destructor

        ~ObservableKeyedList()
        {
            Dispose(false);
        }

        #endregion
    }
}
