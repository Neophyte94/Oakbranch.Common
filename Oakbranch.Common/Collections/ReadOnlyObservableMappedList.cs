using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Threading;

namespace Oakbranch.Common.Collections
{
    /// <summary>
    /// Represents a read-only list of mapped elements that automatically synchronizes with a source list.
    /// </summary>
    /// <typeparam name="TSource">The type of elements in the source list.</typeparam>
    /// <typeparam name="TTarget">The type of elements exposed by the list.</typeparam>
    public abstract class ReadOnlyObservableMappedList<TSource, TTarget> :
        IReadOnlyObservableList<TTarget>,
        IDisposable
    {
        #region Instance props & fields

        private readonly IReadOnlyObservableList<TSource> _sourceItems;
        private readonly List<TTarget> _targetItems;
        private readonly ReaderWriterLockSlim _locker;
        private int _version = int.MinValue;

        public int Count => _targetItems.Count;

        private bool _isDisposed;
        protected bool IsDisposed => _isDisposed;

        #endregion

        #region Instance indexers

        public TTarget this[int index]
        {
            get
            {
                _locker.EnterReadLock();
                try
                {
                    return _targetItems[index];
                }
                finally
                {
                    _locker.ExitReadLock();
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
                if (IsDisposed) return;
                _collectionChanged -= value;
            }
        }

        #endregion

        #region Instance constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyObservableMappedList{TSource, TTarget}"/> class.
        /// </summary>
        /// <param name="sourceList">The source list to automatically synchronize with.</param>
        /// <param name="synchronizeOnCreation">
        /// Specifies whether the list must be instantly synchronized with the source.
        /// <para>Use <see langword="false"/> to configure the mapper first, and then manually call Synchronize().</para>
        /// </param>
        public ReadOnlyObservableMappedList(
            IReadOnlyObservableList<TSource> sourceList,
            bool synchronizeOnCreation = true)
        {
            _sourceItems = sourceList ?? throw new ArgumentNullException(nameof(sourceList));
            _targetItems = new List<TTarget>(_sourceItems.Count);
            _locker = new ReaderWriterLockSlim();

            _sourceItems.CollectionChanged += OnSourceListChanged;
            if (synchronizeOnCreation)
            {
                Synchronize();
            }
        }

        #endregion

        #region Instance methods

        /// <summary>
        /// When implemented in a derived class, maps the given source element to its corresponding target element.
        /// </summary>
        /// <param name="source">The source element to be mapped.</param>
        /// <returns>The mapped target element.</returns>
        protected abstract TTarget Map(TSource source);

        /// <summary>
        /// When implemented in a derived class, determines whether the given target element 
        /// is the exact match of the given source element according to the used map.
        /// <para>If it is impossible to determine a match with an absolute confidence,
        /// the implementation should return <see langword="false"/>.</para>
        /// </summary>
        /// <param name="target">The target element to check.</param>
        /// <param name="source">The source element to check.</param>
        /// <returns><see langword="true"/> if the target element is the exact mapped match of the source element; otherwise, <see langword="false"/>.</returns>
        protected abstract bool IsExactMapping(TTarget target, TSource source);

        public IEnumerator<TTarget> GetEnumerator()
        {
            int originalVersion = _version;
            int count = _targetItems.Count;

            for (int i = 0; i != count; ++i)
            {
                _locker.EnterReadLock();
                try
                {
                    if (_version != originalVersion)
                    {
                        throw new Exception("The collection has been modified.");
                    }

                    yield return _targetItems[i];
                }
                finally
                {
                    _locker.ExitReadLock();
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => (this as IEnumerable<TTarget>).GetEnumerator();

        private void RaiseChangeNotificationEvents(NotifyCollectionChangedEventArgs e)
        {
            if (e.Action != NotifyCollectionChangedAction.Move &&
                e.Action != NotifyCollectionChangedAction.Replace)
            {
                _propertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Count)));
            }

            _collectionChanged?.Invoke(this, e);
        }

        private void OnSourceListChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (!IsDisposed)
            {
                Synchronize();
            }
        }

        protected void Synchronize()
        {
            _locker.EnterUpgradeableReadLock();

            try
            {
                if (_targetItems.Count == 0)
                {
                    int sourceCount = _sourceItems.Count;
                    if (sourceCount == 0)
                    {
                        return;
                    }

                    // Populate the target list directly from mapped source items.
                    List<TTarget> newItems = new List<TTarget>(sourceCount);
                    for (int i = 0; i != sourceCount; ++i)
                    {
                        newItems.Add(Map(_sourceItems[i]));
                    }

                    _locker.EnterWriteLock();
                    try
                    {
                        unchecked { ++_version; }
                        _targetItems.AddRange(newItems);
                    }
                    finally
                    {
                        _locker.ExitWriteLock();
                    }

                    RaiseChangeNotificationEvents(
                        new NotifyCollectionChangedEventArgs(
                            NotifyCollectionChangedAction.Add, newItems, 0));
                }
                else if (_sourceItems.Count == 0)
                {
                    _locker.EnterWriteLock();
                    try
                    {
                        unchecked { ++_version; }
                        _targetItems.Clear();
                    }
                    finally
                    {
                        _locker.ExitWriteLock();
                    }

                    RaiseChangeNotificationEvents(
                        new NotifyCollectionChangedEventArgs(
                            NotifyCollectionChangedAction.Reset));
                }
                else
                {
                    List<TTarget> addedItems = null, removedItems = null;
                    int alignedCount = Math.Min(_sourceItems.Count, _targetItems.Count);
                    int countDifference = _targetItems.Count - _sourceItems.Count;
                    bool wasWriteLockEntered = false;

                    void EnsureInsideWriteLock()
                    {
                        if (!wasWriteLockEntered)
                        {
                            _locker.EnterWriteLock();
                            wasWriteLockEntered = true;
                        }
                    }

                    // Synchronize the lists within their common length.
                    try
                    {
                        for (int i = 0; i != alignedCount; ++i)
                        {
                            TSource source = _sourceItems[i];
                            TTarget target = _targetItems[i];

                            if (IsExactMapping(target, source))
                            {
                                continue;
                            }

                            EnsureInsideWriteLock();
                            unchecked { ++_version; }

                            if (removedItems == null)
                            {
                                removedItems = new List<TTarget>(Math.Max(_targetItems.Count - _sourceItems.Count + 1, 4));
                            }
                            removedItems.Add(target);

                            target = Map(source);
                            _targetItems[i] = target;

                            if (addedItems == null)
                            {
                                addedItems = new List<TTarget>(Math.Max(_sourceItems.Count - _targetItems.Count + 1, 4));
                            }
                            addedItems.Add(target);
                        }

                        // Synchronize the two lists beyond their common length.
                        if (countDifference != 0)
                        {
                            EnsureInsideWriteLock();
                            unchecked { ++_version; }

                            if (countDifference > 0)
                            {
                                if (removedItems == null)
                                {
                                    removedItems = new List<TTarget>(countDifference);
                                }

                                for (int i = countDifference; i != 0; --i)
                                {
                                    removedItems.Add(_targetItems[_targetItems.Count - i]);
                                }

                                removedItems.RemoveRange(_sourceItems.Count, countDifference);
                            }
                            else
                            {
                                countDifference = -countDifference;
                                if (addedItems == null)
                                {
                                    addedItems = new List<TTarget>(countDifference);
                                }

                                for (int i = 0; i != countDifference; ++i)
                                {
                                    TTarget target = Map(_sourceItems[_targetItems.Count + 1]);

                                    _targetItems.Add(target);
                                    addedItems.Add(target);
                                }
                            }
                        }
                    }
                    finally
                    {
                        if (wasWriteLockEntered)
                        {
                            _locker.ExitWriteLock();
                        }
                    }

                    // Raise change notification events if needed.
                    if (addedItems != null && addedItems.Count != 0)
                    {
                        if (removedItems != null && removedItems.Count != 0)
                        {
                            RaiseChangeNotificationEvents(
                                new NotifyCollectionChangedEventArgs(
                                    NotifyCollectionChangedAction.Reset));
                        }
                        else
                        {
                            RaiseChangeNotificationEvents(
                                new NotifyCollectionChangedEventArgs(
                                    NotifyCollectionChangedAction.Add, addedItems, _targetItems.Count - countDifference));
                        }
                    }
                    else if (removedItems != null && removedItems.Count != 0)
                    {
                        RaiseChangeNotificationEvents(
                            new NotifyCollectionChangedEventArgs(
                                NotifyCollectionChangedAction.Remove, removedItems, _targetItems.Count));
                    }
                }
            }
            finally
            {
                _locker.ExitUpgradeableReadLock();
            }
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
                _sourceItems.CollectionChanged -= OnSourceListChanged;
                _propertyChanged = null;
                _collectionChanged = null;
                _locker.Dispose();
            }
        }

        #endregion

        #region Destructor

        ~ReadOnlyObservableMappedList()
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