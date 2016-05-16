using UnityEngine;
using System.Collections;

namespace Resources.Pooling {

    public class SingletonObjectPool : ObjectPool {

        #region Initialize
        public override void Initialize(GameObject aPrefab, int aAmount = 1) {
            base.Initialize(aPrefab, aAmount);

            DontDestroyOnLoad(gameObject);
        }

        public override void InitializeWithComponent(GameObject aPrefab, int aAmount = 1) {
            base.InitializeWithComponent(aPrefab, aAmount);

            DontDestroyOnLoad(gameObject);
        }

        public override void InitializeWithComponent<T>(GameObject aPrefab, int aAmount = 1) {
            base.InitializeWithComponent<T>(aPrefab, aAmount);

            DontDestroyOnLoad(gameObject);
        }
        #endregion

        #region Creation
        protected override GameObject CreateObject() {
            GameObject newObject = base.CreateObject();
            DontDestroyOnLoad(newObject);

            return newObject;
        }

        protected override GameObject CreateObject<T>() {
            GameObject newObject = base.CreateObject<T>();
            DontDestroyOnLoad(newObject);

            return newObject;
        }
        #endregion

    }

}
