using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Collections.Generic;

namespace Resource.Editor {

    public class ComponentUsageEditor : EditorWindow {
        [MenuItem("Window/Components/Search")]
        static void Init() {
            ComponentUsageEditor window = GetWindow<ComponentUsageEditor>();
            window.Show();
            window.position = new Rect(20, 80, 550, 500);
        }

        private readonly string[] modes = new string[] { "Search for component usage", "Search for missing components" };
        private readonly string[] checkType = new string[] { "Check single component", "Check all components" };

        private List<string> listResult;
        private List<ComponentNames> prefabComponents, notUsedComponents, addedComponents, existingComponents, sceneComponents;
        private int editorMode, selectedCheckType;
        private MonoScript targetComponent;
        private string componentName = "";

        private bool showPrefabs, showAdded, showScene, showUnused = true;
        private Vector2 scroll, scroll1, scroll2, scroll3, scroll4;

        #region Component Names Class
        private class ComponentNames {
            public string componentName;
            public string namespaceName;
            public string assetPath;
            public List<string> usageSource;
            public ComponentNames(string comp, string space, string path) {
                this.componentName = comp;
                this.namespaceName = space;
                this.assetPath = path;
                this.usageSource = new List<string>();
            }
            public override bool Equals(object obj) {
                return ((ComponentNames) obj).componentName == componentName && ((ComponentNames) obj).namespaceName == namespaceName;
            }
            public override int GetHashCode() {
                return componentName.GetHashCode() + namespaceName.GetHashCode();
            }
        }
        #endregion

        #region OnGUI
        void OnGUI() {
            GUILayout.Space(3);
            int oldValue = GUI.skin.window.padding.bottom;
            GUI.skin.window.padding.bottom = -20;
            Rect windowRect = GUILayoutUtility.GetRect(1, 17);
            windowRect.x += 4;
            windowRect.width -= 7;
            editorMode = GUI.SelectionGrid(windowRect, editorMode, modes, 2, "Window");
            GUI.skin.window.padding.bottom = oldValue;

            switch (editorMode) {
                case 0: // Search for component usage
                    SearchForComponentUsage();
                    break;
                case 1: // Search for missing components
                    SearchForMissingComponents();
                    break;
            }

            if (editorMode == 1 || selectedCheckType == 0) {
                if (listResult != null) {
                    if (listResult.Count == 0) {
                        GUILayout.Label(editorMode == 0 ? (componentName == "" ? "Choose a component" : "No prefabs use component " + componentName) : ("No prefabs have missing components!\nClick Search to check again"));
                    } else {
                        GUILayout.Label(editorMode == 0 ? ("The following prefabs use component " + componentName + ":") : ("The following prefabs have missing components:"));
                        scroll = GUILayout.BeginScrollView(scroll);
                        foreach (string s in listResult) {
                            GUILayout.BeginHorizontal();
                            GUILayout.Label(s, GUILayout.Width(position.width / 1.25f));
                            if (GUILayout.Button("Select", GUILayout.MinWidth(100.0f))) {
                                Selection.activeObject = AssetDatabase.LoadMainAssetAtPath(s);
                            }
                            GUILayout.EndHorizontal();
                        }
                        GUILayout.EndScrollView();
                    }
                }
            } else {
                showPrefabs = GUILayout.Toggle(showPrefabs, "Show prefab components");
                if (showPrefabs) {
                    GUILayout.Label("The following components are attatched to prefabs:");
                    DisplayResults(ref scroll1, ref prefabComponents);
                }
                showAdded = GUILayout.Toggle(showAdded, "Show AddComponent arguments");
                if (showAdded) {
                    GUILayout.Label("The following components are AddComponent arguments:");
                    DisplayResults(ref scroll2, ref addedComponents);
                }
                showScene = GUILayout.Toggle(showScene, "Show Scene-used components");
                if (showScene) {
                    GUILayout.Label("The following components are used by scene objects:");
                    DisplayResults(ref scroll3, ref sceneComponents);
                }
                showUnused = GUILayout.Toggle(showUnused, "Show Unused Components");
                if (showUnused) {
                    GUILayout.Label("The following components are not used by prefabs, by AddComponent, OR in any scene:");
                    DisplayResults(ref scroll4, ref notUsedComponents);
                }
            }
        }
        #endregion

