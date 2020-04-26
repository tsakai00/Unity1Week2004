using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] private Transform _target;

    private float   _targetX;
    private Vector2 _shakeVel = Vector2.zero;
    private Vector2 _shakeOfs = Vector2.zero;
    private Vector3 _position;

    public void SetTarget(Transform trans)
    {
        _target = trans;
    }

    // Start is called before the first frame update
    void Start()
    {
        _targetX = _target.position.x;
        _position = transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        var pos = _position;
        UpdateTarget();
        UpdateShake();

        pos.x = Mathf.Max(_targetX, 0.0f);
        _position = pos;

        pos.x += _shakeOfs.x;
        pos.y += _shakeOfs.y;
        transform.position = pos;
    }

    private void UpdateTarget()
    {
        float tgt = _target.position.x + 1.0f;
        _targetX += (tgt - _targetX) * 0.1f;
    }

    private void UpdateShake()
    {
        _shakeVel *= 0.8f;
        _shakeVel += (Vector2.zero - _shakeOfs) * 0.2f;
        _shakeOfs += _shakeVel;
    }

    /// <summary>
    /// ダメージ受けたときのシェイク
    /// </summary>
    public void Shake(Vector2 dir, float power = 0.5f)
    {
        _shakeVel = -dir.normalized * power;
    }

}
