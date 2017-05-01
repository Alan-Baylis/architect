using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Resource.Utils {

    public static class EditorUtils {

        #region Display Functions
        public static void DisplayDisabledGroup(this SerializedProperty aProperty, bool aIncludeChildren, GUILayoutOption[] aLayoutOptions) {
#if UNITY_EDITOR
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.PropertyField(aProperty, aIncludeChildren, new GUILayoutOption[0]);
            EditorGUI.EndDisabledGroup();
#endif
        }
        #endregion

    }

}
