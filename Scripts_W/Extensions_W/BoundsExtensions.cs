using UnityEngine;

namespace Wichtel.Extensions {
public static class BoundsExtensions
{
    public static Bounds CalculateBoundsInChildren(Transform _transform)
    {
        Bounds bounds = new Bounds(_transform.position, Vector3.zero);
 
        foreach(Renderer renderer in _transform.GetComponentsInChildren<Renderer>())
        {
            bounds.Encapsulate(renderer.bounds);
        }

        return bounds;
    }
}
}