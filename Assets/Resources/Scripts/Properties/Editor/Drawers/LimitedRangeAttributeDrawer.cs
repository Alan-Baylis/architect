using UnityEngine;
using UnityEditor;

namespace Resource.Properties {

    [CustomPropertyDrawer(typeof(LimitedRangeAttribute))]
    public class LimitedRangeAttributeDrawer : BasePropertyDrawer {
        private const float padding = 10.0f;

        private GUIContent labelContent = new GUIContent();

        #region GUI Functions
        public override float GetPropertyHeight(SerializedProperty aProperty, GUIContent aLabel) {
            return EditorGUI.GetPropertyHeight(aProperty, aLabel) * 2.0f;
        }

        public override void OnGUI(Rect aPosition, SerializedProperty aProperty, GUIContent aLabel) {
            if (aProperty.type == "LimitedRange") {

                LimitedRangeAttribute range = attribute as LimitedRangeAttribute;
                SerializedProperty min = aProperty.FindPropertyRelative("min");
                SerializedProperty max = aProperty.FindPropertyRelative("max");
                float newMin = min.floatValue;
                float newMax = max.floatValue;

                // Property Label
                EditorGUI.LabelField(new Rect(aPosition.x, aPosition.y, aPosition.width, aPosition.height), aLabel);

                // Setup
                EditorGUIUtility.labelWidth = 25.0f;

                // Min Limit Field
                labelContent = new GUIContent("Min");
                Rect minPosition = EditorGUI.PrefixLabel(new Rect(aPosition.x + 100.0f, aPosition.y, 80.0f, aPosition.height / 2.0f), labelContent);
                newMin = Mathf.Clamp(EditorGUI.FloatField(minPosition, newMin), range.MinLimit, range.MaxLimit);

                //// Max Limit Field
                EditorGUIUtility.labelWidth = 28.0f;
                labelContent.text = "Max";
                Rect maxPosition = EditorGUI.PrefixLabel(new Rect(aPosition.x + (minPosition.x + minPosition.width), aPosition.y, (aPosition.width - (minPosition.x + minPosition.width)), aPosition.height / 2.0f), labelContent);
                newMax = Mathf.Clamp(EditorGUI.FloatField(maxPosition, newMax), range.MinLimit, range.MaxLimit);

                // Slider
                EditorGUI.MinMaxSlider(new Rect(aPosition.x + padding, aPosition.y + (aPosition.height / 2.0f), aPosition.width - (padding * 2.0f), aPosition.height / 2.0f), ref newMin, ref newMax, range.MinLimit, range.MaxLimit);

                // Update the range
                min.floatValue = newMin;
                max.floatValue = newMax;
            } else {
                Debug.LogError(string.Format("Invalid type [{0}] for LimitedRangeAttribute", aProperty.type));
            }
        }
        #endregion

    }

}
