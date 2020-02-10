using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Wichtel {
#if UNITY_EDITOR
public class UT_EditorUtilities_W
{
    public static List<T> GetAssets<T>(string _filter /*z.B. t:prefab*/) where T : ScriptableObject
    {
        string[] foldersToSearch = {"Assets"};
        return GetAssets<T>(foldersToSearch, _filter);
    }
    
    public static List<T> GetAssets<T>(string[] _foldersToSearch, string _filter) where T : UnityEngine.Object
    {
        string[] guids = AssetDatabase.FindAssets(_filter, _foldersToSearch);
        List<T> a = new List<T>();
        for (int i = 0; i < guids.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[i]);
            a.Add(AssetDatabase.LoadAssetAtPath<T>(path));
        }
        return a;
    }
}
#endif
}