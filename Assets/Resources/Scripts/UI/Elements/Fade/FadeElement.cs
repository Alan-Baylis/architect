using UnityEngine;
using System.Collections;

namespace Resource.UI {

    /// <summary>
    /// UI Element that can fade in and out (including all of its children)
    /// </summary>
    [RequireComponent(typeof(CanvasGroup))]
    public class FadeElement : MonoBehaviour {
        [SerializeField, Tooltip("Speed at which the interface element will be faded")]
        private float rate;

        private FadeValues currentFade = new FadeValues();

        private CanvasGroup canvasGroup;
        private Coroutine fadeCoroutine;

        #region PausedFade Struct
        private struct FadeValues {
            private float start;
            private float end;
            private System.Action callback;

            #region Getters & Setters
            public float Start {
                get { return start; }
                set { start = value; }
            }

            public float End {
                get { return end; }
            }

            public System.Action Callback {
                get { return callback; }
            }
            #endregion

            #region Constructors
            public FadeValues(float aStart, float aEnd, System.Action aCallback) {
                start = aStart;
                end = aEnd;
                callback = aCallback;
            }
            #endregion

            #region Utility Functions
            /// <summary>
            /// Update all the values of the processed fade
            /// </summary>
            public void Update(float aStart, float aEnd, System.Action aCallback) {
                start = aStart;
                end = aEnd;
                callback = aCallback;
            }
            #endregion

        }
        #endregion

        #region Initialization
        private void Awake() {
            canvasGroup = GetComponent<CanvasGroup>();
        }
        #endregion

        #region Fade In
        /// <summary>
        /// Fade the interface element out (invisible => visible)
        /// </summary>
        public void FadeIn(System.Action aCallback = null) {
            if (fadeCoroutine != null) {
                StopCoroutine(fadeCoroutine);
            }

            fadeCoroutine = StartCoroutine(ProcessFade(0.0f, 1.0f, aCallback));
        }

        /// <summary>
        /// Fade the interface element out (invisible => visible)
        /// </summary>
        public IEnumerator FadeInAsync(System.Action aCallback = null) {
            if (fadeCoroutine != null) {
                StopCoroutine(fadeCoroutine);
                fadeCoroutine = null;
            }

            yield return StartCoroutine(ProcessFade(0.0f, 1.0f, aCallback));
        }
        #endregion

        #region Fade Out
        /// <summary>
        /// Fade the interface element out (visible => invisible)
        /// </summary>
        public void FadeOut(System.Action aCallback = null) {
            if (fadeCoroutine != null) {
                StopCoroutine(fadeCoroutine);
            }

            fadeCoroutine = StartCoroutine(ProcessFade(1.0f, 0.0f, aCallback));
        }

        /// <summary>
        /// Fade the interface element out (visible => invisible)
        /// </summary>
        public IEnumerator FadeOutAsync(System.Action aCallback = null) {
            if (fadeCoroutine != null) {
                StopCoroutine(fadeCoroutine);
                fadeCoroutine = null;
            }

            yield return StartCoroutine(ProcessFade(1.0f, 0.0f, aCallback));
        }
        #endregion

        #region Fade Manually
        /// <summary>
        /// Fade the interface element out from the given start to the given end
        /// </summary>
        public void Fade(float aStart, float aEnd, System.Action aCallback = null) {
            if (fadeCoroutine != null) {
                StopCoroutine(fadeCoroutine);
            }

            // Ensure the start and end values are within an acceptable range
            aStart = Mathf.Clamp(aStart, 0.0f, 1.0f);
            aEnd = Mathf.Clamp(aEnd, 0.0f, 1.0f);

            fadeCoroutine = StartCoroutine(ProcessFade(aStart, aEnd, aCallback));
        }

        /// <summary>
        /// Fade the interface element out from the given start to the given end
        /// </summary>
        public IEnumerator FadeAsync(float aStart, float aEnd) {
            if (fadeCoroutine != null) {
                StopCoroutine(fadeCoroutine);
                fadeCoroutine = null;
            }

            // Ensure the start and end values are within an acceptable range
            aStart = Mathf.Clamp(aStart, 0.0f, 1.0f);
            aEnd = Mathf.Clamp(aEnd, 0.0f, 1.0f);

            yield return StartCoroutine(ProcessFade(aStart, aEnd, null));
        }
        #endregion

        #region Fade Utilities
        /// <summary>
        /// Restart the fade from its previously paused status
        /// </summary>
        public void ResumeFade() {
            if (fadeCoroutine != null) {
                fadeCoroutine = StartCoroutine(ProcessFade(currentFade.Start, currentFade.End, currentFade.Callback));
            } else {
                Debug.LogWarning(string.Format("GameObject '{0}' does not have have any stored values to restart from. Only call this from a paused state!", gameObject));
            }
        }

        /// <summary>
        /// Pause the object's fade and store its current status
        /// </summary>
        public void PauseFade() {
            if (fadeCoroutine != null) {
                currentFade.Start = canvasGroup.alpha;
                StopCoroutine(fadeCoroutine);
            } else {
                Debug.LogWarning(string.Format("GameObject '{0}' is currently not fading. Start fading this element prior to making this call!", gameObject));
            }
        }

        /// <summary>
        /// Stop the object's fade completely (cannot resume after stopping)
        /// </summary>
        public void StopFade() {
            if (fadeCoroutine != null) {
                StopCoroutine(fadeCoroutine);
                fadeCoroutine = null;
            } else {
                Debug.LogWarning(string.Format("GameObject '{0}' is currently not fading. Start fading this element prior to making this call!", gameObject));
            }
        }
        #endregion

        #region Fade Processing
        private IEnumerator ProcessFade(float aStart, float aEnd, System.Action aCallback = null) {
            currentFade.Update(aStart, aEnd, aCallback);

            float currentRate = aStart;

            // Modify the rate so it will always make the lerp reach the end value
            rate = (aStart == 0) ? Mathf.Abs(rate) : rate * -1;

            // Lerp the alpha of the Fade Element
            do {
                currentRate += (rate * Time.deltaTime);
                canvasGroup.alpha = Mathf.Lerp(aStart, aEnd, aStart);
                yield return null;
            } while (canvasGroup.alpha != aEnd);
            
            // Ensure the fade is completely finished
            yield return null;

            fadeCoroutine = null;

            if (aCallback != null) {
                aCallback();
            }
        }
        #endregion

    }

}
