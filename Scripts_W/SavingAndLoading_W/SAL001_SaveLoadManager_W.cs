using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BayatGames.SaveGamePro;
using Sirenix.OdinInspector;

namespace Wichtel{
public class SAL001_SaveLoadManager_W : SerializedMonoBehaviour
{
    public Dictionary<string, SaveableData> loadedData = new Dictionary<string, SaveableData>();

    public List<ISaveable> saveableManagers = new List<ISaveable>();
    public List<ISaveable> saveableObjects  = new List<ISaveable>();

    public Dictionary<string, SaveableData> dictionaryToSave = new Dictionary<string, SaveableData>();

    //--- Singleton Behaviour ---
    #region Singleton

    private static SAL001_SaveLoadManager_W instance_;

    public static SAL001_SaveLoadManager_W instance
        => instance_ == null ? FindObjectOfType<SAL001_SaveLoadManager_W>() : instance_;

    public void Awake() //The IDManager is supposed to be initialized in Awake, not in InitSingleton
    {
        if (instance_ == null)
            instance_ = this;
        else
            Destroy(gameObject);
        
        DontDestroyOnLoad(gameObject);
    }

    #endregion
    //--- ---
    
    //--- Singleton Behaviour ---
    /*#region Singleton

    private static SAL001_SaveLoadManager _instance;
    public static SAL001_SaveLoadManager instance
    {
        get
        {
            if(_instance == null)
                return GameObject.FindObjectOfType<SAL001_SaveLoadManager>();
            else
                return _instance;
        }
        set { _instance = value; }
    }
    private void Awake()
    {
        if (instance == null) instance = this;
            else 
            {
                Destroy(gameObject);
                return;
            }
            DontDestroyOnLoad(gameObject);
    }

    #endregion*/
    //--- ---

    //--- Add & Remove ISaveables ---
    #region Add & Remove ISaveables
    public void AddSaveableManager(ISaveable _saveableData)
    {
        if(!saveableManagers.Contains(_saveableData))
            saveableManagers.Add(_saveableData);

        //Debug.Log("Add Manager ISaveable: " + _saveableData.ID);
    }

    public void RemoveSaveableManager(ISaveable _saveableData)
    {
        if(saveableManagers.Contains(_saveableData))
            saveableManagers.Remove(_saveableData);

        //Debug.Log("Remove Manager ISaveable: " + _saveableData.ID);
    }

    public void AddSaveableObject(ISaveable _saveableData)
    {
        if(!saveableObjects.Contains(_saveableData))
            saveableObjects.Add(_saveableData);

        //Debug.Log("Add Object ISaveable: " + _saveableData.ID);
    }

    public void RemoveSaveableObject(ISaveable _saveableData)
    {
        if(saveableObjects.Contains(_saveableData))
            saveableObjects.Remove(_saveableData);

        //Debug.Log("Remove Object ISaveable: " + _saveableData.ID);
    }

    #endregion
    //--- ---

    public bool CheckForExistingSaveGameFileName(string _saveGameFileName)
    {
        return SaveGame.Exists(_saveGameFileName);
    }

    public void SaveGameState(string _saveGameFileName)
    {
        
        dictionaryToSave.Clear();

        //Save Managers
        foreach(ISaveable _saveableData in saveableManagers)
        {
            SaveableData _data = _saveableData.SaveData();
            dictionaryToSave.Add( _data.ID, _data );

            //Debug.Log("Save Manager: " + _saveableData.ID);
        }

        //Save Objects
        foreach(ISaveable _saveableData in saveableObjects)
        {
            SaveableData _data = _saveableData.SaveData();
            dictionaryToSave.Add( _data.ID, _data );

            //Debug.Log("Save Object: " + _saveableData.ID);
        }

        SaveGame.Save(_saveGameFileName, dictionaryToSave);

        Debug.Log("Game saved!");
    }

    public void LoadGameState(string _saveGameFileName)
    {
        
        loadedData = SaveGame.Load<Dictionary<string, SaveableData>>(_saveGameFileName, null);


        //Load Managers
        foreach(ISaveable _saveableData in saveableManagers)
        {
            _saveableData.LoadData();
            //Debug.Log("Load Manager: " + _saveableData.ID);
        }

        //Initialize Managers
        foreach(ISaveable _saveableData in saveableManagers)
        {
            _saveableData.InitSaveable();
            //Debug.Log("Initialize Manager: " + _saveableData.ID);
        }

        //Load Objects
        foreach(ISaveable _saveableData in saveableObjects)
        {
            _saveableData.LoadData();
            //Debug.Log("Load Object: " + _saveableData.ID);
        }

        //Initialize Objects
        foreach(ISaveable _saveableData in saveableObjects)
        {
            _saveableData.InitSaveable();
            //Debug.Log("Initialize Object: " + _saveableData.ID);
        }
        
        Debug.Log("Game loaded!");
    }
}

[Serializable]
public abstract class SaveableData //base class for all classes that can be saved
{
    public string ID; //each saveable instance has a unique ID
}
}