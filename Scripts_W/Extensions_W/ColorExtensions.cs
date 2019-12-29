using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wichtel.Extensions{
public static class ColorExtensions
{
    //use this to change only one, two or three values of a Color
    public static Color With(this Color original, float? r = null, float? g = null, float? b = null, float? a = null)
    {
        float newR = r ?? original.r;
        float newG = g ?? original.g;
        float newB = b ?? original.b;
        float newA = a ?? original.a;
        
        return new Color(newR, newG, newB, newA);
    }
}
}