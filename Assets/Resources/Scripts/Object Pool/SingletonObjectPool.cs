using UnityEngine;

namespace Resource.Pooling {

    public class SingletonObjectPool : ObjectPool {

        #region Initialize
        public override void Initialize(GameObject aPrefab, int aAmount = 1) {
            base.Initialize(aPrefab, aAmount);

            canDestroy = true;
            DontDestroyOnLoad(gameObject);
        }

        public override void InitializeWithComponent<T>(GameObject aPrefab, int aAmount = 1) {
            base.InitializeWithComponent<T>(aPrefab, aAmount);

            canDestroy = true;
            DontDestroyOnLoad(gameObject);
        }

        public override void InitializeWithComponent(GameObject aPrefab, System.Type aType, int aAmount = 1) {
            base.InitializeWithComponent(aPrefab, aType, aAmount);

            canDestroy = true;
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

        protected override GameObject CreateObject(System.Type aType) {
            GameObject newObject = base.CreateObject(aType);
            DontDestroyOnLoad(newObject);

            return newObject;
        }
        #endregion

    }

}
