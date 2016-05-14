using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Architect.Utils {

    public class MultiDictionary {
        private Dictionary<object, List<object>> dictionary = new Dictionary<object, List<object>>();

        #region Adding
        public void Add<T>(object aKey, T aValue) where T : Component {
            List<object> values;

            if (dictionary.TryGetValue(aKey, out values) == false) {
                values = new List<object>();
            }

            values.Add(aValue);
            dictionary[aKey] = values;
        }
        #endregion

        #region Removal
        public void Remove<T>(object aKey) where T : Component {
            dictionary.Remove(aKey);
        }

        public void RemoveValue<T>(object aKey, T aValue) where T : Component {
            List<object> values;

            if (dictionary.TryGetValue(aKey, out values)) {
                values.Remove(aValue);

                // TODO: Investigate if necessary
                dictionary[aKey] = values;
            }
        }
        #endregion

        #region Retrieval
        public T Get<T>(object aKey) where T : Component {
            return dictionary[aKey] as T;
        }

        public T GetValue<T>(object aKey, T aValue) where T : Component {
            return dictionary[aKey] as T;
        }
        #endregion

    }

}
