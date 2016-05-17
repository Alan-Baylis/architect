using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Resources.Properties;
using Resources.Utils;

namespace Resources.Pooling {

    public class ObjectPool : MonoBehaviour {
        [SerializeField]
        private GameObject prefab;
        [SerializeField]
        private int size = 1;
        [SerializeField, ComponentPopup(typeof(Component)), Tooltip("If set the ObjectPool will create a map between its associated objects and the given component")]
        private string component;
        [SerializeField, Range(0, 1)]
        private bool sucks;

        private bool initialized = false;
        private Stack<GameObject> pooledObjects;
        private Dictionary<GameObject, Component> componentMap;

        #region Getters & Setters
        public int Count {
            get { return pooledObjects.Count; }
        }
        #endregion

        #region Initialization
        /// <summary>
        /// If this class is added as a Component via the Inspector then allow it to initialize itself
        /// </summary>
        private void Start() {
            if (prefab != null) {
                if (string.IsNullOrEmpty(component) == false) {
                    System.Type type = System.Type.GetType(component);
                    InitializeWithComponent(prefab, type, size);
                } else {
                    Initialize(prefab, size);
                }
            }
        }

        /// <summary>
        /// Initialize and setup a pool with Gameobjects
        /// </summary>
        public virtual void Initialize(GameObject aPrefab, int aAmount = 1) {
            if (initialized == false) {
                initialized = true;

                pooledObjects = new Stack<GameObject>();

                prefab = aPrefab;

                ObjectPoolManager.Instance.Add(prefab, this);

                for (int i = 0; i < aAmount; i++) {
                    CreateObject();
                }
            } else {
                Debug.LogWarning(string.Format("ObjectPool '{0}' already initialized. Avoid initializing pools more than once!", this.gameObject));
            }
        }

        /// <summary>
        /// Initialize and setup a pool with GameObjects mapped to the component 'PoolableObject'
        /// </summary>
        public virtual void InitializeWithComponent(GameObject aPrefab, int aAmount = 1) {
            InitializeWithComponent<PoolableObject>(aPrefab, aAmount);
        }

        /// <summary>
        /// Initialize and setup a pool with GameObjects mapped to the given component
        /// </summary>
        public virtual void InitializeWithComponent<T>(GameObject aPrefab, int aAmount = 1) where T : Component {
            if (initialized == false) {
                initialized = true;

                pooledObjects = new Stack<GameObject>(aAmount);
                componentMap = new Dictionary<GameObject, Component>(aAmount);

                prefab = aPrefab;

                ObjectPoolManager.Instance.Add(prefab, this);

                for (int i = 0; i < aAmount; i++) {
                    CreateObject<T>();
                }
            } else {
                Debug.LogWarning(string.Format("ObjectPool '{0}' already initialized. Avoid initializing pools more than once!", this.gameObject));
            }
        }
        #endregion

        /// <summary>
        /// Initialize and setup a pool with GameObjects mapped to the given component
        /// </summary>
        public virtual void InitializeWithComponent(GameObject aPrefab, System.Type aType, int aAmount = 1) {
            if (initialized == false) {
                initialized = true;

                pooledObjects = new Stack<GameObject>(aAmount);
                componentMap = new Dictionary<GameObject, Component>(aAmount);

                prefab = aPrefab;

                ObjectPoolManager.Instance.Add(prefab, this);

                for (int i = 0; i < aAmount; i++) {
                    CreateObject(aType);
                }
            } else {
                Debug.LogWarning(string.Format("ObjectPool '{0}' already initialized. Avoid initializing pools more than once!", this.gameObject));
            }
        }

        #region Creation
        /// <summary>
        /// Create a new GameObject for the pool
        /// </summary>
        protected virtual GameObject CreateObject() {
            GameObject newObject = Instantiate(prefab);
            newObject.SetActive(false);
            newObject.transform.SetParent(this.transform, false);

            // Initialize the poolable object
            PoolableObject newPoolableObject = newObject.GetComponent<PoolableObject>();
            if (newPoolableObject != null) {
                newPoolableObject.Initialize(this);
            } else {
                Debug.LogError(string.Format("Prefab '{0}' does not contain component '{1}'", prefab, typeof(PoolableObject)));
            }

            pooledObjects.Push(newObject);

            return newObject;
        }

