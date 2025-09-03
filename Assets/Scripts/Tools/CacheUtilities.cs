using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
public class CacheTools : ScriptableObject
{
    [MenuItem("Tools/Cache/Clean Cache")]
    public static void CleanCache()
    {
        if (Caching.ClearCache())
        {
            Debug.LogWarning("Successfully cleaned all caches.");
        }
        else
        {
            Debug.LogWarning("Cache was in use.");
        }
    }
}
#endif