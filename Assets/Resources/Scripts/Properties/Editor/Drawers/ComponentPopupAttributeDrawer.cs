using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Reflection;
using Resource.Utils;

namespace Resource.Properties {

    [CustomPropertyDrawer(typeof(ComponentPopupAttribute))]
    public class ComponentPopupAttributeDrawer : BasePropertyDrawer {
        private int aIndex = 0;
        private int currentIndex = 0;
        private List<GUIContent> components;
        
        #region GUI Functions
        public override void OnGUI(Rect aRect, SerializedProperty aProperty, GUIContent aLabel) {
            if (aProperty.type != "string") {
                EditorGUI.LabelField(aRect, aLabel.text, "ComponentPopup only string compatible.");
                return;
            }

            if (components == null) {
                ComponentPopupAttribute classAttribute = attribute as ComponentPopupAttribute;
                SetupClasses(classAttribute.Type);

                aIndex = Mathf.Clamp(components.FindIndex(c => c.tooltip == aProperty.stringValue), 0, components.Count);
            }

            aIndex = EditorGUI.Popup(aRect, aLabel, aIndex, components.ToArray());

            if (aIndex != currentIndex) {
                currentIndex = aIndex;
                aProperty.stringValue = components[aIndex].tooltip;
            }
        }
        #endregion

        #region Setup Functions
        private void SetupClasses(Type aType) {
            components = new List<GUIContent>(155); // 155 is roughly the number of Unity's MonoBehaviours so let's just leave it there
            components.Add(new GUIContent(StringUtils.NONE, StringUtils.NONE));

            // Loop through all assemblies since user created MonoBehaviours will not appear in the basic Assembly.GetAssembly() call
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly assembly in assemblies) {
                // Assembly-CSharp and Unity should contains all of the MonoBehaviours that could be used in GetComponent calls
                if (assembly.FullName.Contains("Assembly-CSharp") || assembly.FullName.Contains("Unity")) {
                    // Get all of the types (scripts) within the assembly
                    Type[] types = assembly.GetTypes();
                    foreach (Type type in types) {
                        // Only add the given type (this is was it passed in the PropertyAttribute)
                        if (type.IsSubclassOf(aType)) {
                            components.Add(new GUIContent(type.Name, type.FullName));
                        }
                    }
                }
            }
        }
        #endregion

    }

}
