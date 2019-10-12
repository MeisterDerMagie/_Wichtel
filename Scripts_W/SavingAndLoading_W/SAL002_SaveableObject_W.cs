using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Wichtel{
    [ExecuteInEditMode]
    [HideMonoScript]
public class SAL002_SaveableObject_W : SerializedMonoBehaviour, ISaveable
{
    //--- DATA ---
    #region DATA

    [SerializeField, ReadOnly] private Guid id;
    public Guid ID { get{return id;} set{id = value;} }
    
    #endregion
    //--- ---

    //--- References ---
    #region References
    [ReadOnly]
    public Dictionary<Guid, SAL003_SaveableComponent_W> saveableComponents = new Dictionary<Guid, SAL003_SaveableComponent_W>();
    #endregion
    //--- ---

    private void OnDestroy()
    {
        Terminate();
    }

    //--- ID generieren ---
    private void OnValidate()
    {
        GenerateUniqueID();
    }

    private void GenerateUniqueID()
    {
        if(ID == Guid.Empty) ID = Guid.NewGuid();
    }
    //--- ---

    //--- Initialize & Terminate ---
    #region Initialize & Terminate
    public void InitSaveable()
    {
        SAL001_SaveLoadManager_W.instance.AddSaveableObject(this);
    }

    public void Terminate()
    {
        if(Application.isPlaying) SAL001_SaveLoadManager_W.instance.RemoveSaveableObject(this);
    }
    #endregion
    //--- ---

    public void RegisterSaveableComponent(SAL003_SaveableComponent_W _ascComponent)
    {
        if(!saveableComponents.ContainsKey(_ascComponent.ID))
        {
            //_ascComponent.ID = ID001_IDManager.Instance.GetNewSaveableComponentID();
            saveableComponents.Add(_ascComponent.ID, _ascComponent);
            Debug.Log("Registered saveable component: " + _ascComponent.ID.ToString());
        }
    }

    public void UnregisterSaveableComponent(SAL003_SaveableComponent_W _ascComponent)
    {
        if(saveableComponents.ContainsKey(_ascComponent.ID))
        {
            saveableComponents.Remove(_ascComponent.ID);
        }
    }


    //--- Save & Load ---
    #region Save & Load

    public SaveableData SaveData()
    {
        var _data = new SAL002_SaveableObjectData();

        _data.ID = ID.ToString();
        
        //Store data of child components
        foreach(var _sc in saveableComponents)
        {
            _data.componentDatas.Add(_sc.Key.ToString(), _sc.Value.StoreData());
        }

        //Save Data
        return _data;
    }

    public void LoadData()
    {
        if (!SAL001_SaveLoadManager_W.instance.loadedData.ContainsKey(ID.ToString())) return;
        
        var _data = new SAL002_SaveableObjectData();
        _data = SAL001_SaveLoadManager_W.instance.loadedData[ID.ToString()] as SAL002_SaveableObjectData;

        foreach(var kvp in _data.componentDatas)
        {
            if(saveableComponents.ContainsKey(Guid.Parse(kvp.Key)))
            {
                saveableComponents[Guid.Parse(kvp.Key)].RestoreData(kvp.Value);
            }
        }
    }

    #endregion
    //--- ---
}

[Serializable]
public class SAL002_SaveableObjectData : SaveableData //each Object writes its data on Save into the SaveGameData
{
    //the object knows its saveableComponents. Stored in a Dictionary with <ID, data>
    public Dictionary<string, SAL003_SaveableComponentData> componentDatas = new Dictionary<string, SAL003_SaveableComponentData>();
}
}