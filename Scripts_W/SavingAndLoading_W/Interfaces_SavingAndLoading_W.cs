using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wichtel{
    
    public interface ISaveable
    {
        Guid ID { get; set; }
        SaveableData SaveData();
        void LoadData();
        void InitSaveable();
        void Terminate();
    }

    public interface IStoreable
    {
        SAL003_SaveableComponentData StoreData();
        void RestoreData(SAL003_SaveableComponentData _data);
        void InitStoreable();
        void Terminate();
    }

    public interface IInitSingletons
    {
        void InitSingleton();
    }

    public interface IInitSelf
    {
        void InitSelf();
    }

    public interface IInitDependencies
    {
        void InitDependencies();
    }
}