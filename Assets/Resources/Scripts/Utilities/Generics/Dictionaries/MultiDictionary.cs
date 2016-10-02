using System.Collections.Generic;

namespace Resource.Utils {

    /// <summary>
    /// Multi-Dictionary Class
    /// </summary>
    /// <typeparam name="K">Key</typeparam>
    /// <typeparam name="V">Value</typeparam>
    public class MultiDictionary<K, V> {
        private Dictionary<K, List<V>> dictionary;

        #region Constructor
        public MultiDictionary(int aAmount = 0) {
            dictionary = new Dictionary<K, List<V>>(aAmount);
        }
        #endregion

        #region Base Accessor
        public List<V> this[K aKey] {
            get {
                List<V> item;
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
        /// <summary>
        /// Add a single value to the list of values for the given key
        /// </summary>
        public void Add(K aKey, V aValue) {
            List<V> values;

            if (dictionary.TryGetValue(aKey, out values) == false) {
                values = new List<V>();
            }

            values.Add(aValue);
            dictionary[aKey] = values;
        }

        /// <summary>
        /// Add a group amount of values to the list of values for the given key
        /// </summary>
        public void Add(K aKey, params V[] aValues) {
            List<V> values;

            if (dictionary.TryGetValue(aKey, out values) == false) {
                values = new List<V>();
                dictionary[aKey] = values;
            }

            foreach (V value in aValues) {
                values.Add(value);
            }
        }

        /// <summary>
        /// Remove an entire key-value pair based on the given key
        /// </summary>
        public void Remove(K aKey) {
            dictionary.Remove(aKey);
        }

        /// <summary>
        /// Remove a single value from the given key
        /// </summary>
        public void Remove(K aKey, V aValue) {
            List<V> values;

            if (dictionary.TryGetValue(aKey, out values)) {
                values.Remove(aValue);
            }
        }

        /// <summary>
        /// Remove a group of values from the given key
        /// </summary>
        public void Remove(K aKey, params V[] aValues) {
            List<V> values;

            if (dictionary.TryGetValue(aKey, out values) == false) {
                values = new List<V>();
                dictionary[aKey] = values;
            }

            foreach (V value in aValues) {
                values.Remove(value);
            }
        }

        public bool TryGetValue(K aKey, out List<V> aValues) {
            aValues = default(List<V>);

            return dictionary.TryGetValue(aKey, out aValues);
        }
        #endregion

        #region Core Utilities
        public Dictionary<K, List<V>>.KeyCollection Keys {
            get {
                return dictionary.Keys;
            }
        }

        public Dictionary<K, List<V>>.ValueCollection Values {
            get {
                return dictionary.Values;
            }
        }

        /// <summary>
        /// Returns the number of key-value pairs contained within the dictionary
        /// </summary>
        public int Count {
            get {
                return dictionary.Count;
            }
        }

        /// <summary>
        /// Returns the number of values for the given key
        /// </summary>
        public int ValueCount(K aKey) {
            List<V> values;

            if (dictionary.TryGetValue(aKey, out values)) {
                return values.Count;
            }

            return 0;
        }

        /// <summary>
        /// Returns whether or not the given key is contained within the dictionaries keys
        /// </summary>
        public bool ContainsKey(K aKey) {
            return dictionary.ContainsKey(aKey);
        }

        /// <summary>
        /// Remove all data from the dictionary
        /// </summary>
        public void Clear() {
            dictionary.Clear();
        }
        #endregion

    }

}
