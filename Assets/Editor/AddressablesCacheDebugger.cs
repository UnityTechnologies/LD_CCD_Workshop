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
        GUILayout.Label("Unity Caching Info", EditorStyles.boldLabel);

        GUILayout.Space(5);

        // Current cache path
        GUILayout.Label("Current cache path:");
        GUILayout.TextField(Caching.currentCacheForWriting.path);

        GUILayout.Space(10);

        // All cache paths
        GUILayout.Label("All cache paths:");

        List<string> cachePaths = new ();

        Caching.GetAllCachePaths(cachePaths);
        foreach (var path in cachePaths)
        {
            GUILayout.TextField(path);
        }

        GUILayout.Space(10);

        if (GUILayout.Button("Clear All Caches"))
        {
            bool cleared = Caching.ClearCache();
            Debug.Log("Cleared cache: " + cleared);
        }
    }
}
