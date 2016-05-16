using UnityEngine;
using System.Collections;
using System.Text;

namespace Resources.Utils {

    /// <summary>
    /// Utilites relating to the usage of strings
    /// </summary>
    public static class StringUtils {
        private const char SPACE = ' ';

        /// <summary>
        /// Format the given string to match that of Unity's inspector field names (ex. rateOfFire => Rate Of Fire)
        /// </summary>
        public static string ToInspectorCase(string aText) {
            StringBuilder newLabel = new StringBuilder(aText.Length);
            int offset = 0;

            // Add a space before each capital letter in the label unless the capitalized letter is followed by another capitalized letter (UI, GUI, etc)
            for (int i = 0; i < aText.Length; i++) {
                if (i + offset < aText.Length && char.IsUpper(aText[i + offset])) {
                    newLabel.Append(SPACE);
                    offset++;
                }

                newLabel.Append(aText[i]);
            }

            // Capitalize the first character of the label
            newLabel.Replace(newLabel[0], char.ToUpper(newLabel[0]), 0, 1);

            return newLabel.ToString();
        }

    }

}
