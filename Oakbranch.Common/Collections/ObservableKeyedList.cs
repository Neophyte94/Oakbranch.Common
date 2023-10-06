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
        #region Static members

        public const int DictionaryThreshold = 10;

        #endregion

        #region Instance members

        private List<TValue> m_List;
        private Dictionary<TKey, TValue> m_Dictionary;

        public int Count => m_List.Count;

        public bool IsReadOnly => false;

        protected virtual bool IsChangesAware => false;

        private bool m_IsDisposed;
        protected bool IsDisposed => m_IsDisposed;

        #endregion

        #region Instance indexers

        public TValue this[int index]
        {
            get
            {
                return m_List[index];
            }
            set
            {
                if (index < 0 || index >= m_List.Count)
                    throw GenerateArgumentOutOfRangeException(nameof(index), index, 0, m_List.Count - 1);

                ValidateNewItem(value);
                TValue itemToRemove = m_List[index];
                TKey newKey = GetKey(value);
                TKey oldKey = GetKey(itemToRemove);
                if (ReferenceEquals(itemToRemove, value)) return;

                if (CheckDictionaryState())
                {
                    m_Dictionary.Remove(oldKey);
                    m_Dictionary.Add(newKey, value);
                }
                else
                {
                    int count = m_List.Count;
                    for (int i = 0; i != count;)
                    {
                        TKey existingKey = GetKey(m_List[i++]);
                        if (existingKey.Equals(newKey) && !existingKey.Equals(oldKey))
                            throw GenerateItemAlreadyExistsException();
                    }
                }

                m_List[index] = value;

                if (IsChangesAware)
                {
                    OnItemRemoved(itemToRemove);
                    OnItemAdded(value);
                }

                RaiseChangeNotificationEvents(
                    new NotifyCollectionChangedEventArgs(
                        NotifyCollectionChangedAction.Replace, value, itemToRemove, index));
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

        private PropertyChangedEventHandler m_PropertyChanged;
        public event PropertyChangedEventHandler PropertyChanged
        {
            add
            {
                ThrowIfDisposed();
                m_PropertyChanged += value;
            }
            remove
            {
                if (m_IsDisposed) return;
                m_PropertyChanged -= value;
            }
        }

        private NotifyCollectionChangedEventHandler m_CollectionChanged;
        public event NotifyCollectionChangedEventHandler CollectionChanged
        {
            add
            {
                ThrowIfDisposed();
                m_CollectionChanged += value;
            }
            remove
            {
                if (m_IsDisposed) return;
                m_CollectionChanged -= value;
            }
        }

        #endregion

        #region Instance constructors

        public ObservableKeyedList()
        {
            m_List = new List<TValue>();
        }

        public ObservableKeyedList(int capacity)
        {
            m_List = new List<TValue>(capacity);
        }

        public ObservableKeyedList(IEnumerable<TValue> collection)
        {
            m_List = new List<TValue>(collection);
            if (!CheckDictionaryState())
            {
                int count = m_List.Count;
                for (int i = 0; i != count; ++i)
                {
                    TValue item = m_List[i];
                    ValidateNewItem(item);
                    TKey key = GetKey(item);
                    for (int j = 0; j != count; ++j)
                    {
                        if (i == j) continue;
                        if (GetKey(m_List[j]).Equals(key))
                            throw new ArgumentException("The specified collection contains duplicate items.");
                    }
                }
            }
            if (IsChangesAware)
            {
                foreach (TValue item in m_List)
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
                return m_Dictionary.ContainsKey(key);
            }
            else
            {
                int count = m_List.Count;
                for (int i = 0; i != count; ++i)
                {
                    if (GetKey(this[i]).Equals(key)) return true;
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
            if (!CheckDictionaryState() || m_Dictionary.ContainsKey(key))
            {
                int count = m_List.Count;
                for (int i = 0; i != count; ++i)
                {
                    if (GetKey(this[i]).Equals(key)) return i;
                }
            }
            return -1;
        }

        public void Add(TValue item)
        {
            InsertInternal(item, -1);
        }

        public void AddRange(IEnumerable<TValue> items)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            if (!m_IsDisposed && m_Dictionary == null)
            {
                int capacity = m_List.Count;
                if (items is ICollection<TValue> c)
                    capacity += c.Count;
                else if (items is TValue[] a)
                    capacity += a.Length;
                else
                    capacity <<= 1;

                if (capacity >= DictionaryThreshold) CreateDictionary(capacity);
            }

            int startIdx = m_List.Count;
            if (CheckDictionaryState())
            {
                foreach (TValue item in items)
                {
                    TKey key = GetKey(item);
                    m_Dictionary.Add(key, item);
                }
            }
            else
            {
                foreach (TValue item in items)
                {
                    TKey key = GetKey(item);
                    int count = m_List.Count;
                    for (int i = 0; i != count;)
                    {
                        if (GetKey(m_List[i++]).Equals(key))
                            throw GenerateItemAlreadyExistsException();
                    }
                }
            }

            m_List.AddRange(items);

            if (IsChangesAware)
            {
                int count = m_List.Count;
                for (int i = startIdx; i != count;)
                {
                    OnItemAdded(m_List[i++]);
                }
            }

            RaiseChangeNotificationEvents(
                new NotifyCollectionChangedEventArgs(
                    NotifyCollectionChangedAction.Add, new List<TValue>(items), startIdx));
        }

        public void Clear()
        {
            if (m_List.Count == 0) return;

            List<TValue> oldList = m_List;
            m_List = new List<TValue>(oldList.Capacity);
            m_Dictionary = new Dictionary<TKey, TValue>(oldList.Capacity);

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
            m_List.CopyTo(array, arrayIndex);
        }

        public void Insert(int index, TValue item)
        {
            if (index < 0 || index > m_List.Count)
                throw GenerateArgumentOutOfRangeException(nameof(index), index, 0, m_List.Count);
        }

        private void InsertInternal(TValue item, int index)
        {
            // Ensure that the new item is considered valid by the derived class.
            ValidateNewItem(item);
            TKey key = GetKey(item);

            // Update the internal dictionary.
            if (CheckDictionaryState())
            {
                m_Dictionary.Add(key, item);
            }
            else
            {
                int count = m_List.Count;
                for (int i = 0; i != count;)
                {
                    if (GetKey(m_List[i++]).Equals(key))
                        throw GenerateItemAlreadyExistsException();
                }
            }

            // Insert the item into the list.
            if (index == -1)
            {
                m_List.Add(item);
                index = m_List.Count - 1;
            }
            else
            {
                m_List.Insert(index, item);
            }

            // Raise the protected-scope notification.
            if (IsChangesAware)
            {
                OnItemAdded(item);
            }

            // Raise the public-scope notifications.
            RaiseChangeNotificationEvents(
                new NotifyCollectionChangedEventArgs(
                    NotifyCollectionChangedAction.Add, item, index));
        }

        public bool Remove(TValue item)
        {
            int index = m_List.IndexOf(item);
            if (index == -1) return false;

            m_List.RemoveAt(index);
            if (m_Dictionary != null && !m_IsDisposed)
            {
                m_Dictionary.Remove(GetKey(item));
            }

            if (IsChangesAware)
            {
                OnItemRemoved(item);
            }

            RaiseChangeNotificationEvents(
                new NotifyCollectionChangedEventArgs(
                    NotifyCollectionChangedAction.Remove, item, index));
            return true;
        }

        public void RemoveAt(int index)
        {
            TValue item = m_List[index];
            m_List.RemoveAt(index);
            if (m_Dictionary != null && !m_IsDisposed)
            {
                m_Dictionary.Remove(GetKey(item));
            }

            if (IsChangesAware)
            {
                OnItemRemoved(item);
            }

            RaiseChangeNotificationEvents(
                new NotifyCollectionChangedEventArgs(
                    NotifyCollectionChangedAction.Remove, item, index));
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            if (CheckDictionaryState())
            {
                return m_Dictionary.TryGetValue(key, out value);
            }
            else
            {
                int count = m_List.Count;
                for (int i = 0; i != count; ++i)
                {
                    value = this[i];
                    if (GetKey(value).Equals(key)) return true;
                }
                value = default;
                return false;
            }
        }

        public void Sort(Comparison<TValue> comparison)
        {
            m_List.Sort(comparison);
            RaiseChangeNotificationEvents(
                new NotifyCollectionChangedEventArgs(
                    NotifyCollectionChangedAction.Reset));
        }

        public void TrimExcess()
        {
            if (m_List.Count < m_List.Capacity)
            {
                m_List.TrimExcess();
            }
        }

        public IEnumerator<TValue> GetEnumerator()
        {
            return m_List.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return m_List.GetEnumerator();
        }

        // Internal dictionary management.
        private bool CheckDictionaryState()
        {
            if (m_Dictionary == null)
            {
                if (m_IsDisposed || m_List.Count < DictionaryThreshold) return false;
                CreateDictionary(m_List.Count << 1);
                return true;
            }
            else
            {
                return true;
            }
        }

        private void CreateDictionary(int capacity)
        {
            m_Dictionary = new Dictionary<TKey, TValue>(capacity);
            int count = m_List.Count;
            for (int i = 0; i != count; ++i)
            {
                TValue item = m_List[i];
                m_Dictionary.Add(GetKey(item), item);
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
                m_PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Count)));
            }
            m_CollectionChanged?.Invoke(this, e);
        }

        protected void ThrowIfDisposed()
        {
            if (m_IsDisposed)
                throw new ObjectDisposedException(GetType().Name);
        }

        /// <summary>
        /// Clears event handlers. Keeps the collection of items.
        /// </summary>
        public virtual void Dispose()
        {
            if (!m_IsDisposed)
            {
                m_IsDisposed = true;
                OnDisposing(true);
                GC.SuppressFinalize(this);
            }
        }

        protected virtual void OnDisposing(bool releaseManaged)
        {
            if (releaseManaged)
            {
                m_PropertyChanged = null;
                m_CollectionChanged = null;
            }
        }

        #endregion

        #region Destructor

        ~ObservableKeyedList()
        {
            if (!m_IsDisposed)
            {
                m_IsDisposed = true;
                OnDisposing(false);
            }
        }

        #endregion
    }
}
