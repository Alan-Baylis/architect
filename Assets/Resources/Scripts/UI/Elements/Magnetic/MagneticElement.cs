using UnityEngine;
using System.Collections;

namespace Resources.UI {

    /// <summary>
    /// An UI elemtent that will magnet to other, specified, objects when moved around
    /// </summary>
    public class MagneticElement : MonoBehaviour {
        [SerializeField, Tooltip("Layers that when collided with will result in the triggering of enabled magnetic properties")]
        private LayerMask targets;

        [SerializeField, Tooltip("When within range of a target element slow the object")]
        private bool doesSlow = true;
        [SerializeField]
        private SlowAttributes slowAttributes;

        [SerializeField, Tooltip("When within range of a target element lerp the object to the center of the target when movement stops")]
        private bool doesLock = false;
        [SerializeField]
        private LockAttributes lockAttributes;

        #region SlowAttributes Struct
        [System.Serializable]
        private struct SlowAttributes {
            [SerializeField]
            private float speed;

            #region Getters & Setters
            public float Speed {
                get { return speed; }
            }
            #endregion
        }
        #endregion

        #region LockAttributes Struct
        [System.Serializable]
        private struct LockAttributes {
            [SerializeField]
            private float speed;

            #region Getters & Setters
            public float Speed {
                get { return speed; }
            }
            #endregion
        }
        #endregion

        #region Update / Triggers
        
            // TODO: Add Update/Triggers to check for and handle collisions/overlaps with the target layers
        
        #endregion

    }

}
