using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using Resource.Utils;

namespace Resource.Editor {

    public class AnimationHierarchyFixer : EditorWindow {
        private const int fieldWidth = 175;

        private Animator selectedAnimator = null;
        private AnimationClip selectedAnimationClip = null;

        private MultiDictionary<string, EditorCurveBinding> animationObjectBindings = null;
        private List<string> objectPaths = null;

        private List<AnimationClip> animationClips = null;
        private string[] animationNames;
        
        private int selectedAnimation = 0;

        private Color defaultColor;
        private Vector2 scrollPosition = Vector2.zero;
        
        private GUIStyle wrappingLabelStyle = new GUIStyle();
        private GUISkin skin_ToolbarContainer;
        private GUISkin skin_ToolbarContent;

        private static Window[] windows = new Window[] { new Window("Animator"), new Window("Animations"), new Window("Timeline Object Paths") };

        #region Window Class
        private class Window {
            private string key = null;
            private Rect rect = default(Rect);
            private GUIContent content = null;

            private bool inError;
            private string errorMessage = string.Empty;

            #region Constructor 
            public Window(string aKey) {
                key = aKey;

                content = new GUIContent(key);
            }
            #endregion

            #region Getters & Setters
            public string Key {
                get { return key; }
            }

            public Rect Rect {
                get { return rect; }
                set { rect = value; }
            }

            public GUIContent Content {
                get { return content; }
            }

            public bool InError {
                get { return inError; }
            }

            public string ErrorMessage {
                get { return errorMessage; }
            }
            #endregion

            #region Utility Functions
            public void SetError(string aMessage) {
                inError = true;
                errorMessage = aMessage;
            }

            public void ClearError() {
                inError = false;
                errorMessage = string.Empty;
            }
            #endregion

        }
        #endregion

        #region Initialize
        [MenuItem("Window/Animation Hierarchy Editor")]
        static void ShowWindow() {
            AnimationHierarchyFixer window = GetWindow<AnimationHierarchyFixer>("Anim Hierarchy");
            window.minSize = new Vector2(765.0f, 500.0f);
            window.maxSize = new Vector2(765, 500.0f);

            window.Show();
        }

        public void OnEnable() {
            defaultColor = GUI.color;

            windows[0].Rect = new Rect(10, 40, 200, 100);
            windows[1].Rect = new Rect(10, 160, 200, 100);
            windows[2].Rect = new Rect(250, 40, 500, 450);

            wrappingLabelStyle.wordWrap = true;
        }
        #endregion

        void OnFocus() {
            skin_ToolbarContainer = Resources.Load<GUISkin>("GUI Skins/Toolbar_Container");
            skin_ToolbarContent = Resources.Load<GUISkin>("GUI Skins/Toolbar_Content");
        }

        #region UI Functions
        protected void OnGUI() {
            GUI.skin = skin_ToolbarContainer;

            DrawGUI();
            //DrawWindowsGUI();
        }

