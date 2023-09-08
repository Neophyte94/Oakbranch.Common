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

        private Dictionary<TKey, TValue> m_ItemsDictionary;

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
                if (index == m_List.Count)
                {
                    Add(value);
                }
                else
                {
                    TValue itemToRemove = m_List[index];
                    if (Equals(itemToRemove, value)) return;
                    m_List[index] = value;
                    if (IsChangesAware)
                    {
                        OnItemRemoved(itemToRemove);
                        OnItemAdded(value);
                    }
                    OnCollectionChanged(new NotifyCollectionChangedEventArgs(
                        NotifyCollectionChangedAction.Replace, value, itemToRemove, index));
                }
            }
        }

        public TValue this[TKey key]
        {
            get
            {
                int idx = IndexOf(key);
                if (idx == -1) throw new ArgumentException("No item with the specified key is contained in the list.");
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
                if (m_IsDisposed)
                    throw new ObjectDisposedException("An instance is disposed and cannot get subscribers.");
                m_PropertyChanged += value;
            }
            remove
            {
                if (!m_IsDisposed) m_PropertyChanged -= value;
            }
        }

        private NotifyCollectionChangedEventHandler m_CollectionChanged;
        public event NotifyCollectionChangedEventHandler CollectionChanged
        {
            add
            {
                if (m_IsDisposed)
                    throw new ObjectDisposedException("An instance is disposed and cannot get subscribers.");
                m_CollectionChanged += value;
            }
            remove
            {
                if (!m_IsDisposed) m_CollectionChanged -= value;
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

        private static ArgumentException GenerateItemAlreadyExistsException()
        {
            return new ArgumentException("The specified item has already been added to the keyed list.");
        }

        #endregion

        #region Instance methods

        private bool CheckDictionaryState()
        {
            if (m_ItemsDictionary == null)
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
            m_ItemsDictionary = new Dictionary<TKey, TValue>(capacity);
            int count = m_List.Count;
            for (int i = 0; i != count; ++i)
            {
                TValue item = m_List[i];
                m_ItemsDictionary.Add(GetKey(item), item);
            }
        }

        public bool ContainsKey(TKey key)
        {
            if (CheckDictionaryState())
            {
                return m_ItemsDictionary.ContainsKey(key);
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
            if (item == null) return false;
            return ContainsKey(GetKey(item));
        }

        public int IndexOf(TValue item)
        {
            if (item == null) return -1;
            return IndexOf(GetKey(item));
        }

        public int IndexOf(TKey key)
        {
            if (!CheckDictionaryState() || m_ItemsDictionary.ContainsKey(key))
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
            OnItemAdding(item);
            m_List.Add(item);
            if (IsChangesAware) OnItemAdded(item);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(
                NotifyCollectionChangedAction.Add, item, m_List.Count - 1));
        }

        public void AddRange(IEnumerable<TValue> items)
        {
            if (items == null) throw new ArgumentNullException(nameof(items));
            if (!m_IsDisposed && m_ItemsDictionary == null)
            {
                int capacity = m_List.Count;
                if (items is ICollection<TValue> c)
                    capacity += c.Count;
                else if (items is TValue[] a)
                    capacity += a.Length;
                else
                    capacity = (capacity << 1);
                if (capacity >= DictionaryThreshold) CreateDictionary(capacity);
            }
            int startIdx = m_List.Count;
            if (CheckDictionaryState())
            {
                foreach (TValue item in items)
                {
                    TKey key = GetKey(item);
                    m_ItemsDictionary.Add(key, item);
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
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(
                NotifyCollectionChangedAction.Add, new List<TValue>(items), startIdx));
        }

        public void Clear()
        {
            if (m_List.Count == 0) return;
            List<TValue> oldList = m_List;
            m_List = new List<TValue>(oldList.Capacity);
            m_ItemsDictionary = new Dictionary<TKey, TValue>(oldList.Capacity);
            if (IsChangesAware)
            {
                foreach (TValue item in oldList)
                {
                    OnItemRemoved(item);
                }
            }
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(
                NotifyCollectionChangedAction.Reset));
        }

        public void CopyTo(TValue[] array, int arrayIndex)
        {
            m_List.CopyTo(array, arrayIndex);
        }

        public void Insert(int index, TValue item)
        {
            OnItemAdding(item);
            m_List.Insert(index, item);
            if (IsChangesAware)
                OnItemAdded(item);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(
                NotifyCollectionChangedAction.Add, item, index));
        }

        public bool Remove(TValue item)
        {
            int index = m_List.IndexOf(item);
            if (index == -1) return false;
            m_List.RemoveAt(index);
            if (m_ItemsDictionary != null && !m_IsDisposed)
                m_ItemsDictionary.Remove(GetKey(item));
            if (IsChangesAware)
                OnItemRemoved(item);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(
                NotifyCollectionChangedAction.Remove, item, index));
            return true;
        }

        public void RemoveAt(int index)
        {
            TValue item = m_List[index];
            m_List.RemoveAt(index);
            if (m_ItemsDictionary != null && !m_IsDisposed)
                m_ItemsDictionary.Remove(GetKey(item));
            if (IsChangesAware)
                OnItemRemoved(item);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(
                NotifyCollectionChangedAction.Remove, item, index));
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            if (CheckDictionaryState())
            {
                return m_ItemsDictionary.TryGetValue(key, out value);
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
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(
                NotifyCollectionChangedAction.Reset));
        }

        public IEnumerator<TValue> GetEnumerator()
        {
            return m_List.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return m_List.GetEnumerator();
        }

        private void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (e.Action != NotifyCollectionChangedAction.Move &&
                e.Action != NotifyCollectionChangedAction.Replace)
            {
                m_PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Count)));
            }
            m_CollectionChanged?.Invoke(this, e);
        }

        private void OnItemAdding(TValue item)
        {
            ValidateNewItem(item);
            TKey key = GetKey(item);
            if (CheckDictionaryState())
            {
                m_ItemsDictionary.Add(key, item);
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
        }

        protected virtual void ValidateNewItem(TValue item) { }

        protected virtual void OnItemAdded(TValue item) { }

        protected virtual void OnItemRemoved(TValue item) { }

        protected abstract TKey GetKey(TValue item);

        public void TrimExcess()
        {
            if (m_List.Count < m_List.Capacity) m_List.TrimExcess();
        }

        /// <summary>
        /// Clears event handlers. Keeps the collection of items.
        /// </summary>
        public virtual void Dispose()
        {
            m_PropertyChanged = null;
            m_CollectionChanged = null;
            m_IsDisposed = true;
        }

        #endregion
    }
}