        /// <summary>
        /// Create a new GameObject for the pool and map it with the given component
        /// </summary>
        protected virtual GameObject CreateObject<T>() where T : Component {
            GameObject newObject = Instantiate(prefab);
            newObject.SetActive(false);
            newObject.transform.SetParent(this.transform, false);
            
            // Initialize the poolable object
            PoolableObject newPoolableObject = newObject.GetComponent<PoolableObject>();
            if (newPoolableObject != null) {
                newPoolableObject.Initialize(this);
            } else {
                Debug.LogError(string.Format("Prefab '{0}' does not contain component '{1}'", prefab, typeof(PoolableObject)));
            }

            // Setup the component mapping based on the given component type
            if (typeof(PoolableObject) == typeof(T)) {
                componentMap.Add(newObject, newPoolableObject);
            } else {
                T newObjectComponent = newObject.GetComponent<T>();
                if (newObjectComponent != null) {
                    componentMap.Add(newObject, newObjectComponent);
                } else {
                    Debug.LogError(string.Format("Prefab '{0}' does not contain component '{1}'", prefab, typeof(T)));
                }
            }

            pooledObjects.Push(newObject);

            return newObject;
        }

        /// <summary>
        /// Create a new GameObject for the pool and map it with the given component
        /// </summary>
        protected virtual GameObject CreateObject(System.Type aType) {
            GameObject newObject = Instantiate(prefab);
            newObject.SetActive(false);
            newObject.transform.SetParent(this.transform, false);

            // Initialize the poolable object
            PoolableObject newPoolableObject = newObject.GetComponent<PoolableObject>();
            if (newPoolableObject != null) {
                newPoolableObject.Initialize(this);
            } else {
                Debug.LogError(string.Format("Prefab '{0}' does not contain component '{1}'", prefab, typeof(PoolableObject)));
            }

            // Setup the component mapping based on the given component type
            if (typeof(PoolableObject) == aType) {
                componentMap.Add(newObject, newPoolableObject);
            } else {
                if (newObject.GetComponent(aType) != null) {
                    componentMap.Add(newObject, newObject.GetComponent(aType));
                } else {
                    Debug.LogError(string.Format("Prefab '{0}' does not contain component '{1}'", prefab, aType));
                }
            }

            pooledObjects.Push(newObject);

            return newObject;
        }
        #endregion

        #region Get Functions
        /// <summary>
        /// Get the next GameObject from the pool. If the pool is empty a new object will be created.
        /// </summary>
        public GameObject GetObject() {
            if (pooledObjects.Count == 0) {
                CreateObject();
            }

            GameObject freedObject = pooledObjects.Pop();
            freedObject.transform.SetParent(null, false);
            freedObject.SetActive(true);

            return freedObject;
        }

        /// <summary>
        /// Get the next GameObject from the pool. If the pool is empty a new object will be created.
        /// </summary>
        public GameObject GetObject<T>() where T : Component {
            if (pooledObjects.Count == 0) {
                CreateObject<T>();
            }

            GameObject freedObject = pooledObjects.Pop();
            freedObject.transform.SetParent(null, false);
            freedObject.SetActive(true);

            return freedObject;
        }

        /// <summary>
        /// Get the next GameObject and return its stored Component from the pool. If the pool is empty a new object will be created.
        /// </summary>
        public T GetObjectComponent<T>() where T : Component {
            return GetObjectComponent<T>(GetObject<T>());
        }

        /// <summary>
        /// Get the given GameObject's stored component.
        /// </summary>
        public T GetObjectComponent<T>(GameObject aObject) where T : Component {
            return componentMap[aObject] as T;
        }
        #endregion

        #region Return Functions
        /// <summary>
        /// Return the GameObject to the pool so it can be re-used again
        /// </summary>
        public void Return(GameObject aObject) {
            if (pooledObjects.Contains(aObject) == false) {
                aObject.transform.SetParent(this.transform, false);
                aObject.SetActive(false);
                
                pooledObjects.Push(aObject);
            }
        }
        #endregion

    }

}
