using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Wichtel{
public class SAVC102_TextMeshProText_W : SAL003_SaveableComponent_W
{
    public TextMeshProUGUI ascComponent;

    public override SAL003_SaveableComponentData StoreData()
    {
        SAVC102_TextMeshProTextData _data = new SAVC102_TextMeshProTextData();

        _data.ID       = ID.ToString();
        _data.textData = ascComponent.text;

        return _data;
    }

    public override void RestoreData(SAL003_SaveableComponentData _data)
    {
        SAVC102_TextMeshProTextData _componentData = _data as SAVC102_TextMeshProTextData;

        ascComponent.text = _componentData.textData;
    }
    
}

[System.Serializable]
public class SAVC102_TextMeshProTextData : SAL003_SaveableComponentData
{
    public string textData;
}
}