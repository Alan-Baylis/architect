using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Text;

namespace Architect.Editor {

    /*
     * Base Custom Inspector is meant to provide some common methods and functions that each 
     * custom editor/inspector can use.
     */
    public class BaseCustomEditor : UnityEditor.Editor {
        protected MonoScript script;
        protected float halfWidth = 116;
        protected float fullWidth = 238;

        private const char SPACE = ' ';

        // The Script field is a standard field in regular inspectors. It allows the user to double click to open said script.
        protected void DisplayScriptField() {
            script = EditorGUILayout.ObjectField("Script", script, typeof(MonoScript), false) as MonoScript;
        }

        // Format the field label so it meets the same format as standard Unity Inspector labels
        protected string FormatLabel(string aLabel) {
            StringBuilder newLabel = new StringBuilder(aLabel.Length);

            // Add a space before each capital letter in the label unless the capitalized letter is followed by another capitalized letter (UI, GUI, etc)
            for (int i = 0; i < aLabel.Length; i++) {
                if (char.IsUpper(aLabel[i]) && ((i - 1) > 0 && char.IsUpper(aLabel[i - 1]) == false)) {
                    newLabel.Append(SPACE);
                }

                newLabel.Append(aLabel[i]);
            }

            // Capitalize the first character of the label
            newLabel.Replace(newLabel[0], char.ToUpper(newLabel[0]), 0, 1);

            return newLabel.ToString();
        }
    }

}

