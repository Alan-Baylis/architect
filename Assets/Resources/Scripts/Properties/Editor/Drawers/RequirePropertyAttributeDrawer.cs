using UnityEngine;
using UnityEditor;

namespace Resource.Properties {

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
            EditorGUI.PropertyField(aRect, aProperty, true);
            EditorGUI.EndDisabledGroup();
        }
        #endregion

    }

}
