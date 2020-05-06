using UnityEngine;

namespace Wichtel.Extensions {
public static class CameraExtensions
{
    /// <summary>
    /// Returns the orthographic camera bounds
    /// </summary>
    /// <param name="_camera"></param>
    /// <returns></returns>
    public static Bounds OrthographicBounds(this Camera _camera)
    {
        if (!_camera.orthographic)
        {
            Debug.LogError($"The camera {_camera.name} is not Orthographic!", _camera);
            return new Bounds();
        }
 
        Transform camTrans = _camera.transform;
        float scaleWidthFactor = (float)Screen.currentResolution.width / (float)Screen.currentResolution.height;
        float height = _camera.orthographicSize * 2f;
        float width = height * scaleWidthFactor;
        float viewDepth = _camera.farClipPlane - _camera.nearClipPlane;
 
        return new Bounds(new Vector3(camTrans.position.x, camTrans.position.y, camTrans.position.z), new Vector3(width, height, viewDepth));
    }
 
    /// <summary>
    /// Returns the orthographic camera rect (without z)
    /// </summary>
    /// <param name="_camera"></param>
    /// <returns></returns>
    public static Rect OrthographicRect(this Camera _camera)
    {
        if (!_camera.orthographic)
        {
            Debug.LogError($"The camera {_camera.name} is not Orthographic!", _camera);
            return new Rect();
        }
 
        Transform camTrans = _camera.transform;
        float scaleWidthFactor = (float)Screen.currentResolution.width / (float)Screen.currentResolution.height;
        float height = _camera.orthographicSize * 2f;
        float width = height * scaleWidthFactor;
 
        return new Rect(new Vector2(camTrans.position.x - (width*0.5f), camTrans.position.y - (height * 0.5f)), new Vector2(width, height));
    }
}
}