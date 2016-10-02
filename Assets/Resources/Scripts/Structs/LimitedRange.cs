using UnityEngine;

namespace Resource.Structs {
    
    [System.Serializable]
    public struct LimitedRange {
        [SerializeField]
        private float min, max;

        #region Statics
        private static LimitedRange ten = new LimitedRange(0.0f, 10.0f);
        private static LimitedRange hundred = new LimitedRange(0.0f, 100.0f);
        #endregion

        #region Getters & Setters
        public float Min {
            get { return min; }
            set { min = value; }
        }

        public float Max {
            get { return max; }
            set { max = value; }
        }

        public static LimitedRange ToTen {
            get { return ten; }
        }

        public static LimitedRange ToHundred {
            get { return hundred; }
        }
        #endregion

        #region Constructors
        public LimitedRange(float aMin, float aMax) {
            min = aMin;
            max = aMax;
        }
        #endregion

        #region Utility Functions
        /// <summary>
        /// Check to see if the given value is within the limited range
        /// </summary>
        /// <param name="aValue"></param>
        public bool WithinRange(float aValue) {
            return (aValue <= max && aValue >= min);
        }

        /// <summary>
        /// Check to see if the given values are within the limited range. If they are not the referenced index will contain the first invalid index.
        /// </summary>
        /// <param name="aIndex">Reference containg the invalid index. All values are valid if this function returns with a value of -1</param>
        /// <param name="aValues">Values to compare</param>
        /// <returns></returns>
        public bool WithinRange(ref int aIndex, params float[] aValues) {
            aIndex = -1;

            for (int i = 0; i < aValues.Length; i++) {
                float value = aValues[i];
                if (value > max || value < min) {
                    aIndex = i;
                    break;
                }
            }

            return (aIndex == -1);
        }
        #endregion

    }

}
