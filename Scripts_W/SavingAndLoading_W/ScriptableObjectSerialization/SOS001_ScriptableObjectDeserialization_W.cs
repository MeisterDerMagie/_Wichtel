using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector;

namespace Wichtel{
public class SOS001_ScriptableObjectDeserialization_W : SerializedMonoBehaviour, IInitSingletons
{
    [SerializeField] private string[] foldersToSearch = {"Assets/ScriptableObjects"};
    [ReadOnly, SerializeField] private Dictionary<string, ScriptableObject> scriptableObjectReferences = new Dictionary<string, ScriptableObject>();
    
    //--- Singleton Behaviour ---
    #region Singleton

    private static SOS001_ScriptableObjectDeserialization_W instance_;
    public static SOS001_ScriptableObjectDeserialization_W Instance 
        => instance_ == null ? FindObjectOfType<SOS001_ScriptableObjectDeserialization_W>() : instance_;

    public void InitSingleton()
    {
        if (instance_ == null)
            instance_ = this;
        else
            Destroy(gameObject);
    }

    #endregion
    //--- ---


    public T DeserializeScriptableObject<T>(string _nameOfScriptableObject) where T : ScriptableObject
    {
        if(scriptableObjectReferences.ContainsKey(_nameOfScriptableObject))
        {
            return (T)scriptableObjectReferences[_nameOfScriptableObject];
        }
        else
        {
            return null;
        }
    }

    #if UNITY_EDITOR
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    private static IEnumerable<T> GetAllInstances<T>() where T : ScriptableObject
    {
        var _guids = AssetDatabase.FindAssets("t:"+ typeof(T).Name, Instance.foldersToSearch);  //FindAssets uses tags check documentation for more info
        var _a = new T[_guids.Length];
        for(var i = 0; i < _guids.Length; i++)         //probably could get optimized 
        {
            var _path = AssetDatabase.GUIDToAssetPath(_guids[i]);
            _a[i] = AssetDatabase.LoadAssetAtPath<T>(_path);
        }

        return _a;
    }

    private void OnValidate()
    {
        IEnumerable _allScriptableObjects = GetAllInstances<ScriptableObject>();

        scriptableObjectReferences.Clear();

        foreach(ScriptableObject _so in _allScriptableObjects)
        {
            if(!scriptableObjectReferences.ContainsKey(_so.name)) scriptableObjectReferences.Add(_so.name, _so);
        }
    }
    #endif
}
}