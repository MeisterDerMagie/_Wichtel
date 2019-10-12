using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Wichtel{
    [RequireComponent(typeof(INIT001_InitScene_W))]
public class INIT002_InitSaveSystem_W : SerializedMonoBehaviour
{
    [SerializeField] public List<ISaveable>  saveableInits  = new List<ISaveable>();
    [SerializeField] public List<IStoreable> storeableInits = new List<IStoreable>();
    
    
    //--- Init Saveables ---
    #region Init Saveables
    public void GetSaveables(Transform _root)
    {
        var _saveable  = _root.GetComponents<ISaveable>();
        saveableInits.AddRange(_saveable);

        foreach(Transform t in _root)
        {
            if(t == _root) continue;  //make sure you don't initialize the existing transform
            GetSaveables(t);         //initialize this Transform's children recursively
        }
    }    
    public void InitSaveables()
    {
        foreach(var _saveable in saveableInits)
        {
            _saveable.InitSaveable();
        }
    }
    #endregion
    //--- ---

    //--- Init Storeables ---
    #region Init Storeables
    public void GetStoreables(Transform _root)
    {
        var _storeable = _root.GetComponents<IStoreable>();
        storeableInits.AddRange(_storeable);

        foreach(Transform t in _root)
        {
            if(t == _root) continue;  //make sure you don't initialize the existing transform
            GetStoreables(t);         //initialize this Transform's children recursively
        }
    }
    public void InitStoreables()
    {
        foreach(var _storeable in storeableInits)
        {
            _storeable.InitStoreable();
        }
    }
    #endregion
    //--- ---
    
    
    public void ClearLists()
    {
        saveableInits.Clear();
        storeableInits.Clear();
    }
    
    private void OnValidate()
    {
        GetComponent<INIT001_InitScene_W>().initSaveSystem = this;
    }
}
}