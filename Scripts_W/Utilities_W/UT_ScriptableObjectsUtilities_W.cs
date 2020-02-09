using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Wichtel
{
#if UNITY_EDITOR
public class UT_ScriptableObjectsUtilities_W
{
    //Finde alle ScriptableObjects eines Typs im Projekt
    public static List<T> GetScriptableObjectInstances<T>() where T : ScriptableObject
    {
        string[] foldersToSearch = {"Assets"};
        return GetScriptableObjectInstances<T>(foldersToSearch);
    }
    
    public static List<T> GetScriptableObjectInstances<T>(string[] _foldersToSearch) where T : UnityEngine.Object
    {
        string[] guids = AssetDatabase.FindAssets("t:" + typeof(T).Name, _foldersToSearch); //FindAssets uses tags check documentation for more info
        List<T> a = new List<T>();
        for (int i = 0; i < guids.Length; i++) //probably could get optimized 
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[i]);
            a.Add(AssetDatabase.LoadAssetAtPath<T>(path));
        }

        return a;
    }
}
#endif
}