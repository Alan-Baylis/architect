using UnityEngine;
using UnityEditor;
using Resource.Utils;

namespace Resource.Properties {

    [CustomPropertyDrawer(typeof(EnumFlagsAttribute))]
    public class EnumFlagsAttributeDrawer : BasePropertyDrawer {

        #region GUI Functions
        public override void OnGUI(Rect aRect, SerializedProperty aProperty, GUIContent aLabel) {
            if (StringUtils.ContainsIgnoreCase(aProperty.type, "enum")) {
                aProperty.intValue = EditorGUI.MaskField(aRect, aLabel, aProperty.intValue, aProperty.enumDisplayNames);
            } else {
                EditorGUI.LabelField(aRect, aLabel.text, "EnumFlags only compatible with enum not with type [" + aProperty.type + "]");
            }
        }
        #endregion

    }

}
