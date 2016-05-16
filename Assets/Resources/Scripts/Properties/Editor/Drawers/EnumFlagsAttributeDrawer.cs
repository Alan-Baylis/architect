using UnityEngine;
using UnityEditor;

namespace Resources.Properties {

    [CustomPropertyDrawer(typeof(EnumFlagsAttribute))]
    public class EnumFlagsAttributeDrawer : PropertyDrawer {

        #region GUI FUnctions
        public override void OnGUI(Rect aRect, SerializedProperty aProperty, GUIContent aLabel) {
            aProperty.intValue = EditorGUI.MaskField(aRect, aLabel, aProperty.intValue, aProperty.enumDisplayNames);
        }
        #endregion

    }

}
