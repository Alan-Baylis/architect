using UnityEngine;
using System.Collections;
using Resource.Properties;

namespace Resource.UI {

    /// <summary>
    /// An UI elemtent that will magnet to other, specified, objects when moved around.
    /// For use with controllers.
    /// </summary>
    public class MagneticCursor : MonoBehaviour {
        [SerializeField]
        private float cursorSpeed = 2.0f;
        [SerializeField, Tooltip("Layers that when collided with will result in this object triggering of enabled magnetic properties")]
        private LayerMask magneticLayers;

        [SerializeField, Tooltip("When within range of a target element slow this object")]
        private bool doesSlow = true;
        [SerializeField, RequireProperty("doesSlow")]
        private SlowAttributes slowAttributes;

        [SerializeField, Tooltip("When within range of a target element snap this object to the center of the target when movement stops")]
        private bool doesSnap = false;
        [SerializeField, RequireProperty("doesSnap")]
        private SnapAttributes snapAttributes;

        private RaycastHit2D[] cursorRaycast;

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
        private struct SnapAttributes {
            [SerializeField]
            private float speed;

            #region Getters & Setters
            public float Speed {
                get { return speed; }
            }
            #endregion
        }
        #endregion

        #region Initialization
        /// <summary>
        /// Disable the system's cursor so we can instead move our own custom cursor stand in
        /// </summary>
        private void Awake() {
            #if !UNITY_EDITOR
                Cursor.visible = false;
            #endif

            cursorRaycast = new RaycastHit2D[1];
        }
        #endregion

        #region Update
        private void Update() {
            Vector3 targetPosition = transform.position;
            targetPosition.x += (System.Convert.ToInt32(Input.GetKey(KeyCode.D)) - (System.Convert.ToInt32(Input.GetKey(KeyCode.A))));
            targetPosition.y += (System.Convert.ToInt32(Input.GetKey(KeyCode.W)) - (System.Convert.ToInt32(Input.GetKey(KeyCode.S))));

            //targetPosition.x += Input.GetAxis("Horizontal");
            //targetPosition.y += Input.GetAxis("Vertical");
            targetPosition.z = 0.0f;

            float currentSpeed = cursorSpeed;
            if (Physics2D.RaycastNonAlloc(transform.position, Vector2.zero, cursorRaycast, float.MaxValue, magneticLayers) > 0) {
                if (doesSlow) {
                    currentSpeed = slowAttributes.Speed;
                }

                if (doesSnap) {
                    if (transform.position == targetPosition) {
                        targetPosition = cursorRaycast[0].collider.transform.position;
                        currentSpeed = snapAttributes.Speed;
                    }
                }
            }

            transform.position = Vector3.MoveTowards(transform.position, targetPosition, currentSpeed * Time.deltaTime);
        }
        #endregion

    }

}
