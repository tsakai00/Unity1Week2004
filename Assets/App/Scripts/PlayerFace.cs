using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFace : MonoBehaviour
{
    [SerializeField] private Transform  _eye;
    [SerializeField] private Transform  _mouth;

    public void LookEye(Vector2 v)
    {
        float w = v.magnitude;
        if(w == 0.0f) { return; }
        v = v / w * Mathf.Min(0.15f, w);

        {
            Vector2 pos = _eye.localPosition;
            _eye.localPosition = pos + (v - pos) * 0.1f;
        }
        {
            v.x *= 1.5f;
            Vector2 pos = _mouth.localPosition;
            _mouth.localPosition = pos + (v - pos) * 0.1f;
        }
    }
}
