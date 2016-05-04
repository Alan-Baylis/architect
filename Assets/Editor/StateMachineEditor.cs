using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Reflection;
using Architect.States;
using Architect.Editor;
using Architect.Utils;

[CustomEditor(typeof(StateMachine), true)]
[CanEditMultipleObjects]
class StateMachineEditor : BaseCustomEditor {
    private StateMachine currentTarget;

    private SerializedProperty currentState;

    private List<State> states;
    private List<string> stateNames = new List<string>();
    private List<string> allStates = new List<string>();

    private int currentStateIndex = 0;

    #region Initialize
    public void OnEnable() {
        if (currentTarget == null) {
            currentTarget = (StateMachine) target;
        }

        if (stateNames == null || stateNames.Count == 0) {
            SetupStates();
        }
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
                stateNames.Add(state.GetType().Name);
            } else {
                stateNames.Add("None");
            }
        }

        // Only set the current state if the state machine has one
        if (currentState.objectReferenceValue != null) {
            int stateIndex = stateNames.FindIndex(s => s == currentState.objectReferenceValue.GetType().Name);
            if (stateIndex != -1) {
                currentStateIndex = stateIndex;
            }
        }

        allStates.Add("None");
        Type[] types = Assembly.GetAssembly(typeof(State)).GetTypes();
        foreach (Type type in types) {
            if (type.IsSubclassOf(typeof(State))) {
                allStates.Add(type.ToString());
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

                    int newIndex = EditorGUILayout.Popup(currentStateIndex, stateNames.ToArray());

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

        for (int i = 0; i < stateNames.Count; i++) {
            EditorGUILayout.BeginHorizontal();

            int stateIndex = allStates.IndexOf(stateNames[i]);
            int newStateIndex = EditorGUILayout.Popup(stateIndex, allStates.ToArray(), GUILayout.Height(16.0f));
            if (stateIndex != newStateIndex) {
                if (currentTarget.GetComponent(allStates[newStateIndex]) == null) {
                    Undo.RecordObject(currentTarget, "Adding / Changing State");

                    State state = currentTarget.GetComponent(stateNames[i]) as State;
                    if (state != null) {
                        Reflection.SetPrivateFieldValue<bool>(state, "safeToDelete", true);
                    }

                    stateNames[i] = allStates[newStateIndex];

                    Type componentType = Type.GetType(stateNames[i] + ",Assembly-CSharp");
                    states[i] = (State) currentTarget.gameObject.AddComponent(componentType);

                    Reflection.SetPrivateFieldValue<List<State>>(currentTarget, "states", states);
                }
            }

            // Remove
            if (GUILayout.Button("X", GUILayout.Height(16), GUILayout.Width(16))) {
                Undo.RecordObject(currentTarget, "Removing State");

                State state = currentTarget.GetComponent(stateNames[i]) as State;
                if (state != null) {
                    Reflection.SetPrivateFieldValue<bool>(state, "safeToDelete", true);
                }

                states.RemoveAt(i);
                stateNames.RemoveAt(i);

                if (stateNames.Count > 0) {
                    currentState.objectReferenceValue = states[0];
                }
            }

            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndVertical();

        // Add
        if (GUILayout.Button("Add state", GUILayout.Height(16))) {
            stateNames.Add("None");
            states.Add(null);
        }
    }
    #endregion

}