        #region Search Functions
        private void SearchForMissingComponents() {
            if (GUILayout.Button("Search!")) {
                string[] allPrefabs = GetAllPrefabs();
                listResult = new List<string>();
                foreach (string prefab in allPrefabs) {
                    UnityEngine.Object o = AssetDatabase.LoadMainAssetAtPath(prefab);
                    GameObject go;
                    try {
                        go = (GameObject) o;
                        Component[] components = go.GetComponentsInChildren<Component>(true);
                        foreach (Component c in components) {
                            if (c == null) {
                                listResult.Add(prefab);
                            }
                        }
                    } catch {
                        Debug.Log("For some reason, prefab " + prefab + " won't cast to GameObject");
                    }
                }
            }
        }

        private void SearchForComponentUsage() {
            selectedCheckType = GUILayout.SelectionGrid(selectedCheckType, checkType, 2, "Toggle");
            GUI.enabled = selectedCheckType == 0;
            targetComponent = (MonoScript) EditorGUILayout.ObjectField(targetComponent, typeof(MonoScript), false);
            GUI.enabled = true;

            if (GUILayout.Button("Check component usage")) {
                AssetDatabase.SaveAssets();
                switch (selectedCheckType) {
                    case 0:
                        componentName = targetComponent.name;
                        string targetPath = AssetDatabase.GetAssetPath(targetComponent);
                        string[] allPrefabs = GetAllPrefabs();
                        listResult = new List<string>();
                        foreach (string prefab in allPrefabs) {
                            string[] single = new string[] { prefab };
                            string[] dependencies = AssetDatabase.GetDependencies(single);
                            foreach (string dependedAsset in dependencies) {
                                if (dependedAsset == targetPath) {
                                    listResult.Add(prefab);
                                }
                            }
                        }
                        break;
                    case 1:
                        List<string> scenesToLoad = new List<string>();
                        existingComponents = new List<ComponentNames>();
                        prefabComponents = new List<ComponentNames>();
                        notUsedComponents = new List<ComponentNames>();
                        addedComponents = new List<ComponentNames>();
                        sceneComponents = new List<ComponentNames>();

                        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) {
                            string projectPath = Application.dataPath;
                            projectPath = projectPath.Substring(0, projectPath.IndexOf("Assets"));

                            string[] allAssets = AssetDatabase.GetAllAssetPaths();

                            foreach (string asset in allAssets) {
                                int indexCS = asset.IndexOf(".cs");
                                int indexJS = asset.IndexOf(".js");
                                if (indexCS != -1 || indexJS != -1) {
                                    ComponentNames newComponent = new ComponentNames(NameFromPath(asset), "", asset);
                                    try {
                                        System.IO.FileStream FS = new System.IO.FileStream(projectPath + asset, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.ReadWrite);
                                        System.IO.StreamReader SR = new System.IO.StreamReader(FS);
                                        string line;
                                        while (!SR.EndOfStream) {
                                            line = SR.ReadLine();
                                            int index1 = line.IndexOf("namespace");
                                            int index2 = line.IndexOf("{");
                                            if (index1 != -1 && index2 != -1) {
                                                line = line.Substring(index1 + 9);
                                                index2 = line.IndexOf("{");
                                                line = line.Substring(0, index2);
                                                line = line.Replace(" ", "");
                                                newComponent.namespaceName = line;
                                            }
                                        }
                                    } catch {
                                    }

                                    existingComponents.Add(newComponent);

                                    try {
                                        System.IO.FileStream FS = new System.IO.FileStream(projectPath + asset, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.ReadWrite);
                                        System.IO.StreamReader SR = new System.IO.StreamReader(FS);

                                        string line;
                                        int lineNum = 0;
                                        while (!SR.EndOfStream) {
                                            lineNum++;
                                            line = SR.ReadLine();
                                            int index = line.IndexOf("AddComponent");
                                            if (index != -1) {
                                                line = line.Substring(index + 12);
                                                if (line[0] == '(') {
                                                    line = line.Substring(1, line.IndexOf(')') - 1);
                                                } else if (line[0] == '<') {
                                                    line = line.Substring(1, line.IndexOf('>') - 1);
                                                } else {
                                                    continue;
                                                }
                                                line = line.Replace(" ", "");
                                                line = line.Replace("\"", "");
                                                index = line.LastIndexOf('.');
                                                ComponentNames newComp;
                                                if (index == -1) {
                                                    newComp = new ComponentNames(line, "", "");
                                                } else {
                                                    newComp = new ComponentNames(line.Substring(index + 1, line.Length - (index + 1)), line.Substring(0, index), "");
                                                }
                                                string pName = asset + ", Line " + lineNum;
                                                newComp.usageSource.Add(pName);
                                                index = addedComponents.IndexOf(newComp);
                                                if (index == -1) {
                                                    addedComponents.Add(newComp);
                                                } else {
                                                    if (!addedComponents[index].usageSource.Contains(pName)) addedComponents[index].usageSource.Add(pName);
                                                }
                                            }
                                        }
                                    } catch {
                                    }
                                }
                                int indexPrefab = asset.IndexOf(".prefab");

                                if (indexPrefab != -1) {
                                    string[] single = new string[] { asset };
                                    string[] dependencies = AssetDatabase.GetDependencies(single);
                                    foreach (string dependedAsset in dependencies) {
                                        if (dependedAsset.IndexOf(".cs") != -1 || dependedAsset.IndexOf(".js") != -1) {
                                            ComponentNames newComponent = new ComponentNames(NameFromPath(dependedAsset), GetNamespaceFromPath(dependedAsset), dependedAsset);
                                            int index = prefabComponents.IndexOf(newComponent);
                                            if (index == -1) {
                                                newComponent.usageSource.Add(asset);
                                                prefabComponents.Add(newComponent);
                                            } else {
                                                if (!prefabComponents[index].usageSource.Contains(asset)) prefabComponents[index].usageSource.Add(asset);
                                            }
                                        }
                                    }
                                }
                                int indexUnity = asset.IndexOf(".unity");
                                if (indexUnity != -1) {
                                    scenesToLoad.Add(asset);
                                }
                            }

                            for (int i = addedComponents.Count - 1; i > -1; i--) {
                                addedComponents[i].assetPath = GetPathFromNames(addedComponents[i].namespaceName, addedComponents[i].componentName);
                                if (addedComponents[i].assetPath == "") addedComponents.RemoveAt(i);

                            }

                            foreach (string scene in scenesToLoad) {
                                EditorSceneManager.OpenScene(scene);
                                GameObject[] sceneGOs = GetAllObjectsInScene();
                                foreach (GameObject g in sceneGOs) {
                                    Component[] comps = g.GetComponentsInChildren<Component>(true);
                                    foreach (Component c in comps) {

                                        if (c != null && c.GetType() != null && c.GetType().BaseType != null && c.GetType().BaseType == typeof(MonoBehaviour)) {
                                            SerializedObject so = new SerializedObject(c);
                                            SerializedProperty p = so.FindProperty("m_Script");
                                            string path = AssetDatabase.GetAssetPath(p.objectReferenceValue);
                                            ComponentNames newComp = new ComponentNames(NameFromPath(path), GetNamespaceFromPath(path), path);
                                            newComp.usageSource.Add(scene);
                                            int index = sceneComponents.IndexOf(newComp);
                                            if (index == -1) {
                                                sceneComponents.Add(newComp);
                                            } else {
                                                if (!sceneComponents[index].usageSource.Contains(scene)) sceneComponents[index].usageSource.Add(scene);
                                            }
                                        }
                                    }
                                }
                            }

                            foreach (ComponentNames c in existingComponents) {
                                if (addedComponents.Contains(c)) continue;
                                if (prefabComponents.Contains(c)) continue;
                                if (sceneComponents.Contains(c)) continue;
                                notUsedComponents.Add(c);
                            }

                            addedComponents.Sort(SortAlphabetically);
                            prefabComponents.Sort(SortAlphabetically);
                            sceneComponents.Sort(SortAlphabetically);
                            notUsedComponents.Sort(SortAlphabetically);
                        }
                        break;
                }
            }
        }
        #endregion

