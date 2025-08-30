using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AddressablesCacheDebugger : EditorWindow
{
    [MenuItem("Window/Addressables Cache Debugger")]
    public static void ShowWindow()
    {
        GetWindow<AddressablesCacheDebugger>("Addressables Cache");
    }

    void OnGUI()
    {
        EditorGUILayout.LabelField("Unity Caching Info", EditorStyles.boldLabel);

        EditorGUILayout.Space(5);

        // Current cache path
        EditorGUILayout.LabelField("Current cache path:");

        EditorGUILayout.TextField(Caching.currentCacheForWriting.path);

        EditorGUILayout.Space(10);

        // All cache paths
        EditorGUILayout.LabelField("All cache paths:");

        List<string> cachePaths = new();

        Caching.GetAllCachePaths(cachePaths);
        foreach (var path in cachePaths)
        {
            EditorGUILayout.BeginHorizontal();

            GUILayout.TextField(path);
            if (GUILayout.Button("<-- Clear this Cache"))
            {
                bool cleared = 
                    Caching.currentCacheForWriting.ClearCache();
                Debug.Log("Cleared cache: " + cleared);
            }

            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.Space(10);

        if (GUILayout.Button("Clear All Caches"))
        {
            bool cleared = Caching.ClearCache();
            Debug.Log("Cleared cache: " + cleared);
        }
    }
}