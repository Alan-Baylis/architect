using UnityEngine;
using System.Collections;
using System.Text;

namespace Resource.Utils {

    /// <summary>
    /// Utilites relating to the usage of strings
    /// </summary>
    public static class StringUtils {
        public const char SPACE = ' ';
        public const string NONE = "None";

        /// <summary>
        /// Format the given string to match that of Unity's inspector field names (ex. rateOfFire => Rate Of Fire)
        /// </summary>
        /// <param name="aText">Text to convert to inspector case</param>
        public static string ToInspectorCase(string aText) {
            StringBuilder newLabel = new StringBuilder(aText.Length);
            int offset = 0;

            // Add a space before each capital letter in the text unless the capitalized letter is followed by another capitalized letter (AI, GUI, etc)
            for (int i = 0; i < aText.Length; i++) {
                if (i + offset < aText.Length && char.IsUpper(aText[i + offset])) {
                    newLabel.Append(SPACE);
                    offset++;
                }

                newLabel.Append(aText[i]);
            }

            // Capitalize the first character of the text
            newLabel[0] = char.ToUpper(newLabel[0]);

            return newLabel.ToString();
        }

        /// <summary>
        /// Check to see if the provided string is contained within a source string without case being important
        /// </summary>
        /// <param name="aSource">Source string that may, or may not, contain the string to check</param>
        /// <param name="aToCheck">String to find within the source string</param>
        /// <returns></returns>
        public static bool ContainsIgnoreCase(string aSource, string aToCheck) {
            return (aSource.IndexOf(aToCheck, System.StringComparison.CurrentCultureIgnoreCase) != -1);
        }

    }

}
