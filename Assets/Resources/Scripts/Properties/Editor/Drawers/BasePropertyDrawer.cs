using UnityEngine;
using UnityEditor;

namespace Resource.Properties {
    
    public class BasePropertyDrawer : PropertyDrawer {
        #region GUI Functions
        public override float GetPropertyHeight(SerializedProperty aProperty, GUIContent aLabel) {
            return EditorGUI.GetPropertyHeight(aProperty);
        }
        #endregion

    }

}
