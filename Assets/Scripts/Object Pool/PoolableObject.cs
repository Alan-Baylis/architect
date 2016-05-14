using UnityEngine;
using System.Collections;

namespace Architect.Pooling {

    public class PoolableObject : MonoBehaviour {
        private ObjectPool containingPool;

        #region Initialization
        public virtual void Initialize(ObjectPool aObjectPool) {
            containingPool = aObjectPool;
        }
        #endregion

        #region Cleanup
        public virtual void Repool() {
            if (containingPool != null) {
                containingPool.Return(gameObject);
            } else {
                Destroy(gameObject);
            }            
        }
        #endregion

    }

}
