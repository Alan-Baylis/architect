using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using System;
using System.Collections.Generic;
using Resources.UI;
using Resources.Utils;

namespace Resources.Editor {

    [CustomEditor(typeof(MagneticElement), true)]
    [CanEditMultipleObjects]
    public class MagneticElementEditor : UnityEditor.Editor {
        private SerializedProperty doesSlowProperty;
        private SerializedProperty doesLockProperty;

        #region Initialization
        protected void OnEnable() {
            doesSlowProperty = serializedObject.FindProperty("doesSlow");
            doesLockProperty = serializedObject.FindProperty("doesLock");
        }
        #endregion

        #region Display
        public override void OnInspectorGUI() {
            DrawInspector(serializedObject);
        }

        // This method is DrawDefaultInspector but extended to allow for overriable fields
        private bool DrawInspector(SerializedObject aObject) {
            EditorGUI.BeginChangeCheck();
            aObject.Update();
            SerializedProperty property = aObject.GetIterator();
            bool enterChildren = true;
            while (property.NextVisible(enterChildren)) {
                switch (property.name) {
                    case "slowAttributes":
                        EditorGUI.BeginDisabledGroup(doesSlowProperty.boolValue);
                        EditorGUILayout.PropertyField(property, true, new GUILayoutOption[0]);
                        enterChildren = false;
                        EditorGUI.EndDisabledGroup();
                        break;
                    case "lockAttributes":
                        EditorGUI.BeginDisabledGroup(doesLockProperty.boolValue);
                        EditorGUILayout.PropertyField(property, true, new GUILayoutOption[0]);
                        enterChildren = false;
                        EditorGUI.EndDisabledGroup();
                        break;
                    case "m_Script":
                        EditorGUI.BeginDisabledGroup(true);
                        EditorGUILayout.PropertyField(property, true, new GUILayoutOption[0]);
                        enterChildren = false;
                        EditorGUI.EndDisabledGroup();
                        break;
                    default:
                        EditorGUILayout.PropertyField(property, true, new GUILayoutOption[0]);
                        enterChildren = false;
                        break;
                }

            }
            aObject.ApplyModifiedProperties();
            return EditorGUI.EndChangeCheck();
        }
        #endregion

    }

}