        // Draw the UI using simle stylings
        private void DrawGUI() {
            EditorGUILayout.BeginHorizontal(GUI.skin.box);
            EditorGUILayout.BeginHorizontal(GUI.skin.button);

            // Animator
            selectedAnimator = DrawObjectField<Animator>(selectedAnimator);
            if (selectedAnimator != null) {
                GameObject inactiveObject = HasInactiveObject(selectedAnimator.gameObject);
                if (inactiveObject != null) {
                    GUILayout.Label(string.Format("The \"{0}\" object along the animator's path is disabled. Please enable it to continue.", inactiveObject.name), wrappingLabelStyle);
                } else if (selectedAnimator.runtimeAnimatorController == null) {
                    GUILayout.Label("The selected animator's controller is null.", wrappingLabelStyle);
                } else {
                    InitializeAnimationClips();
                }

                // Animation Clips
                if (animationClips != null && animationClips.Count != 0) {
                    selectedAnimation = Mathf.Clamp(EditorGUILayout.Popup(selectedAnimation, animationNames, GUILayout.Width(fieldWidth)), 0, animationClips.Count - 1);

                    AnimationClip newAnimationClip = animationClips[selectedAnimation];

                    if (newAnimationClip != null) {
                        if (selectedAnimationClip != newAnimationClip) {
                            selectedAnimationClip = newAnimationClip;
                            InitializeAnimationBindings();
                        }
                    }
                } else {
                    GUILayout.Label("The selected animator has no animations.");
                }
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndHorizontal();

            // Draw the animation object
            if (selectedAnimator != null && animationClips != null && animationClips.Count != 0) {
                scrollPosition = GUILayout.BeginScrollView(scrollPosition);

                if (animationObjectBindings != null) {
                    DisplayPaths();
                }

                GUILayout.EndScrollView();
            }
        }
        
        /// <summary>
        /// Draw the UI using EditorWindows
        /// </summary>
        public void DrawWindowsGUI() {
            if (selectedAnimator != null && windows[0].InError == false && windows[1].InError == false) {
                CurveUtils.DrawCurve(windows[0].Rect, windows[1].Rect, SIDE.bottom, SIDE.top);
                CurveUtils.DrawCurve(windows[1].Rect, windows[2].Rect);
            }

            BeginWindows();

            // Draw animator window         
            windows[0].Rect = GUILayout.Window(0, windows[0].Rect, DrawWindow, windows[0].Content);
            if (selectedAnimator == null || windows[0].InError || string.IsNullOrEmpty(windows[0].ErrorMessage) == false) {
                EndWindows();
                return;
            }

            // Draw animation clip window
            windows[1].Rect = GUILayout.Window(1, windows[1].Rect, DrawWindow, windows[1].Content);
            if (selectedAnimationClip == null || windows[1].InError) {
                EndWindows();
                return;
            }

            // Draw hierarchy window
            windows[2].Rect = GUILayout.Window(2, windows[2].Rect, DrawWindow, windows[2].Content);

            EndWindows();
        }
        #endregion

        #region Windows
        // Draw each individual window
        private void DrawWindow(int aWindowID) {
            switch (aWindowID) {
                case 0: // Animator Window
                    windows[aWindowID].ClearError();

                    if (selectedAnimator == null) {
                        GUI.color = Color.red;
                    } else if (windows[aWindowID].InError) {
                        GUI.color = Color.yellow;
                    }

                    selectedAnimator = DrawObjectField<Animator>(selectedAnimator);

                    if (selectedAnimator != null) {
                        GameObject inactiveObject = HasInactiveObject(selectedAnimator.gameObject);
                        if (inactiveObject != null) {
                            windows[aWindowID].SetError(string.Format("The \"{0}\" object along the animator's path is disabled. Please enable it to continue.", inactiveObject.name));
                        } else if (selectedAnimator.runtimeAnimatorController == null) {
                            windows[aWindowID].SetError("The selected animator's controller is null.");
                        } else {
                            InitializeAnimationClips();
                        }
                    }
                    
                    GUILayout.Label(windows[aWindowID].ErrorMessage, wrappingLabelStyle);

                    break;
                case 1: // Animations Window
                    if (animationClips == null || animationClips.Count == 0) {
                        windows[aWindowID].SetError("The selected animator has no animations.");
                    } else {
                        EditorGUILayout.BeginHorizontal();
                        selectedAnimation = Mathf.Clamp(EditorGUILayout.Popup(selectedAnimation, animationNames, GUILayout.Width(fieldWidth)), 0, animationClips.Count - 1);
                        EditorGUILayout.EndHorizontal();
                        
                        AnimationClip newAnimationClip = animationClips[selectedAnimation];

                        if (newAnimationClip != null) {
                            if (selectedAnimationClip != newAnimationClip) {
                                selectedAnimationClip = newAnimationClip;
                                InitializeAnimationBindings();
                            }
                        }
                    }

                    GUILayout.Label(windows[aWindowID].ErrorMessage, wrappingLabelStyle);

                    break;
                case 2: // Animation Components Window
                    scrollPosition = GUILayout.BeginScrollView(scrollPosition);

                    if (animationObjectBindings != null) {
                        DisplayPaths();
                    }

                    GUILayout.EndScrollView();

                    break;
            }

            GUI.DragWindow();
        }
        #endregion

        #region Animation Pathing
        /// <summary>
        /// Update the object used within the animation timeline (either with a new object or with the same object in a new hierarchical location)
        /// </summary>
        protected void UpdatePath(string aOldPath, string aNewPath) {
            if (animationObjectBindings.ContainsKey(aNewPath)) {
                throw new UnityException(string.Format("Path {0} already exists in that animation!", aNewPath));
            }

            AssetDatabase.StartAssetEditing();
            Undo.RecordObject(selectedAnimationClip, "Animation Hierarchy Change");

            foreach (EditorCurveBinding binding in animationObjectBindings[aOldPath]) {
                EditorCurveBinding bindingToUpdate = binding;
                AnimationCurve curve = AnimationUtility.GetEditorCurve(selectedAnimationClip, bindingToUpdate);
                ObjectReferenceKeyframe[] objectReferenceCurve = AnimationUtility.GetObjectReferenceCurve(selectedAnimationClip, bindingToUpdate);

                if (curve != null) {
                    AnimationUtility.SetEditorCurve(selectedAnimationClip, bindingToUpdate, null);
                } else {
                    AnimationUtility.SetObjectReferenceCurve(selectedAnimationClip, bindingToUpdate, null);
                }
                
                bindingToUpdate.path = aNewPath;

                if (curve != null) {
                    AnimationUtility.SetEditorCurve(selectedAnimationClip, bindingToUpdate, curve);
                } else {
                    AnimationUtility.SetObjectReferenceCurve(selectedAnimationClip, bindingToUpdate, objectReferenceCurve);
                }
            }

            AssetDatabase.StopAssetEditing();

            UpdateAnimationBindings(aOldPath, aNewPath);

            Repaint();
        }
        
        /// <summary>
        /// Display the paths for each of the objects used within the timeline of the selected animation
        /// </summary>
        private void DisplayPaths() {
            foreach (string path in objectPaths) {
                GameObject animationObject = FindObjectInRoot(path);

                EditorGUILayout.BeginHorizontal();

                GUI.color = (animationObject != null) ? Color.green : Color.red;
                GameObject newAnimationObject = EditorGUILayout.ObjectField(animationObject, typeof(GameObject), true, GUILayout.Width(fieldWidth)) as GameObject;
                GUI.color = defaultColor;

                GUILayout.Label("/" + path);

                EditorGUILayout.EndHorizontal();

                try {
                    if (animationObject != newAnimationObject) {
                        UpdatePath(path, GetChildPath(newAnimationObject));
                    }
                } catch (UnityException ex) {
                    Debug.LogError(ex.Message);
                }
            }
        }

        /// <summary>
        /// Get the path of the child object to update the hierarchy within the animation
        /// </summary>
        protected string GetChildPath(GameObject aObject, bool aSeparate = false) {
            if (aObject != selectedAnimator.gameObject) {
                if (aObject.transform.parent != null) {
                    return GetChildPath(aObject.transform.parent.gameObject, true) + aObject.name + (aSeparate ? "/" : string.Empty);
                } else {
                    throw new UnityException(string.Format("Object must belong to '{0}'!", selectedAnimator));
                }
            } else {
                return string.Empty;
            }
        }
        #endregion

        #region Animator/Animation Initialization
        /// <summary>
        /// Initializa the animation clips and continue doing so until the animation has at least one clip
        /// </summary>
        private void InitializeAnimationClips() {
            if (animationClips == null || animationClips.Count == 0) {
                animationClips = new List<AnimationClip>(selectedAnimator.runtimeAnimatorController.animationClips);

                animationNames = new string[animationClips.Count];
                for (int i = 0; i < animationNames.Length; i++) {
                    animationNames[i] = animationClips[i].name;
                }
            }
        }

        /// <summary>
        /// Initialize the animation bindings (animation timeline objects)
        /// </summary>
        private void InitializeAnimationBindings() {
            if (animationObjectBindings == null || objectPaths == null) {
                animationObjectBindings = SetAnimationBindings();
                objectPaths = new List<string>(animationObjectBindings.Keys);
            }
        }

        /// <summary>
        /// Update the animation bindings after they change within the Window
        /// </summary>
        private void UpdateAnimationBindings(string aOldPath, string aNewPath) {
            int index = objectPaths.FindIndex(p => p == aOldPath);
            objectPaths[index] = aNewPath;

            animationObjectBindings = SetAnimationBindings();
        }

        /// <summary>
        /// Taking the animation information create a list and dictionary of animation bindings to display on the Editor Window
        /// </summary>
        private MultiDictionary<string, EditorCurveBinding> SetAnimationBindings() {
            MultiDictionary<string, EditorCurveBinding> newDictionary = new MultiDictionary<string, EditorCurveBinding>();

            List<EditorCurveBinding> animationCurves = new List<EditorCurveBinding>();
            animationCurves.AddRange(AnimationUtility.GetCurveBindings(selectedAnimationClip));
            animationCurves.AddRange(AnimationUtility.GetObjectReferenceCurveBindings(selectedAnimationClip));

            foreach (EditorCurveBinding curveData in animationCurves) {
                newDictionary.Add(curveData.path, curveData);
            }

            return newDictionary;
        }
        #endregion

        #region Utility Functions
        /// <summary>
        /// Draw an object field of given type with given color.
        /// </summary>
        private T DrawObjectField<T>(Object aObject, Color aColor = default(Color)) where T : Object {
            T drawnObject = null;

            if (default(Color) != aColor) {
                GUI.color = aColor;
            }

            drawnObject = EditorGUILayout.ObjectField(aObject, typeof(Animator), true, GUILayout.Width(fieldWidth)) as T;

            return drawnObject;
        }

        /// <summary>
        /// Check the object recursively (up to the objects root) to ensure the whole object is active. If it isn't we can't modify it.
        /// </summary>
        private GameObject HasInactiveObject(GameObject aObject) {
            GameObject inactiveObject = (aObject.activeSelf == false) ? aObject : null;

            if (inactiveObject == null) {
                Transform parent = aObject.transform.parent;

                while (parent != null) {
                    if (parent.gameObject.activeSelf == false) {
                        inactiveObject = parent.gameObject;
                        break;
                    }
                    
                    parent = parent.parent;
                }
            }

            return inactiveObject;
        }

        protected GameObject FindObjectInRoot(string aPath) {
            Transform child = selectedAnimator.transform.Find(aPath);
            return (child != null) ? child.gameObject : null;
        }
        #endregion

    }

}
