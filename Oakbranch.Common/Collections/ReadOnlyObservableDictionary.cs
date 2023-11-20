using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace Oakbranch.Common.Collections
{
    /// <summary>
    /// Represents an immutable collection of key/value pairs that can formally be observed.
    /// <para>Implements <see cref="IReadOnlyObservableDictionary{TKey, TValue}"/> and <see cref="IDisposable"/>.</para>
    /// </summary>
    /// <typeparam name="TKey">The type of keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of values in the dictionary.</typeparam>
    public sealed class ReadOnlyObservableDictionary<TKey, TValue> : IReadOnlyObservableDictionary<TKey, TValue>, IDisposable
    {
        #region Instance props & fields

        private readonly ReadOnlyDictionary<TKey, TValue> _dictionary;

        public IEnumerable<TKey> Keys => _dictionary.Keys;

        public IEnumerable<TValue> Values => _dictionary.Values;

        public int Count => _dictionary.Count;

        private bool _isDisposed;

        #endregion

        #region Instance indexers

        public TValue this[TKey key] => _dictionary[key];

        #endregion

        #region Instance events

        [SuppressMessage("CodeQuality", "IDE0052:Remove unread private members", Justification = "Formal implementation")]
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

        [SuppressMessage("CodeQuality", "IDE0052:Remove unread private members", Justification = "Formal implementation")]
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

        public ReadOnlyObservableDictionary(IEnumerable<TValue> values, Func<TValue, TKey> selector)
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            int count = -1;
            if (values is ICollection ic) { count = ic.Count; }
            else if (values is TValue[] array) { count = array.Length; }

            Dictionary<TKey, TValue> temp = 
                count == -1 ? new Dictionary<TKey, TValue>() : new Dictionary<TKey, TValue>(count);
            foreach (TValue val in values)
            {
                temp.Add(selector(val), val);
            }

            _dictionary = new ReadOnlyDictionary<TKey, TValue>(temp);
        }

        public ReadOnlyObservableDictionary(IEnumerable<KeyValuePair<TKey, TValue>> pairs)
        {
            if (pairs == null)
                throw new ArgumentNullException(nameof(pairs));

            int count = -1;
            if (pairs is ICollection ic) { count = ic.Count; }
            else if (pairs is KeyValuePair<TKey, TValue>[] array) { count = array.Length; }

            Dictionary<TKey, TValue> temp =
                count == -1 ? new Dictionary<TKey, TValue>() : new Dictionary<TKey, TValue>(count);
            foreach (var pair in pairs)
            {
                temp.Add(pair.Key, pair.Value);
            }

            _dictionary = new ReadOnlyDictionary<TKey, TValue>(temp);
        }

        public ReadOnlyObservableDictionary(IDictionary<TKey, TValue> source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            _dictionary = new ReadOnlyDictionary<TKey, TValue>(source);
        }

        #endregion

        #region Instance methods

        public bool ContainsKey(TKey key) => _dictionary.ContainsKey(key);

        public bool TryGetValue(TKey key, out TValue value) => _dictionary.TryGetValue(key, out value);

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => _dictionary.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _dictionary.GetEnumerator();

        private void ThrowIfDisposed()
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool releaseManaged)
        {
            if (_isDisposed) return;

            if (releaseManaged)
            {
                _propertyChanged = null;
                _collectionChanged = null;
            }

            _isDisposed = true;
        }

        #endregion

        #region Destructor

        ~ReadOnlyObservableDictionary()
        {
            Dispose(false);
        }

        #endregion
    }
}
