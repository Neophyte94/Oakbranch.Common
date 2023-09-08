using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Text;

namespace Oakbranch.Common.Collections
{
    public sealed class ReadOnlyObservableDictionary<TKey, TValue> : IReadOnlyObservableDictionary<TKey, TValue>, IDisposable
    {
        private readonly ReadOnlyDictionary<TKey, TValue> m_Dictionary;

        public TValue this[TKey key] => m_Dictionary[key];

        public IEnumerable<TKey> Keys => m_Dictionary.Keys;

        public IEnumerable<TValue> Values => m_Dictionary.Values;

        public int Count => m_Dictionary.Count;

        public event PropertyChangedEventHandler PropertyChanged;
        public event NotifyCollectionChangedEventHandler CollectionChanged;

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

        public bool ContainsKey(TKey key) => m_Dictionary.ContainsKey(key);

        public bool TryGetValue(TKey key, out TValue value) => m_Dictionary.TryGetValue(key, out value);

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => m_Dictionary.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => m_Dictionary.GetEnumerator();

        public void Dispose()
        {
            PropertyChanged = null;
            CollectionChanged = null;
        }
    }
}
