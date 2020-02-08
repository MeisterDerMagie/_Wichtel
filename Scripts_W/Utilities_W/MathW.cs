//copyright by Martin M. Klöckener
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wichtel
{
public static class MathW
{
    //remap for float
    public static float Remap(float value, float low1, float high1, float low2, float high2)
    {
        return low2 + (value - low1) * (high2 - low2) / (high1 - low1);
    }
    
    /// <summary>
    /// returns f rooted by n
    /// </summary>
    /// <param name="f">Zahl, von der die Wurzel gezogen werden soll</param>
    /// <param name="n">n-te Wurzel</param>
    /// <returns></returns>
    public static float NthRoot(float f, float n)
    {
        return Mathf.Pow(f, 1.0f / n);
    }
}
}