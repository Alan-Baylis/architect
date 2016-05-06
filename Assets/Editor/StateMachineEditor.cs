using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Reflection;
using Architect.States;
using Architect.Utils;

namespace Architect.Editor {

    [CustomEditor(typeof(StateMachine), true)]
    [CanEditMultipleObjects]
    class StateMachineEditor : BaseCustomEditor {
        private StateMachine currentTarget;

        private SerializedProperty currentState;

        private List<State> states;
        private List<string> availableStates = new List<string>();  // All states available on the current StateMachine

        // TODO: Add toggle to show Name/FullName (?)
        private List<GUIContent> possibleStates = new List<GUIContent>();   // All states that can be added to the current StateMachine (text: Name, tooltip: FullName)

        private int currentStateIndex = 0;

        #region Initialize
        public void Awake() {
            currentTarget = (StateMachine) target;

            SetupStates();
        }

        private void SetupStates() {
            currentState = serializedObject.FindProperty("currentState");

            states = Reflection.GetPrivateFieldValue<List<State>>(currentTarget, "states");
            if (states == null) {
                states = new List<State>();
            }

            // Get all states on the object that are not already in the list (this allows for the AddComponent dialogue to work)
            State[] foundStates = currentTarget.GetComponentsInChildren<State>();
            foreach (State foundState in foundStates) {
                if (states.Contains(foundState) == false) {
                    states.Add(foundState);
                }
            }

            foreach (State state in states) {
                if (state != null) {
                    availableStates.Add(state.GetType().Name);
                } else {
                    availableStates.Add("None");
                }
            }

            // Only set the current state if the state machine has one
            if (currentState.objectReferenceValue != null) {
                int stateIndex = availableStates.FindIndex(s => s == currentState.objectReferenceValue.GetType().Name);
                if (stateIndex != -1) {
                    currentStateIndex = stateIndex;
                }
            }

            possibleStates.Add(new GUIContent("None", "None"));
            Type[] types = Assembly.GetAssembly(typeof(State)).GetTypes();
            foreach (Type type in types) {
                if (type.IsSubclassOf(typeof(State))) {
                    possibleStates.Add(new GUIContent(type.Name, type.FullName));
                }
            }
        }
        #endregion

        #region Display
        public override void OnInspectorGUI() {
            DrawInspector(serializedObject);
            DisplayStateUI();
        }

        // This method is DrawDefaultInspector but extended to allow for overriable fields
        private bool DrawInspector(SerializedObject aObject) {
            EditorGUI.BeginChangeCheck();
            aObject.Update();
            SerializedProperty property = aObject.GetIterator();
            bool enterChildren = true;
            while (property.NextVisible(enterChildren)) {
                switch (property.name) {
                    case "currentState":
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField(FormatLabel(property.name), GUILayout.Width(halfWidth));

                        int newIndex = EditorGUILayout.Popup(currentStateIndex, availableStates.ToArray());

                        if (currentStateIndex != newIndex) {
                            currentStateIndex = newIndex;
                            
                            currentState.objectReferenceValue = states[currentStateIndex];
                        }
                        EditorGUILayout.EndHorizontal();
                        enterChildren = false;
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

        #region States
        private void DisplayStateUI() {
            EditorGUILayout.BeginVertical();

            for (int i = 0; i < availableStates.Count; i++) {
                EditorGUILayout.BeginHorizontal();

                // Update state slot
                int stateIndex = possibleStates.FindIndex(s => s.text == availableStates[i]);
                int newStateIndex = EditorGUILayout.Popup(stateIndex, possibleStates.ToArray(), GUILayout.Height(16.0f));
                if (stateIndex != newStateIndex) {
                    if (currentTarget.GetComponent(possibleStates[newStateIndex].tooltip) == null) {
                        Undo.RecordObject(currentTarget, "Adding / Changing State");

                        State state = currentTarget.GetComponent(availableStates[i]) as State;
                        if (state != null) {
                            Reflection.SetPrivateFieldValue<bool>(state, "safeToDelete", true);
                        }

                        availableStates[i] = possibleStates[newStateIndex].tooltip;

                        Type componentType = Type.GetType(availableStates[i] + ",Assembly-CSharp");
                        states[i] = (State) currentTarget.gameObject.AddComponent(componentType);

                        Reflection.SetPrivateFieldValue<List<State>>(currentTarget, "states", states);
                    }
                }

                // Remove selected state
                if (GUILayout.Button("X", GUILayout.Height(16), GUILayout.Width(16))) {
                    Undo.RecordObject(currentTarget, "Removing State");

                    State state = currentTarget.GetComponent(availableStates[i]) as State;
                    if (state != null) {
                        Reflection.SetPrivateFieldValue<bool>(state, "safeToDelete", true);
                    }

                    states.RemoveAt(i);
                    availableStates.RemoveAt(i);

                    if (availableStates.Count > 0 && states[0] != null) {
                        currentState.objectReferenceValue = states[0];
                    }
                }

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndVertical();

            // Add a new empty state slot
            if (GUILayout.Button("Add state", GUILayout.Height(16))) {
                availableStates.Add("None");
                states.Add(null);
            }
        }
        #endregion

    }

}
