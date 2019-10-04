using UnityEngine;
using UnityEngine.UI;

namespace Wichtel.Extensions
{
public static class ScrollRectExtensions
{
    private static Vector3[] viewportWorldCorners = new Vector3[4];
    private static Vector3[] contentWorldCorners  = new Vector3[4];
    
    
    //--- ScrollRect position relative to the edges of the viewport ---
    //left
    public static float PixelPositionFromLeft(this ScrollRect sr)
    {
        GetWorldCorners(sr);
        return viewportWorldCorners[0].x - contentWorldCorners[0].x;
    }

    //top
    public static float PixelPositionFromTop(this ScrollRect sr)
    {
        GetWorldCorners(sr);
        return -(viewportWorldCorners[1].y - contentWorldCorners[1].y);
    }

    //right
    public static float PixelPositionFromRight(this ScrollRect sr)
    {
        GetWorldCorners(sr);
        return -(viewportWorldCorners[2].x - contentWorldCorners[2].x);
    }
    
    //bottom
    public static float PixelPositionFromBottom(this ScrollRect sr)
    {
        GetWorldCorners(sr);
        return viewportWorldCorners[3].y - contentWorldCorners[3].y;
    }

    private static void GetWorldCorners(ScrollRect sr)
    {
        sr.viewport.GetWorldCorners(viewportWorldCorners);
        sr.content.GetWorldCorners(contentWorldCorners);
    }
    //--- ---
}
}