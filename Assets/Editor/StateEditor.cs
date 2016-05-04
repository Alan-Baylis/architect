using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;
using Architect.States;
using Architect.Editor;
using Architect.Utils;

[CustomEditor(typeof(State), true)]
[CanEditMultipleObjects]
public class StateEditor : BaseCustomEditor {
    private State currentTarget;

    private int transitionMask = 0;

    private List<State> transitions;
    private List<string> stateNames = new List<string>();

    #region Initialize
    public void Awake() {
        if (currentTarget == null) {
            currentTarget = (State) target;
        }

        SetupStates();
    }

    private void SetupStates() {
        List<State> states = new List<State>(currentTarget.GetComponents<State>());
        states.RemoveAt(states.FindIndex(s => s.GetType().Name == currentTarget.GetType().Name));

        foreach (State state in states) {
            stateNames.Add(state.GetType().Name);
        }

        // Setup the transition mask value from the stored transitions
        transitions = Reflection.GetPrivateFieldValue<List<State>>(currentTarget, "transitions");

        if (transitions != null) {
            for (int i = 0; i < transitions.Count; i++) {
                transitionMask |= (1 << (stateNames.FindIndex(s => s == transitions[i].GetType().Name)));
            }

            SetTransitions(transitionMask);
        }
    }
    #endregion

    #region Display
    public override void OnInspectorGUI() {
        DrawInspector(serializedObject);
        SafeDelete();
    }

    // This method is DrawDefaultInspector but extended to allow for overriable fields
    private bool DrawInspector(SerializedObject aObject) {
        EditorGUI.BeginChangeCheck();
        aObject.Update();
        SerializedProperty property = aObject.GetIterator();
        bool enterChildren = true;
        while (property.NextVisible(enterChildren)) {
            switch (property.name) {
                case "transitions":
                    SetTransitions(EditorGUILayout.MaskField("Allowed Transitions", transitionMask, stateNames.ToArray()));
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

    public void OnDisable() {
        SafeDelete();
    }

    #region Utility Functions
    private void SetTransitions(int aNewMask, bool aOverride = false) {
        if (transitionMask != aNewMask || aOverride) {
            transitionMask = aNewMask;

            // Set the allowed transtions on the state
            transitions = new List<State>();

            // Using the, bitwise, mask set the list of transitions based on what current selected
            for (int i = 0; i < stateNames.Count; i++) {
                if ((transitionMask & (1 << i)) != 0) {
                    Type componentType = Type.GetType(stateNames[i] + ",Assembly-CSharp");
                    transitions.Add((State) currentTarget.GetComponent(componentType));
                }
            }

            Undo.RecordObject(currentTarget, "Changed State Transitions");
            Reflection.SetPrivateFieldValue<List<State>>(currentTarget, "transitions", transitions);
        }
    }

    private void SafeDelete() {
        bool safeToDelete = Reflection.GetPrivateFieldValue<bool>(currentTarget, "safeToDelete");
        if (safeToDelete) {
            DestroyImmediate(currentTarget);
        }
    }
    #endregion

}
