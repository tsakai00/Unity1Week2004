using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Util
{
    public static Vector3 GetMousePosition(Camera camera)
    {
        var pos = Input.mousePosition;
        pos.z = -camera.transform.position.z;
        pos = camera.ScreenToWorldPoint(pos);
        return pos;
    }
}