        #region Display Functions
        private void DisplayResults(ref Vector2 scroller, ref List<ComponentNames> list) {
            if (list == null) return;
            scroller = GUILayout.BeginScrollView(scroller);
            foreach (ComponentNames c in list) {
                GUILayout.BeginHorizontal();
                GUILayout.Label(c.assetPath, GUILayout.Width(position.width / 5 * 4));
                if (GUILayout.Button("Select", GUILayout.Width(position.width / 5 - 30))) {
                    Selection.activeObject = AssetDatabase.LoadMainAssetAtPath(c.assetPath);
                }
                GUILayout.EndHorizontal();
                if (c.usageSource.Count == 1) {
                    GUILayout.Label("   In 1 Place: " + c.usageSource[0]);
                }
                if (c.usageSource.Count > 1) {
                    GUILayout.Label("   In " + c.usageSource.Count + " Places: " + c.usageSource[0] + ", " + c.usageSource[1] + (c.usageSource.Count > 2 ? ", ..." : ""));
                }
            }
            GUILayout.EndScrollView();
        }
        #endregion

        #region Get Functions
        private GameObject[] GetAllObjectsInScene() {
            List<GameObject> objectsInScene = new List<GameObject>();
            GameObject[] sceneObjects = Resources.FindObjectsOfTypeAll<GameObject>();
            foreach (GameObject sceneObject in sceneObjects) {
                string assetPath = AssetDatabase.GetAssetPath(sceneObject.transform.root.gameObject);
                if (string.IsNullOrEmpty(assetPath) == false) {
                    continue;
                }

                objectsInScene.Add(sceneObject);
            }

            return objectsInScene.ToArray();
        }

