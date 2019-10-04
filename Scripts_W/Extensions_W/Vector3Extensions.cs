using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wichtel.Extensions{
public static class Vector3Extensions
{
    //use this to change only one or two values of a Vector3
    //Example: transform.position = otherTransform.position.With(y: 3f); --> Equals to: transform.position = new Vector3(otherTransform.position.x, 3f, otherTransform.position.z);
    public static Vector3 With(this Vector3 original, float? x = null, float? y = null, float? z = null)
    {
        float newX = x ?? original.x;
        float newY = y ?? original.y;
        float newZ = z ?? original.z;
        return new Vector3(newX, newY, newZ);
    }
}
}