using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Wichtel{
    [RequireComponent(typeof(PMAN001_PrefabManager_W))]
public class SAL004_SaveableManager_W : SerializedMonoBehaviour, ISaveable
{
    //--- DATA ---
    #region DATA

    [SerializeField, ReadOnly] private Guid id;
    public Guid ID { get{return id;} set{id = value;} }
    
    #endregion
    //--- ---

    //--- References ---
    #region References
    [ReadOnly, SerializeField] private PMAN001_PrefabManager_W prefabManager;
    public Dictionary<Guid, SAL002_SaveableObject_W> saveableObjects = new Dictionary<Guid, SAL002_SaveableObject_W>();
    #endregion
    //--- ---

    //--- ID generieren ---
    [Button(ButtonSizes.Medium), GUIColor(0, 1, 0)]
    [ShowIf("HasNoID", true)]
    [PropertyOrder(0)]
    public void GenerateUniqueID()
    {
        //ID001_IDManager.Instance.RegisterSaveableManagerID(this);
    }

    //--- Komponente entfernen ---
    [Button(ButtonSizes.Medium), GUIColor(0.84f, 0.36f, 0.26f)]
    [HorizontalGroup("RemoveComponent", PaddingLeft = 0.35f, PaddingRight = 0.35f)]
    [ShowIf("UserNotClickedRemove", true)]
    [PropertyOrder(10)]
    public void RemoveComponent()
    {
        userClickedRemove = true;
    }

    [Button(ButtonSizes.Medium), GUIColor(0.84f, 0.36f, 0.26f)]
    [HorizontalGroup("Are you sure?", MaxWidth = 100)]
    [ShowIf("userClickedRemove", true)]
    [PropertyOrder(11)]
    public void Yes()
    {
        Terminate();
        DestroyImmediate(this);
    }
    
    [Button(ButtonSizes.Medium)]
    [HorizontalGroup("Are you sure?", MaxWidth = 100)]
    [ShowIf("userClickedRemove", true)]
    [PropertyOrder(12)]
    public void No()
    {
        userClickedRemove = false;
    }
    //--- ---


    //--- Für Odin Inspector ---
    [SerializeField, HideInInspector] private bool userClickedRemove = false;
    private bool UserNotClickedRemove()
    {
        return !userClickedRemove;
    }
    private bool HasNoID()
    {
        return (id == Guid.Empty); 
    }
    //--- ---

    //--- Initialize & Terminate ---
    #region Initialize & Terminate
    public void InitSaveable()
    {
        SAL001_SaveLoadManager_W.instance.AddSaveableManager(this);
    }

    public void Terminate()
    {
        //ID001_IDManager.Instance.UnregisterSaveableManagerID(this);
        SAL001_SaveLoadManager_W.instance.RemoveSaveableManager(this);
    }
    #endregion
    //--- ---

    public void OnInstantiatedPrefab(GameObject _go)
    {
        if(_go.GetComponent<SAL002_SaveableObject_W>() != null)
        {
            SAL002_SaveableObject_W _saveableObject = _go.GetComponent<SAL002_SaveableObject_W>();
            saveableObjects.Add(_saveableObject.ID, _saveableObject);
        }
    }

    private void OnValidate()
    {
        prefabManager = GetComponent<PMAN001_PrefabManager_W>();
        if(ID == Guid.Empty) ID = Guid.NewGuid();
    }

    private void OnDestroy()
    {
        //if(!Application.isPlaying) ID001_IDManager.Instance.UnregisterSaveableManagerID(this);
    }
    


    //--- Save & Load ---
    #region Save & Load

    public SaveableData SaveData()
    {
        SAL004_SaveableManagerData _data = new SAL004_SaveableManagerData();

        _data.ID = ID.ToString();

        foreach(KeyValuePair<Guid, SAL002_SaveableObject_W> _kvp in saveableObjects)
        {
            _data.objectIDs.Add(_kvp.Key.ToString());
        }

        return _data;
    }

    public void LoadData()
    {
        saveableObjects.Clear();
        SAL004_SaveableManagerData _data = new SAL004_SaveableManagerData();

        if(SAL001_SaveLoadManager_W.instance.loadedData.ContainsKey(ID.ToString()))
        {
            _data = SAL001_SaveLoadManager_W.instance.loadedData[ID.ToString()] as SAL004_SaveableManagerData;

            //Debug.Log("loaded _data.ID = " + _data.ID);
        }

        
        foreach(string _id in _data.objectIDs)
        {
            GameObject _prefab = prefabManager.InstantiatePrefab();
            SAL002_SaveableObject_W _saveableObject = _prefab.GetComponent<SAL002_SaveableObject_W>();
            _saveableObject.ID = Guid.Parse(_id);
        }
    }

    #endregion
    //--- ---
}

[Serializable]
public class SAL004_SaveableManagerData : SaveableData //each Manager writes its data on Save into the SaveGameData
{
    //The Manager keeps track of objects that are instantiated at runtime and re-instantiates them when loading the game.
    //Managers are loaded first, instantiate their child objects. Then the objects load their data and pass the
    //component data to their saveableComponents.
    public List<string> objectIDs = new List<string>();

}
}