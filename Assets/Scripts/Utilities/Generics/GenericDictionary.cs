using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Architect.Utils {

    public class GenericDictionary {
        private Dictionary<object, object> dictionary;

        #region Constructor
        public GenericDictionary(int aAmount = 0) {
            dictionary = new Dictionary<object, object>(aAmount);
        }
        #endregion

        #region Adding
        public void Add<T>(object aKey, T aValue) where T : Component {
            dictionary[aKey] = aValue;
        }
        #endregion

        #region Removal
        public void Remove<T>(object aKey) where T : Component {
            dictionary.Remove(aKey);
        }
        #endregion

        #region Retrieval
        public T Get<T>(object aKey) where T : Component {
            return dictionary[aKey] as T;
        }
        #endregion

    }

}

