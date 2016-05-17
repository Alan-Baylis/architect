using UnityEngine;
using UnityEditor;

namespace Resources.Properties {

    [CustomPropertyDrawer(typeof(EnumFlagsAttribute))]
    public class EnumFlagsAttributeDrawer : BasePropertyDrawer {

        #region GUI Functions
        public override void OnGUI(Rect aRect, SerializedProperty aProperty, GUIContent aLabel) {
            if (aProperty.type != "enum") {
                EditorGUI.LabelField(aRect, aLabel.text, "EnumFlags only compatible with enum");
                return;
            }

            aProperty.intValue = EditorGUI.MaskField(aRect, aLabel, aProperty.intValue, aProperty.enumDisplayNames);
        }
        #endregion

    }

}
