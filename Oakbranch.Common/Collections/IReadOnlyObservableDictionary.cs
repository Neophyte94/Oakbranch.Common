using System.Collections.Generic;

namespace Oakbranch.Common.Collections
{
    /// <summary>
    /// Represents an abstraction for a read-only collection of key-value pairs that allows tracking changes to it.
    /// </summary>
    /// <typeparam name="TKey">The type of keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of values in the dictionary.</typeparam>
    public interface IReadOnlyObservableDictionary<TKey, TValue> :
        IReadOnlyObservableCollection<KeyValuePair<TKey, TValue>>,
        IReadOnlyDictionary<TKey, TValue>
    { }
}