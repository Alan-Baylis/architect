using UnityEngine;
using UnityEditor;

namespace Resources.Properties {

    [CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
    public class ReadOnlyAttributeDrawer : BasePropertyDrawer {

        #region GUI Functions
        public override void OnGUI(Rect aRect, SerializedProperty aProperty, GUIContent aLabel) {
            EditorGUI.BeginDisabledGroup(true);
            EditorGUI.PropertyField(aRect, aProperty, true);
            EditorGUI.EndDisabledGroup();
        }
        #endregion

    }

}
