using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

/// <summary>
/// Find references to the currently selected object
/// </summary>
public class ReferenceFinder : EditorWindow {
    private Vector2 scrollPosition = Vector2.zero;
    private List<GameObject> references = new List<GameObject>();
    private List<string> paths = null;

    private const string arrowUnicode = "\u25B6";

    // Used to queue a call to FindObjectReferences() to avoid doing it mid-layout
    private Object toFindAfterLayout = null;

    [MenuItem("Assets/Find References", false, 39)]
    static void FindObjectReferences() {
        ReferenceFinder window = GetWindow<ReferenceFinder>(true, "Find References", true);
        window.FindObjectReferences(Selection.activeObject);
    }

    #region OnGUI
    void OnGUI() {
        GUILayout.Space(5);
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        GUILayout.BeginHorizontal();
        GUILayout.Label("Found: " + references.Count);
        if (GUILayout.Button("Clear", EditorStyles.miniButton)) {
            references.Clear();
        }
        GUILayout.EndHorizontal();

        GUILayout.Space(5);

        for (int i = references.Count - 1; i >= 0; i--) {
            LayoutItem(i, references[i]);
        }

        EditorGUILayout.EndScrollView();

        if (toFindAfterLayout != null) {
            FindObjectReferences(toFindAfterLayout);
            toFindAfterLayout = null;
        }
    }

    /// <summary>Layout item within the window</summary>
    private void LayoutItem(int aIndex, Object aObject) {
        if (aObject != null) {
            GUIStyle style = EditorStyles.miniButtonLeft;
            style.alignment = TextAnchor.MiddleLeft;

            GUILayout.BeginHorizontal();

            // Button of referencing object
            if (GUILayout.Button(aObject.name, style)) {
                Selection.activeObject = aObject;
                EditorGUIUtility.PingObject(aObject);
            }

            // Arrow to find references of a referencing object
            if (GUILayout.Button(arrowUnicode, EditorStyles.miniButtonRight, GUILayout.MaxWidth(20))) {
                toFindAfterLayout = aObject;
            }

            GUILayout.EndHorizontal();
        }
    }
    #endregion

    #region Finding
    /// <summary>Finds references to passed objects and puts them in references</summary>
    private void FindObjectReferences(Object aToFind) {
        EditorUtility.DisplayProgressBar("Searching", "Generating file paths", 0.0f);

        // Get all prefabs in the project
        if (paths == null) {
            paths = new List<string>();
            GetFilePaths("Assets", ".prefab", ref paths);
        }

        int pathsCount = paths.Count;

        float progress = 0;
        int updateIteration = Mathf.Max(1, pathsCount / 100); // So we only update progress bar 100 times, not for every item

        Object[] searchArray = new Object[1];
        references.Clear();

        // Loop through all files, and add any that have the selected object in it's list of dependencies
        for (int i = 0; i < pathsCount; ++i) {
            searchArray[0] = AssetDatabase.LoadMainAssetAtPath(paths[i]);
            if (searchArray.Length > 0 && searchArray[0] != aToFind) {
                Object[] dependencies = EditorUtility.CollectDependencies(searchArray);
                if (ArrayUtility.Contains(dependencies, aToFind)) {
                    references.Add(searchArray[0] as GameObject);
                }

            }

            if (i % updateIteration == 0) {
                progress += 0.01f;
                EditorUtility.DisplayProgressBar("Searching", "Searching dependencies", progress);
            }
        }

        EditorUtility.DisplayProgressBar("Searching", "Removing redundant references", 1);

        // Go through the references and remove any that are not direct dependencies.
        for (int i = references.Count - 1; i >= 0; i--) {
            searchArray[0] = references[i];
            Object[] dependencies = EditorUtility.CollectDependencies(searchArray);

            bool shouldRemove = false;

            for (int j = 0; j < dependencies.Length; j++) {
                shouldRemove = (references.Find(item => item == dependencies[j] && item != searchArray[0]) != null);

                if (shouldRemove) {
                    break;
                }
            }

            if (shouldRemove) {
                references.RemoveAt(i);
            }
        }

        EditorUtility.ClearProgressBar();
    }
    #endregion

    #region Utility Functions
    /// <summary>Recursively find all file paths with a particular extention in a directory</summary>
    private void GetFilePaths(string aStartDirectory, string aExtension, ref List<string> aPaths) {
        try {
            // Add any file paths with the provided extention
            string[] files = Directory.GetFiles(aStartDirectory);
            foreach (string file in files) {
                if (file.EndsWith(aExtension)) {
                    aPaths.Add(file);
                }
            }

            // Recursively search all directories
            string[] directories = Directory.GetDirectories(aStartDirectory);
            foreach (string directory in directories) {
                GetFilePaths(directory, aExtension, ref aPaths);
            }
        } catch (System.Exception e) {
            Debug.LogError(e.Message);
        }
    }
    #endregion

}
