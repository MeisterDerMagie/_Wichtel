//(c) copyright by Martin M. Klöckener
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Haferbrei {
[RequireComponent(typeof(RectTransform))]
[ExecuteAlways]
public class UltimateLayoutElement : UIBehaviour, ILayoutElement, ILayoutIgnorer
{
    public UltimateLayoutGroup.Sizing sizingX;
    public UltimateLayoutGroup.Sizing sizingY;
    
    public void CalculateLayoutInputHorizontal()
    {
        throw new System.NotImplementedException();
    }

    public void CalculateLayoutInputVertical()
    {
        throw new System.NotImplementedException();
    }

    public float minWidth { get; }
    public float preferredWidth { get; }
    public float flexibleWidth { get; }
    public float minHeight { get; }
    public float preferredHeight { get; }
    public float flexibleHeight { get; }
    public int layoutPriority { get; }
    public bool ignoreLayout { get; }
}
}