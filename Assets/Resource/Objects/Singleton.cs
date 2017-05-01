using Resource.Utils;
using UnityEngine;

namespace Resource {

    /// <summary>
    /// Create a Singleton monobehaviour.
    /// To load this as a prefab when one isn't found in the scene the prefab must be stored in the 'Resources' directory of the Unity project
    /// </summary>
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour {
		private static T instance;
		private static object accessLock = new object();

		private static bool applicationIsClosing = false;

		protected Singleton() { }

		#region Initialization
		protected virtual void Awake() {
            Initialize();
        }

        public virtual void Initialize() {
            if (instance != null && instance != this) {
                DestroyImmediate(gameObject);
                return;
            }
        }
		#endregion

		#region Getters & Setters
		public static T Instance {
			get {
				if (applicationIsClosing) {
					Debug.LogWarning("Application is closing [Singleton] had been destroyed and is no longer accessible");
					return null;
				}

				lock (accessLock) {
					if (instance == null) {
						T[] instances = FindObjectsOfType<T>();

						if (instances.Length > 0) {
							instance = instances[0];

							if (instances.Length > 1) {
								throw new System.Exception(string.Format("Application contains more than one (1) instance of [Singleton] '{0}'. Reopen scene or stop creating multiple instances to correct the issue", instance));
							}
						} else {
							GameObject singleton = Resources.Load<GameObject>("Prefabs/" + typeof(T).Name);
							if (singleton == null) {
								singleton = Resources.Load<GameObject>("Prefabs/" + StringUtils.ToSpacedFormat(typeof(T).Name));
							}

							if (singleton != null) {
								singleton = Instantiate(singleton);
								instance = singleton.GetComponent<T>();

								Debug.Log(string.Format("[Singleton] instance of '{0}' was loaded from resources", typeof(T).Name));
							} else {
								singleton = new GameObject();
								instance = singleton.AddComponent<T>();

								singleton.name = string.Format("(Singleton) {0}", typeof(T).Name);

								Debug.Log(string.Format("[Singleton] instance of '{0}' was created", typeof(T).Name));
							}

							singleton.SetActive(true);

                            DontDestroyOnLoad(singleton);
                        }
					}

					return instance;
				}
			}
		}

        public static T EditorInstance {
            get {
                if (applicationIsClosing) {
                    Debug.LogWarning("Application is closing [Singleton] had been destroyed and is no longer accessible");
                    return null;
                }

                lock (accessLock) {
                    if (instance == null) {
                        T[] instances = FindObjectsOfType<T>();

                        if (instances.Length > 0) {
                            instance = instances[0];

                            if (instances.Length > 1) {
                                throw new System.Exception(string.Format("Application contains more than one (1) instance of [Singleton] '{0}'. Reopen scene or stop creating multiple instances to correct the issue", instance));
                            }
                        } else {
                            GameObject singleton = Resources.Load<GameObject>("Prefabs/" + typeof(T).Name);
                            if (singleton == null) {
                                singleton = Resources.Load<GameObject>("Prefabs/" + StringUtils.ToSpacedFormat(typeof(T).Name));
                            }

                            if (singleton != null) {
                                singleton = Instantiate(singleton);
                                instance = singleton.GetComponent<T>();

                                Debug.Log(string.Format("[Singleton] instance of '{0}' was loaded from resources", typeof(T).Name));
                            } else {
                                singleton = new GameObject();
                                instance = singleton.AddComponent<T>();

                                singleton.name = string.Format("(Singleton) {0}", typeof(T).Name);

                                Debug.Log(string.Format("[Singleton] instance of '{0}' was created", typeof(T).Name));
                            }

                            singleton.SetActive(true);
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
