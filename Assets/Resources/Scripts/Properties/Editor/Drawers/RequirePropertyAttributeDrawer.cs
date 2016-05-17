using UnityEngine;
using UnityEditor;

namespace Resources.Properties {

    [CustomPropertyDrawer(typeof(RequirePropertyAttribute))]
    public class RequirePropertyAttributeDrawer : BasePropertyDrawer {
        private SerializedProperty requiredProperty;

        #region GUI Functions
        public override void OnGUI(Rect aRect, SerializedProperty aProperty, GUIContent aLabel) {
            RequirePropertyAttribute requiredPropertyAttribute = attribute as RequirePropertyAttribute;

            if (requiredProperty == null) {
                requiredProperty = aProperty.serializedObject.FindProperty(requiredPropertyAttribute.PropertyName);
            }

            if (requiredProperty.type != "bool") {
                EditorGUI.LabelField(aRect, aLabel.text, "RequiredProperty must require a bool property.");
                return;
            }

            EditorGUI.BeginDisabledGroup(requiredProperty.boolValue == false);
            if (aProperty.isExpanded) {
                EditorGUI.PropertyField(aRect, aProperty, true);
            } else {
                EditorGUI.PropertyField(aRect, aProperty, false);
            }
            EditorGUI.EndDisabledGroup();
        }
        #endregion

    }

}