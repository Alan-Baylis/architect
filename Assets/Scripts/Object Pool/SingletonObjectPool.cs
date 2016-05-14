using UnityEngine;
using System.Collections;

namespace Architect.Pooling {

    public class SingletonObjectPool : ObjectPool {

        #region Initialize
        public override void Initialize(GameObject aPrefab, int aAmount = 1) {
            base.Initialize(aPrefab, aAmount);

            DontDestroyOnLoad(gameObject);
        }

        public override void Initialize<T>(GameObject aPrefab, int aAmount = 1) {
            base.Initialize<T>(aPrefab, aAmount);

            DontDestroyOnLoad(gameObject);
        }
        #endregion

        #region Creation
        protected override GameObject CreateObject<T>() {
            GameObject newObject = base.CreateObject<T>();
            DontDestroyOnLoad(newObject);

            return newObject;
        }
        #endregion

    }

}
