using System;
using System.Collections;
using System.Collections.Generic;

namespace Resources.Utils {

    /// <summary>
    /// Generic Dictionary Class
    /// </summary>
    /// <typeparam name="K">Key</typeparam>
    /// <typeparam name="V">Value</typeparam>
    public class GenericDictionary<K, V> : IDictionary<K, V> {
        protected Dictionary<K, V> dictionary;

        #region Constructor
        public GenericDictionary(int aAmount = 0) {
            dictionary = new Dictionary<K, V>(aAmount);
        }
        #endregion

        #region Base Accessor
        public virtual V this[K aKey] {
            get {
                V item;
                if (dictionary.TryGetValue(aKey, out item)) {
                    return item;
                }

                throw new KeyNotFoundException(string.Format("Key '{0}' not found within dictionary.", aKey));
            }

            set {
                dictionary[aKey] = value;
            }
        }
        #endregion

        #region Core Functions
        public virtual void Add(K aKey, V aValue) {
            dictionary[aKey] = aValue;
        }

        public virtual void Remove(K aKey) {
            dictionary.Remove(aKey);
        }

        /// <summary>
        /// Get the value for the given key if it exists in the dictionary
        /// </summary>
        public bool TryGetValue(K aKey, out V aValue) {
            aValue = default(V);

            return dictionary.TryGetValue(aKey, out aValue);
        }
        #endregion

        #region Core Utilities
        public Dictionary<K, V>.KeyCollection Keys {
            get {
                return dictionary.Keys;
            }
        }

        public Dictionary<K, V>.ValueCollection Values {
            get {
                return dictionary.Values;
            }
        }

        /// <summary>
        /// Returns the number of key value pairs contained within the dictionary
        /// </summary>
        public int Count {
            get {
                return dictionary.Count;
            }
        }

        ICollection<K> IDictionary<K, V>.Keys {
            get {
                throw new NotImplementedException();
            }
        }

        ICollection<V> IDictionary<K, V>.Values {
            get {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Returns whether or not the given key is contained within the dictionary's keys
        /// </summary>
        public bool ContainsKey(K aKey) {
            return dictionary.ContainsKey(aKey);
        }

        /// <summary>
        /// Returns whether or not the given value is contained within the dictionary's values
        /// </summary>
        public bool ContainsValue(V aValue) {
            return dictionary.ContainsValue(aValue);
        }

        /// <summary>
        /// Remove all data from the dictionary
        /// </summary>
        public void Clear() {
            dictionary.Clear();
        }

        public bool IsReadOnly {
            get {
                return false;
            }
        }

        // TODO: Learn what to do with this
        bool IDictionary<K, V>.Remove(K key) {
            throw new NotImplementedException();
        }

        // TODO: Learn what to do with this
        public void Add(KeyValuePair<K, V> aItem) {
            throw new NotImplementedException();
        }

        // TODO: Learn what to do with this
        public bool Contains(KeyValuePair<K, V> item) {
            return dictionary.ContainsKey(item.Key);
        }

        // TODO: Learn what to do with this
        public void CopyTo(KeyValuePair<K, V>[] array, int arrayIndex) {
            throw new NotImplementedException();
        }

        // TODO: Learn what to do with this
        public bool Remove(KeyValuePair<K, V> aItem) {
            throw new NotImplementedException();
        }

        // TODO: Learn what to do with this
        // TODO: Learn what to do with this
        public IEnumerator<KeyValuePair<K, V>> GetEnumerator() {
            throw new NotImplementedException();
        }

        // TODO: Learn what to do with this
        IEnumerator IEnumerable.GetEnumerator() {
            throw new NotImplementedException();
        }
        #endregion

    }

}

