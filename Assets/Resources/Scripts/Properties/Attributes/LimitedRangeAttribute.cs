using UnityEngine;

namespace Resource.Properties {

    /// <summary>
    /// Require the given property to be set to true (active) for this property to be enabled and modifiable.
    /// If the given property is set to false (inactive) then this property will be disabled.
    /// </summary>
    public class LimitedRangeAttribute : PropertyAttribute {
        private float minLimit, maxLimit;

        #region Getters & Setters
        public float MinLimit {
            get { return minLimit; }
            set { minLimit = value; }
        }

        public float MaxLimit {
            get { return maxLimit; }
            set { maxLimit = value; }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Require the given property to be set to true (active) for this property to be enabled and modifiable.
        /// If the given property is set to false (inactive) then this property will be disabled.
        /// </summary>
        /// <param name="aMinLimit">Minimum value the range can be</param>
        /// <param name="aMaxLimit">Maximum value the range can be</param>
        public LimitedRangeAttribute(float aMinLimit, float aMaxLimit) {
            minLimit = aMinLimit;
            maxLimit = aMaxLimit;
        }
        #endregion

    }

}
