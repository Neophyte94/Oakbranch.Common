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
    public class ReadOnlyObservableList<T> : ReadOnlyList<T>, INotifyCollectionChanged, INotifyPropertyChanged, IDisposable
    {
        #region Instance members

        private bool m_IsDisposed;

        #endregion

        #region Instance events

        [SuppressMessage("CodeQuality", "IDE0052:Remove unread private members", Justification = "Formal implementation")]
        private NotifyCollectionChangedEventHandler m_CollectionChanged;
        public event NotifyCollectionChangedEventHandler CollectionChanged
        {
            add
            {
                if (!m_IsDisposed)
                {
                    m_CollectionChanged += value;
                }
            }
            remove
            {
                m_CollectionChanged -= value;
            }
        }

        [SuppressMessage("CodeQuality", "IDE0052:Remove unread private members", Justification = "Formal implementation")]
        private PropertyChangedEventHandler m_PropertyChanged;
        public event PropertyChangedEventHandler PropertyChanged
        {
            add
            {
                if (!m_IsDisposed)
                {
                    m_PropertyChanged += value;
                }
            }
            remove
            {
                m_PropertyChanged -= value;
            }
        }

        #endregion

        #region Instance constructors

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

        #endregion

        #region Instance methods

        public void Dispose()
        {
            if (!m_IsDisposed)
            {
                m_CollectionChanged = null;
                m_PropertyChanged = null;
                m_IsDisposed = true;
            }
        }

        #endregion
    }
}
