using UnityEngine;
using System.Collections;
using System;

namespace Resource.Utils {

    public static class MathUtils {
        
        #region Digit Count
        /// <summary>
        /// Get the number of digits contained within the number (length of the number)
        /// </summary>
        public static int DigitCount(int aNumber) {
            return (aNumber == 0) ? 1 : (int) Math.Floor(Math.Log10(Math.Abs(aNumber)) + 1);
        }

        /// <summary>
        /// Get the number of digits contained within the number (length of the number)
        /// </summary>
        public static int DigitCount(long aNumber) {
            return (aNumber == 0) ? 1 : (int) Math.Floor(Math.Log10(Math.Abs(aNumber)) + 1);
        }

        /// <summary>
        /// Get the number of digits contained within the number (length of the number)
        /// </summary>
        public static int DigitCount(float aNumber) {
            string number = Math.Abs(aNumber).ToString().Replace(".", string.Empty);
            return (aNumber == 0) ? 1 : (int) Math.Floor(Math.Log10(float.Parse(number)) + 1);
        }

        /// <summary>
        /// Get the number of digits contained within the number (length of the number)
        /// </summary>
        public static int DigitCount(double aNumber) {
            string number = Math.Abs(aNumber).ToString().Replace(".", string.Empty);
            return (aNumber == 0) ? 1 : (int) Math.Floor(Math.Log10(float.Parse(number)) + 1);
        }
        #endregion

    }

}
