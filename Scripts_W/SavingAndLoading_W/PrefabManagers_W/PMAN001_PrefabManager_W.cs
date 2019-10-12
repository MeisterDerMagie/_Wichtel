using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wichtel{
public class PMAN001_PrefabManager_W : MonoBehaviour
{
    public GameObject prefab;
    public virtual GameObject InstantiatePrefab()
    {
        GameObject _go = Instantiate(prefab, transform);
        
        if(_go.GetComponent<SAL002_SaveableObject_W>() != null && GetComponent<SAL004_SaveableManager_W>() != null)
        {
            _go.GetComponent<SAL002_SaveableObject_W>().InitSaveable();
            GetComponent<SAL004_SaveableManager_W>().OnInstantiatedPrefab(_go);
        }

        return _go;
    }

    public void CallInstantiateFromUI()
    {
        InstantiatePrefab();
    }
}
}