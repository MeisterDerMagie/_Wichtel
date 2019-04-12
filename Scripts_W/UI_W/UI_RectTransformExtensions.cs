using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wichtel.UI
{
public static class UI_RectTransformExtensions
{
    //-------------- Overlaps ------------------
    public static bool Overlaps(this RectTransform rectTrans, RectTransform other)
    {
        return rectTrans.WorldRect().Overlaps(other.WorldRect());
    }
    public static bool Overlaps(this RectTransform _rect1, RectTransform other, bool allowInverse)
    {
        return _rect1.WorldRect().Overlaps(other.WorldRect(), allowInverse);
    }

    //Create Worldspace rect from rectransform
    public static Rect WorldRect(this RectTransform _rectTransform)
    {
        Vector2 sizeDelta = _rectTransform.sizeDelta;
        float rectTransformWidth = sizeDelta.x * _rectTransform.lossyScale.x;
        float rectTransformHeight = sizeDelta.y * _rectTransform.lossyScale.y;

        Vector3 position = _rectTransform.position;
        return new Rect(position.x - rectTransformWidth / 2f, position.y - rectTransformHeight / 2f, rectTransformWidth, rectTransformHeight);
    }

    public static bool Contains(this RectTransform rectTrans, RectTransform other)
    {
        Vector3[] corners = new Vector3[4];
        other.GetWorldCorners(corners);

        Rect cRect = rectTrans.WorldRect();
        return  cRect.Contains(corners[0])
                &&cRect.Contains(corners[1])
                &&cRect.Contains(corners[2])
                &&cRect.Contains(corners[3]);
    }

    public static void SetLeft(this RectTransform rt, float left)
    {
        rt.offsetMin = new Vector2(left, rt.offsetMin.y);
    }

    public static void SetRight(this RectTransform rt, float right)
    {
        rt.offsetMax = new Vector2(-right, rt.offsetMax.y);
    }

    public static void SetTop(this RectTransform rt, float top)
    {
        rt.offsetMax = new Vector2(rt.offsetMax.x, -top);
    }

    public static void SetBottom(this RectTransform rt, float bottom)
    {
        rt.offsetMin = new Vector2(rt.offsetMin.x, bottom);
    }


    public static void SetAnchor(this RectTransform rt, AnchorPresets align, int offsetX=0, int offsetY=0)
    {
        rt.anchoredPosition = new Vector3(offsetX, offsetY, 0);

        switch (align)
        {
            case(AnchorPresets.TopLeft):
            {
                rt.anchorMin = new Vector2(0, 1);
                rt.anchorMax = new Vector2(0, 1);
                break;
            }
            case (AnchorPresets.TopCenter):
            {
                rt.anchorMin = new Vector2(0.5f, 1);
                rt.anchorMax = new Vector2(0.5f, 1);
                break;
            }
            case (AnchorPresets.TopRight):
            {
                rt.anchorMin = new Vector2(1, 1);
                rt.anchorMax = new Vector2(1, 1);
                break;
            }

            case (AnchorPresets.MiddleLeft):
            {
                rt.anchorMin = new Vector2(0, 0.5f);
                rt.anchorMax = new Vector2(0, 0.5f);
                break;
            }
            case (AnchorPresets.MiddleCenter):
            {
                rt.anchorMin = new Vector2(0.5f, 0.5f);
                rt.anchorMax = new Vector2(0.5f, 0.5f);
                break;
            }
            case (AnchorPresets.MiddleRight):
            {
                rt.anchorMin = new Vector2(1, 0.5f);
                rt.anchorMax = new Vector2(1, 0.5f);
                break;
            }

            case (AnchorPresets.BottomLeft):
            {
                rt.anchorMin = new Vector2(0, 0);
                rt.anchorMax = new Vector2(0, 0);
                break;
            }
            case (AnchorPresets.BottonCenter):
            {
                rt.anchorMin = new Vector2(0.5f, 0);
                rt.anchorMax = new Vector2(0.5f,0);
                break;
            }
            case (AnchorPresets.BottomRight):
            {
                rt.anchorMin = new Vector2(1, 0);
                rt.anchorMax = new Vector2(1, 0);
                break;
            }

            case (AnchorPresets.HorStretchTop):
            {
                rt.anchorMin = new Vector2(0, 1);
                rt.anchorMax = new Vector2(1, 1);
                break;
            }
            case (AnchorPresets.HorStretchMiddle):
            {
                rt.anchorMin = new Vector2(0, 0.5f);
                rt.anchorMax = new Vector2(1, 0.5f);
                break;
            }
            case (AnchorPresets.HorStretchBottom):
            {
                rt.anchorMin = new Vector2(0, 0);
                rt.anchorMax = new Vector2(1, 0);
                break;
            }

            case (AnchorPresets.VertStretchLeft):
            {
                rt.anchorMin = new Vector2(0, 0);
                rt.anchorMax = new Vector2(0, 1);
                break;
            }
            case (AnchorPresets.VertStretchCenter):
            {
                rt.anchorMin = new Vector2(0.5f, 0);
                rt.anchorMax = new Vector2(0.5f, 1);
                break;
            }
            case (AnchorPresets.VertStretchRight):
            {
                rt.anchorMin = new Vector2(1, 0);
                rt.anchorMax = new Vector2(1, 1);
                break;
            }

            case (AnchorPresets.StretchAll):
            {
                rt.anchorMin = new Vector2(0, 0);
                rt.anchorMax = new Vector2(1, 1);
                break;
            }
        }
    }

    public static void SetPivot(this RectTransform source, PivotPresets preset)
    {

        switch (preset)
        {
            case (PivotPresets.TopLeft):
            {
                source.pivot = new Vector2(0, 1);
                break;
            }
            case (PivotPresets.TopCenter):
            {
                source.pivot = new Vector2(0.5f, 1);
                break;
            }
            case (PivotPresets.TopRight):
            {
                source.pivot = new Vector2(1, 1);
                break;
            }

            case (PivotPresets.MiddleLeft):
            {
                source.pivot = new Vector2(0, 0.5f);
                break;
            }
            case (PivotPresets.MiddleCenter):
            {
                source.pivot = new Vector2(0.5f, 0.5f);
                break;
            }
            case (PivotPresets.MiddleRight):
            {
                source.pivot = new Vector2(1, 0.5f);
                break;
            }

            case (PivotPresets.BottomLeft):
            {
                source.pivot = new Vector2(0, 0);
                break;
            }
            case (PivotPresets.BottomCenter):
            {
                source.pivot = new Vector2(0.5f, 0);
                break;
            }
            case (PivotPresets.BottomRight):
            {
                source.pivot = new Vector2(1, 0);
                break;
            }
        }
    }
}

public enum AnchorPresets
{
    TopLeft,
    TopCenter,
    TopRight,

    MiddleLeft,
    MiddleCenter,
    MiddleRight,

    BottomLeft,
    BottonCenter,
    BottomRight,
    BottomStretch,

    VertStretchLeft,
    VertStretchRight,
    VertStretchCenter,

    HorStretchTop,
    HorStretchMiddle,
    HorStretchBottom,

    StretchAll
}
public enum PivotPresets
{
    TopLeft,
    TopCenter,
    TopRight,

    MiddleLeft,
    MiddleCenter,
    MiddleRight,

    BottomLeft,
    BottomCenter,
    BottomRight,
}
}