using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wichtel.UI{
    [ExecuteInEditMode]
    [RequireComponent(typeof(RectTransform))]
public class UI_RectTransCopySize : MonoBehaviour
{
    public RectTransform rectTransToGet;
    public bool setPositionToo;
    private RectTransform ownRectTranform;

    private void OnEnable()
    {
        ownRectTranform = GetComponent<RectTransform>();
    }

    private void OnValidate()
    {
        ownRectTranform = GetComponent<RectTransform>();
    }

    #if UNITY_EDITOR
    private void Update()
    {
        if(!Application.isPlaying) GetSize();
    }
    #endif

    public void GetSize()
    {
        if(ownRectTranform != null && rectTransToGet != null)
        {
            Rect _otherRect = rectTransToGet.rect;
            Rect _parentRect = transform.parent.GetComponent<RectTransform>().rect;
            
            
            //set position if wanted
            if(setPositionToo) ownRectTranform.position = rectTransToGet.position;

            //set size
            ownRectTranform.sizeDelta = new Vector2(

                _otherRect.width  - (_parentRect.width  * (ownRectTranform.anchorMax.x - ownRectTranform.anchorMin.x)) ,
                _otherRect.height - (_parentRect.height * (ownRectTranform.anchorMax.y - ownRectTranform.anchorMin.y))

            );
        }
        else
        {
            Debug.LogWarning("Set rectTransToGet!", this);
        }
    }
}
}