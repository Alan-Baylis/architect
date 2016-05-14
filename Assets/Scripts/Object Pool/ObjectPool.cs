using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Architect.Utils;

namespace Architect.Pooling {

    public class ObjectPool : MonoBehaviour {
        protected GameObject objectPrefab;

        private Stack<GameObject> pooledObjects;
        private GenericDictionary componentMap;

        #region Getters & Setters
        public int Count {
            get { return pooledObjects.Count; }
        }
        #endregion

        #region Initialization
        /// <summary>
        /// Initialize and setup a pool with GameObjects
        /// </summary>
        public virtual void Initialize(GameObject aPrefab, int aAmount = 1) {
            Initialize<PoolableObject>(aPrefab, aAmount);
        }

        /// <summary>
        /// Initialize and setup a pool with GameObjects as well as store a mapping between these GameObjects and the given component for quick and cheap access.
        /// </summary>
        public virtual void Initialize<T>(GameObject aPrefab, int aAmount = 1) where T : Component {
            pooledObjects = new Stack<GameObject>(aAmount);
            componentMap = new GenericDictionary(aAmount);

            objectPrefab = aPrefab;

            PoolManager.Instance.Add(objectPrefab, this);

            for (int i = 0; i < aAmount; i++) {
                CreateObject<T>();
            }
        }
        #endregion

        #region Creation
        /// <summary>
        /// Create a new GameObject for the pool and store the given component
        /// </summary>
        protected virtual GameObject CreateObject<T>() where T : Component {
            GameObject newObject = Instantiate(objectPrefab);
            newObject.SetActive(false);
            newObject.transform.SetParent(this.transform, false);
            
            // Initialize the poolable object
            PoolableObject newPoolableObject = newObject.GetComponent<PoolableObject>();
            if (newPoolableObject != null) {
                newPoolableObject.Initialize(this);
            } else {
                Debug.LogError(string.Format("Prefab '{0}' does not contain component '{1}'", objectPrefab, typeof(PoolableObject)));
                return null;
            }

            // Setup the component mapping based on the given component type
            if (typeof(PoolableObject) == typeof(T)) {
                componentMap.Add(newObject, newPoolableObject);
            } else {
                T newObjectComponent = newObject.GetComponent<T>();
                if (newObjectComponent != null) {
                    componentMap.Add(newObject, newObjectComponent);
                } else {
                    Debug.LogWarning(string.Format("Prefab '{0}' does not contain component '{1}'", objectPrefab, typeof(T)));
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
            return componentMap.Get<T>(aObject);
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
