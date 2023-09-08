using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

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
        #region Instance members

        private readonly ReadOnlyDictionary<TKey, TValue> m_Dictionary;

        public IEnumerable<TKey> Keys => m_Dictionary.Keys;

        public IEnumerable<TValue> Values => m_Dictionary.Values;

        public int Count => m_Dictionary.Count;

        private bool m_IsDisposed;

        #endregion

        #region Instance indexers

        public TValue this[TKey key] => m_Dictionary[key];

        #endregion

        #region Instance events

#pragma warning disable IDE0052
        // The event is only formal and never actually raised.
        private PropertyChangedEventHandler m_PropertyChanged;
        public event PropertyChangedEventHandler PropertyChanged
        {
            add
            {
                if (!m_IsDisposed)
                    m_PropertyChanged += value;
            }
            remove
            {
                m_PropertyChanged -= value;
            }
        }

        // The event is only formal and never actually raised.
        private NotifyCollectionChangedEventHandler m_CollectionChanged;
        public event NotifyCollectionChangedEventHandler CollectionChanged
        {
            add
            {
                if (!m_IsDisposed)
                    m_CollectionChanged += value;
            }
            remove
            {
                m_CollectionChanged -= value;
            }
        }
#pragma warning restore IDE0052

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

            m_Dictionary = new ReadOnlyDictionary<TKey, TValue>(temp);
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

            m_Dictionary = new ReadOnlyDictionary<TKey, TValue>(temp);
        }

        public ReadOnlyObservableDictionary(IDictionary<TKey, TValue> source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            m_Dictionary = new ReadOnlyDictionary<TKey, TValue>(source);
        }

        #endregion

        #region Instance methods

        public bool ContainsKey(TKey key) => m_Dictionary.ContainsKey(key);

        public bool TryGetValue(TKey key, out TValue value) => m_Dictionary.TryGetValue(key, out value);

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => m_Dictionary.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => m_Dictionary.GetEnumerator();

        public void Dispose()
        {
            if (!m_IsDisposed)
            {
                m_PropertyChanged = null;
                m_CollectionChanged = null;
                m_IsDisposed = true;
            }
        }

        #endregion
    }
}
