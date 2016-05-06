using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Architect.States {

    [Serializable]
    public class StateMachine : MonoBehaviour {
        [SerializeField]
        private State currentState;
        [SerializeField, HideInInspector]
        private List<State> states;

        private State previousState;

        private bool initialized = false;
        private bool changingState = false;
        private bool returningToPreviousState = false;

        #region Getters & Setters
        public bool Initialized {
            get { return initialized; }
        }

        public bool ReturningToPreviousState {
            get { return returningToPreviousState; }
        }
        #endregion

        #region Initialization
        void Start() {
            if (states != null) {
                foreach (State state in states) {
                    if (state != null) {
                        state.Initialize(this);
                    }
                }

                if (currentState != null) {
                    currentState.Enable();
                } else {
                    Debug.LogWarning(string.Format("StatMachine: {0} does not have a current state. Please set it in the editor!", this));
                }
            }

            initialized = true;
        }
        #endregion

        // With more StateMachines it would be worth making a master or controller with a single Update that updates all state machines
        void Update() {
            if (initialized && currentState != null && changingState == false) {
                currentState.Action();
            }
        }

        #region State Transition
        // Change the state given a valid state key. Store the previous state in case we want to go back.
        public void ChangeState(string aStateKey) {
            changingState = true;

            if (states != null) {
                if (string.IsNullOrEmpty(aStateKey) == false) {
                    State nextState = states.Find(s => s.Key == aStateKey);
                    if (nextState != null && currentState.CanTransition(nextState)) {
                        currentState.Disable();
                        previousState = currentState;
                        currentState = nextState;
                        currentState.Enable();
                    }

                    #if UNITY_EDITOR
                        if (nextState == null) {
                            Debug.LogWarning(string.Format("State: {0} on Object: {1} does not exist in StateMachine", aStateKey, gameObject));
                        }
                    #endif
                }
            }

            changingState = false;
        }

        // Return to the previous state while clearing out the previous state data (may not want to do this).
        public void ReturnToPreviousState() {
            if (previousState != null) {
                returningToPreviousState = true;

                currentState.Disable();
                currentState = previousState;
                currentState.Enable();

                previousState = null;
            }
        }
        #endregion

    }
}
