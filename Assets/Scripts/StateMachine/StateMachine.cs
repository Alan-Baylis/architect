using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Architect.States {

    [System.Serializable]
    [DisallowMultipleComponent]
    public class StateMachine : MonoBehaviour {
        [SerializeField]
        private State currentState;
        [SerializeField]
        private List<State> states;

        private State previousState;
        
        private bool changingState = false;

        #region Getters & Setters
        public bool ChangingState {
            get { return changingState; }
        }
        #endregion

        #region Initialization
        // Initialize all states. Enable and start executing the current state (chosen in the inspector).
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
        }
        #endregion

        #region Update Functions
        void Update() {
            if (currentState != null && changingState == false) {
                currentState.Action();
            }
        }
        #endregion

        #region State Transitions
        /// <summary>
        /// Change the state given a valid state key. Store the previous state in case we want to go back.
        /// </summary>
        public void ChangeState(string aStateKey) {
            if (changingState == false) {
                changingState = true;

                if (states != null && string.IsNullOrEmpty(aStateKey) == false) {
                    State nextState = states.Find(s => s.Key == aStateKey.Trim());
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

                changingState = false;
            }
        }

        public void ReturnToPreviousState() {
            if (previousState != null && changingState == false) {
                changingState = true;

                currentState.Disable();
                currentState = previousState;
                currentState.Enable();

                previousState = null;
                changingState = false;
            }
        }
        #endregion

    }
}
