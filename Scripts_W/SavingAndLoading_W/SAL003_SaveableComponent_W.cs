using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Wichtel{
    [ExecuteInEditMode]
    [HideMonoScript]
public class SAL003_SaveableComponent_W : SerializedMonoBehaviour, IStoreable
{
     //--- DATA ---
    [SerializeField, ReadOnly] private Guid id;
    public Guid ID { get => id; set => id = value; }
    //--- ---
    
    //--- References ---
    [SerializeField, ReadOnly] private SAL002_SaveableObject_W associatedSaveableObject;
    //--- ---

    //--- ID generieren ---
    private void GenerateUniqueID()
    {
        if (ID == Guid.Empty) ID = Guid.NewGuid();
    }
    
    //--- Init & Terminate ---
    #region Initialize & Terminate
    public void InitStoreable()
    {
        GenerateUniqueID();
        GetAssociatedSaveableObject();
        RegisterComponent();
    }

    public void Terminate()
    {
        UnregisterComponent();
    }
    #endregion
    //--- ---
    
    //--- Register & Unregister this component ---
    private void RegisterComponent()
    {
        //Register this component at the associated SaveableObject
        if (associatedSaveableObject != null)
            associatedSaveableObject.RegisterSaveableComponent(this);
    }

    private void UnregisterComponent()
    {
        //Unregister this component at the associated SaveableObject
        if (associatedSaveableObject != null)
            associatedSaveableObject.UnregisterSaveableComponent(this);
    }

    private void GetAssociatedSaveableObject()
    {
        if (associatedSaveableObject != null) return; // do nothing, if the reference is already set
        var _ascSaveableObjects = GetComponentsInParent<SAL002_SaveableObject_W>(true);
        if (_ascSaveableObjects.Length > 0) associatedSaveableObject = _ascSaveableObjects[0];
        else Debug.LogWarning("Watch out! There should be a SaveableObject associated with every SaveableComponent!", this);
    }
    //--- ---
    
    private void OnValidate()
    {
        InitStoreable();
    }

    private void OnDestroy()
    {
        if(!Application.isPlaying)
            Terminate();
    }

    //--- Store & Restore ---
    #region Store & Restore

    public virtual SAL003_SaveableComponentData StoreData()
    {
        return null;
    }

    public virtual void RestoreData(SAL003_SaveableComponentData _data)
    {

    }

    #endregion
    //--- ---
}

[Serializable]
public abstract class SAL003_SaveableComponentData : SaveableData
{
    
}
}