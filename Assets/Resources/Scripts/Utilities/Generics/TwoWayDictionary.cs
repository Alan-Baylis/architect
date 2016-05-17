using UnityEngine;
using System.Collections.Generic;

namespace Resources.Utils {

    /// <summary>
    /// TwTwo-Wayictionary Class
    /// </summary>
    /// <typeparam name="K">Key</typeparam>
    /// <typeparam name="V">Value</typeparam>
    public class TwoWayDictionary<K, V> : GenericDictionary<K, V> {
        private Dictionary<V, K> dictionaryReverse;

        #region Constructor
        public TwoWayDictionary(int aAmount) {
            dictionary = new Dictionary<K, V>(aAmount);
            dictionaryReverse = new Dictionary<V, K>(aAmount);
        }
        #endregion

        #region Base Accessor
        public override V this[K aKey] {
            get {
                return base[aKey];


            }

            set {
                base[aKey] = value;
            }
        }
        #endregion

        #region Core Functions
        public override void Add(K aKey, V aValue) {
            dictionary[aKey] = aValue;
            dictionaryReverse[aValue] = aKey;
        }

        /// <summary>
        /// Remove the key-value pair from the dictionary based on the given key
        /// </summary>
        public override void Remove(K aKey) {
            V value = dictionary[aKey];

            dictionary.Remove(aKey);
            dictionaryReverse.Remove(value);
        }

        /// <summary>
        /// Remove key-value pair from the dictionary based on the given value
        /// </summary>
        public void Remove(V aValue) {
            K key = dictionaryReverse[aValue];

            dictionary.Remove(key);
            dictionaryReverse.Remove(aValue);
        }

        /// <summary>
        /// Get the key for the given value if it exists in the dictionary
        /// </summary>
        public bool TryGetKey(V aValue, out K aKey) {
            return dictionaryReverse.TryGetValue(aValue, out aKey);
        }
        #endregion

    }

}
