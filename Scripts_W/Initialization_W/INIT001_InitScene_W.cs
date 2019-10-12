using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Wichtel{
public class INIT001_InitScene_W : SerializedMonoBehaviour
{
    [SerializeField] public List<IInitSingletons>   singletonInits = new List<IInitSingletons>();
    [SerializeField] public List<IInitSelf>         selfInits      = new List<IInitSelf>();
    [SerializeField] public List<IInitDependencies> dependentInits = new List<IInitDependencies>();

    [SerializeField, ShowIf("ShowIfLinkedWithInitSaveSystem"), InfoBox("Connected with Save System Initializer!"), HideLabel, DisplayAsString]
    public INIT002_InitSaveSystem_W initSaveSystem;
    
    private void Awake()
    {
        gameObject.SetActive(false); //wird der Initializer hier inaktiv gesetzt, wird kein Awake, Start, OnEnable der Kinder aufgerufen. Nichtmal OnDisable!
        //Man könnte also hier die Hierarchy inaktiv setzen, dann alles initialisieren (bei Spielstart) und dann erst zu einem späteren Zeitpunkt aktivieren.

        ClearLists();

        //-- Step 1 : Initialize Singletons --
        GetSingletons(transform);
        InitSingletons();

        //-- Step 2 : Initialize Save Data (if there is a Saving System hooked up)
        if (initSaveSystem != null)
        {
            initSaveSystem.GetSaveables(transform);
            initSaveSystem.InitSaveables();

            initSaveSystem.GetStoreables(transform);
            initSaveSystem.InitStoreables();
        }

        //Step 3 : Initialize Self and Dependant
        GetSelfAndDependent(transform);
        InitSelfAndDepentents();

        System.GC.Collect(); //Force Garbage Collection

        gameObject.SetActive(true); //Initialization is finished --> activate Scene
    }

    //--- Init Singletons ---
    #region Init Singletons
    private void GetSingletons(Transform _root)
    {
        var _singleton = _root.GetComponents<IInitSingletons>();
        singletonInits.AddRange(_singleton);

        foreach(Transform t in _root)
        {
            if(t == _root) continue;  //make sure you don't initialize the existing transform
            GetSingletons(t);        //initialize this Transform's children recursively
        }
    }
    private void InitSingletons()
    {
        foreach(var _singleton in singletonInits)
        {
            _singleton.InitSingleton();
        }
    }
    #endregion
    //--- ---

    //--- Init Self and Dependents ---
    #region Init Self and Dependents
    private void GetSelfAndDependent(Transform _root)
    {
        var _self      = _root.GetComponents<IInitSelf>();
        var _dependent = _root.GetComponents<IInitDependencies>();

        selfInits.AddRange(_self);
        dependentInits.AddRange(_dependent);

        foreach(Transform t in _root)
        {
            if(t == _root) continue;  //make sure you don't initialize the existing transform
            GetSelfAndDependent(t);  //initialize this Transform's children recursively
        }
    }
    private void InitSelfAndDepentents()
    {
        foreach(var _self in selfInits)
        {
            _self.InitSelf();
        }
        foreach(var _dependent in dependentInits)
        {
            _dependent.InitDependencies();
        }
    }
    #endregion
    //--- ---

    private void ClearLists()
    {
        singletonInits.Clear();
        selfInits.Clear();
        dependentInits.Clear();

        if (initSaveSystem != null) initSaveSystem.ClearLists();
    }
    
    //--- Odin Methods ---
    [UsedImplicitly]
    private bool ShowIfLinkedWithInitSaveSystem()
    {
        return (initSaveSystem != null);
    }
    //--- ---
}
}