using UnityEngine;
using System.Collections.Generic;

namespace Resource.Pooling {

    /// <summary>
    /// Manager to handle and contain all of the pools of the given type
    /// - Syncronized Add, Get, and Remove
    /// </summary>
    public class ObjectPoolManager {
        private static ObjectPoolManager instance;

        private Dictionary<string, ObjectPool> pools = new Dictionary<string, ObjectPool>();

        #region Getters & Setters
        public static ObjectPoolManager Instance {
            get {
                if (instance == null) {
                    instance = new ObjectPoolManager();
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
            pools[aObject.name] = aObjectPool;
        }

        /// <summary>
        /// Add an Object Pool to the map of pools using the string as a key
        /// </summary>
        public void Add(string aKey, ObjectPool aObjectPool) {
            pools[aKey] = aObjectPool;
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
            ObjectPool pool;

            if (pools.TryGetValue(aKey, out pool) == false) {
                Debug.LogWarning(string.Format("Pool with key '{0}' does not exist within the pools mapping", aKey));
            }

            return pool;
        }

        /// <summary>
        /// Remove the pool associated to the GameObject's name (key)
        /// </summary>
        public void Remove(GameObject aObject) {
            pools.Remove(aObject.name);
        }

        /// <summary>
        /// Remove the pool associated with the given key
        /// </summary>
        public void Remove(string aKey) {
            pools.Remove(aKey);
        }
        #endregion

        #region Utility Functions
        public void Reset() {
 
        }

        public void Clear() {
            foreach (ObjectPool pool in pools.Values) {
                if (pool.CanDestroy) {
                    GameObject.Destroy(pool.gameObject);
                }
            }
        }
        #endregion

    }

}
