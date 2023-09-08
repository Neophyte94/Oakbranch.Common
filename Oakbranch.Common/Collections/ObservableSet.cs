using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Text;

namespace Oakbranch.Common.Collections
{
    public class ObservableSet<T> : IObservableCollection<T>, IDisposable
    {
        #region Instance members

        private readonly HashSet<T> m_Items;

        public int Count => m_Items.Count;

        public bool IsReadOnly => false;

        private bool m_IsDisposed;
        protected bool IsDisposed => m_IsDisposed;

        #endregion

        #region Instance events

        private NotifyCollectionChangedEventHandler m_CollectionChanged;
        public event NotifyCollectionChangedEventHandler CollectionChanged
        {
            add
            {
                if (m_IsDisposed)
                    throw new ObjectDisposedException(nameof(ObservableSet<T>));
                m_CollectionChanged += value;
            }
            remove
            {
                m_CollectionChanged -= value;
            }
        }

        private PropertyChangedEventHandler m_PropertyChanged;
        public event PropertyChangedEventHandler PropertyChanged
        {
            add
            {
                if (m_IsDisposed)
                    throw new ObjectDisposedException(nameof(ObservableSet<T>));
                m_PropertyChanged += value;
            }
            remove
            {
                m_PropertyChanged -= value;
            }
        }

        #endregion

        #region Instance constructors

        public ObservableSet()
        {
            m_Items = new HashSet<T>();
        }

        public ObservableSet(IEnumerable<T> collection)
        {
            m_Items = new HashSet<T>(collection);
        }

        public ObservableSet(IEqualityComparer<T> equalityComparer)
        {
            m_Items = new HashSet<T>(equalityComparer);
        }

        public ObservableSet(IEnumerable<T> collection, IEqualityComparer<T> equalityComparer)
        {
            m_Items = new HashSet<T>(collection, equalityComparer);
        }

        #endregion

        #region Instance methods

        public void Add(T item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));
            if (!m_Items.Add(item))
                throw new ArgumentException("The collection already contains the specified item.");
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(
                NotifyCollectionChangedAction.Add, item));
        }

        public void Clear()
        {
            if (m_Items.Count == 0) return;
            m_Items.Clear();
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(
                NotifyCollectionChangedAction.Reset));
        }

        public bool Contains(T item)
        {
            return m_Items.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));
            if (array.Length - arrayIndex < Count)
                throw new ArgumentException("The specified array is too small.");
            foreach (T item in m_Items)
            {
                array[arrayIndex++] = item;
            }
        }

        public bool Remove(T item)
        {
            if (m_Items.Remove(item))
            {
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(
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
            return m_Items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return m_Items.GetEnumerator();
        }

        private void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            m_PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Count)));
            m_CollectionChanged?.Invoke(this, e);
        }

        public void Dispose()
        {
            if (m_IsDisposed) return;
            m_PropertyChanged = null;
            m_CollectionChanged = null;
            m_IsDisposed = true;
        }

        #endregion
    }
}
