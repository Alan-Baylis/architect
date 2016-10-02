using UnityEngine;
using System.Collections;

namespace Resource.Utils {

    /// <summary>
    /// Create a Singleton monobehaviour
    /// </summary>
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour {
        private static T instance;
        private static object accessLock = new object();

        private static bool applicationIsClosing = false;

        protected Singleton() { }

        #region Getters & Setters
        public static T Instance {
            get {
                if (applicationIsClosing) {
                    Debug.LogWarning(string.Format("Application is closing [Singleton] '{0}' had been destroyed and is no longer accessible"));
                    return null;
                }

                lock (accessLock) {
                    if (instance == null) {
                        T[] instances = FindObjectsOfType<T>();

                        if (instances.Length > 0) {
                            instance = instances[0];

                            if (instances.Length > 1) {
                                Debug.LogError(string.Format("Application contains more than one (1) instance of [Singleton] '{0}'. Reopen scene or stop creating multiple instances to correct the issue", instance));
                            }
                        } else {
                            GameObject singleton = new GameObject();
                            instance = singleton.AddComponent<T>();

                            singleton.name = string.Format("(Singleton) {0}", typeof(T));

                            DontDestroyOnLoad(singleton);

                            Debug.Log(string.Format("[Singleton] instance of '{0}' was created", typeof(T)));
                        }
                    }

                    return instance;
                }
            }
        }
        #endregion

        #region Utility Functions
        private void OnDestroy() {
            applicationIsClosing = true;
        }
        #endregion

    }

}
