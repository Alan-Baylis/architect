using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Resources.Editor {

    public class BaseEditor : UnityEditor.Editor {

        #region Display Functions
        protected void DisplayDisabledGroup(SerializedProperty aProperty, bool aDisabled = true) {
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.PropertyField(aProperty, aDisabled, new GUILayoutOption[0]);
            EditorGUI.EndDisabledGroup();
        }

        protected void DisplayDisabledGroup(SerializedProperty aProperty, GUILayoutOption[] aLayoutOptions, bool aDisabled = true) {
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.PropertyField(aProperty, aDisabled, aLayoutOptions);
            EditorGUI.EndDisabledGroup();
        }
        #endregion

    }

}