        private string GetNamespaceFromPath(string path) {
            foreach (ComponentNames c in existingComponents) {
                if (c.assetPath == path) {
                    return c.namespaceName;
                }
            }

            return string.Empty;
        }

        private string GetPathFromNames(string space, string name) {
            ComponentNames test = new ComponentNames(name, space, "");
            int index = existingComponents.IndexOf(test);
            if (index != -1) {
                return existingComponents[index].assetPath;
            }

            return string.Empty;
        }

        public static string[] GetAllPrefabs() {
            string[] temp = AssetDatabase.GetAllAssetPaths();
            List<string> result = new List<string>();
            foreach (string s in temp) {
                if (s.Contains(".prefab")) result.Add(s);
            }

            return result.ToArray();
        }
        #endregion

        #region Utility Functions
        private string NameFromPath(string s) {
            s = s.Substring(s.LastIndexOf('/') + 1);
            return s.Substring(0, s.Length - 3);
        }

        private int SortAlphabetically(ComponentNames a, ComponentNames b) {
            return a.assetPath.CompareTo(b.assetPath);
        }
        #endregion

    }

}


/*

    using UnityEngine;
using UnityEditor;

public class FindMissingScripts : EditorWindow {
	static int go_count = 0, components_count = 0, missing_count = 0;
	
	[MenuItem("Window/FindMissingScripts")]
	public static void ShowWindow() {
		EditorWindow.GetWindow(typeof(FindMissingScripts));
	}
	
	public void OnGUI() {
		if (GUILayout.Button("Find Missing Scripts in selected GameObjects")) {
			FindInSelected();
		}
	}

	private static void FindInSelected() {
		GameObject[] go = Selection.gameObjects;
		go_count = 0;
		components_count = 0;
		missing_count = 0;
		foreach (GameObject g in go) {
			FindInGO(g);
		}
		Debug.Log(string.Format("Searched {0} GameObjects, {1} components, found {2} missing", go_count, components_count, missing_count));
	}
	
	private static void FindInGO(GameObject g) {
		go_count++;
		Component[] components = g.GetComponents<Component>();
		for (int i = 0; i < components.Length; i++) {
			components_count++;
			if (components[i] == null) {
				missing_count++;
				string s = g.name;
				Transform t = g.transform;
				while (t.parent != null) {
					s = t.parent.name + "/" + s;
					t = t.parent;
				}
				Debug.Log(s + " has an empty script attached in position: " + i, g);
			}
		}
		// Now recurse through each child GO (if there are any):
		foreach (Transform childT in g.transform) {
			//Debug.Log("Searching " + childT.name  + " " );
			FindInGO(childT.gameObject);
		}
	}
}

    */
