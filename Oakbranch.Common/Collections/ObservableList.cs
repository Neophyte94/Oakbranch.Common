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
        #region Instance members

        private List<T> m_List;

        public int Count => m_List.Count;

        public int Capacity { get => m_List.Capacity; set => m_List.Capacity = value; }

        public bool IsReadOnly => false;

        protected virtual bool IsChangesAware => false;

        private bool m_IsDisposed;
        protected bool IsDisposed => m_IsDisposed;

        #endregion

        #region Instance indexers

        public T this[int index]
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
                    T itemToRemove = m_List[index];
                    if (Equals(itemToRemove, value)) return;
                    m_List[index] = value;
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

        public ObservableList()
        {
            m_List = new List<T>();
        }

        public ObservableList(int capacity)
        {
            m_List = new List<T>(capacity);
        }

        public ObservableList(IEnumerable<T> collection)
        {
            m_List = new List<T>(collection);
            if (IsChangesAware)
            {
                foreach (T item in m_List)
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
            m_List.Add(item);
            if (IsChangesAware && item != null)
            {
                OnItemAdded(item);
            }
            RaiseChangeNotificationEvents(new NotifyCollectionChangedEventArgs(
                NotifyCollectionChangedAction.Add, item, m_List.Count - 1));
        }

        public virtual void AddRange(IEnumerable<T> items)
        {
            if (items == null) throw new ArgumentNullException(nameof(items));
            int startIdx = Count;
            m_List.AddRange(items);
            if (IsChangesAware)
            {
                int count = Count;
                for (int i = startIdx; i != count; ++i)
                {
                    if (m_List[i] != null) OnItemAdded(m_List[i]);
                }
            }
            RaiseChangeNotificationEvents(new NotifyCollectionChangedEventArgs(
                NotifyCollectionChangedAction.Add, new List<T>(items), startIdx));
        }

        public virtual void Clear()
        {
            if (m_List.Count == 0) return;
            List<T> oldList = m_List;
            m_List = new List<T>(oldList.Capacity);
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
            return m_List.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            m_List.CopyTo(array, arrayIndex);
        }

        public int IndexOf(T item)
        {
            return m_List.IndexOf(item);
        }

        public virtual void Insert(int index, T item)
        {
            m_List.Insert(index, item);
            if (IsChangesAware && item != null)
            {
                OnItemAdded(item);
            }
            RaiseChangeNotificationEvents(new NotifyCollectionChangedEventArgs(
                NotifyCollectionChangedAction.Add, item, index));
        }

        public virtual bool Remove(T item)
        {
            int index = m_List.IndexOf(item);
            if (index == -1) return false;
            m_List.RemoveAt(index);
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
            T item = m_List[index];
            m_List.RemoveAt(index);
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
                items[j++] = m_List[i++];
            }

            m_List.RemoveRange(index, count);

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
            if (m_List.Count > 1)
            {
                m_List.Sort(comparison);
                RaiseChangeNotificationEvents(new NotifyCollectionChangedEventArgs(
                    NotifyCollectionChangedAction.Reset));
            }
        }

        public void Sort(IComparer<T> comparer)
        {
            if (m_List.Count > 1)
            {
                m_List.Sort(comparer);
                RaiseChangeNotificationEvents(new NotifyCollectionChangedEventArgs(
                    NotifyCollectionChangedAction.Reset));
            }
        }

        public void TrimExcess()
        {
            m_List.TrimExcess();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return m_List.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return m_List.GetEnumerator();
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
        public void Dispose()
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

        ~ObservableList()
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
