using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Architect.Pooling {

    /// <summary>
    /// Manager to handle and contain all of the pools of the given type
    /// - Syncronized Add, Get, and Remove
    /// </summary>
    public class PoolManager {
        private static PoolManager instance;

        private Dictionary<string, ObjectPool> pools = new Dictionary<string, ObjectPool>();
        private readonly Object poolsLock = new Object();

        #region Getters & Setters
        public static PoolManager Instance {
            get {
                if (instance == null) {
                    instance = new PoolManager();
                }

                return instance;
            }
        }
        #endregion

        #region Pools
        /// <summary>
        /// Add an Object Pool to the map of pools using the GameObject's name as a key
        /// </summary>
        public void Add(GameObject aObject, ObjectPool aObjectPool) {
            Add(aObject.name, aObjectPool);
        }

        /// <summary>
        /// Add an Object Pool to the map of pools using the string as a key
        /// </summary>
        public void Add(string aKey, ObjectPool aObjectPool) {
            lock (poolsLock) {
                pools[aKey] = aObjectPool;
            }
        }

        /// <summary>
        /// Get the pool attached to the GameObject's name (key)
        /// </summary>
        public ObjectPool Get(GameObject aObject) {
            return Get(aObject.name);
        }

        /// <summary>
        /// Get the pool associated with the given key
        /// </summary>
        public ObjectPool Get(string aKey) {
            lock (poolsLock) {
                ObjectPool pool;

                if (pools.TryGetValue(aKey, out pool) == false) {
                    Debug.LogWarning(string.Format("Pool with key '{0}' does not exist within the pools mapping", aKey));
                }

                return pool;
            }
        }

        /// <summary>
        /// Remove the pool associated to the GameObject's name (key)
        /// </summary>
        public void Remove(GameObject aObject) {
            Remove(aObject.name);
        }

        /// <summary>
        /// Remove the pool associated with the given key
        /// </summary>
        public void Remove(string aKey) {
            lock (poolsLock) {
                pools.Remove(aKey);
            }
        }
        #endregion

        #region Utility Functions
        public void Reset() {
 
        }

        public void Clear() {
            foreach (ObjectPool pool in pools.Values) {
                GameObject.Destroy(pool.gameObject);
            }

            pools.Clear();
        }
        #endregion

    }

}
