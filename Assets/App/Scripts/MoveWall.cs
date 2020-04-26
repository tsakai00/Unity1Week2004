using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 動く壁
/// </summary>
public class MoveWall : MonoBehaviour
{
    private class JointParam
    {
        public Vector2          dir;
        public PlayerLegJoint   joint;

        public JointParam(Vector2 dir, PlayerLegJoint joint)
        {
            this.dir = dir;
            this.joint = joint;
        }
    }

    [SerializeField] private float      _angularVelocity;
    [SerializeField] private Vector2    _targetPos;
    [SerializeField] private float      _moveSec = 1.0f;

    private List<JointParam>  _jointList = new List<JointParam>();
    private Vector2 _basePos;
    private float   _sec = 0.0f;

    void Awake()
    {
        _basePos = transform.position;

        var rendererList = gameObject.GetComponentsInChildren<Renderer>();
        foreach(var renderer in rendererList)
        {
            var scl = renderer.transform.lossyScale;
            var mat = new Material(renderer.material);
            mat.SetVector("UV", scl * 0.2f);
            renderer.material = mat;
        }
    }

    void FixedUpdate()
    {
        var rot = transform.rotation.eulerAngles;
        Vector2 pos = transform.position;

        // 回転
        rot.z += _angularVelocity * Time.fixedDeltaTime;

        // 移動
        _sec += Time.fixedDeltaTime;
        if(_sec > _moveSec) { _sec -= _moveSec; }
        float t = _sec / _moveSec;
        t = Mathf.Sin(t * Mathf.PI * 2) * 0.5f + 0.5f;
        pos = Vector2.Lerp(_basePos, _targetPos + _basePos, t);

        // 
        transform.rotation = Quaternion.Euler(rot);
        transform.position = pos;

        UpdateJoint();
    }

    private void UpdateJoint()
    {
        foreach(var i in _jointList)
        {
            Vector2 pos = transform.position;
            var dir = i.dir;
            dir = transform.rotation * dir;
            dir = pos + dir;

            i.joint.body.position = dir;
        }
    }

    public void AddJoint(PlayerLegJoint joint)
    {
        if(joint == null) { return; }

        Vector2 pos = transform.position;
        var dir = joint.body.position - pos;
        var q = Quaternion.Inverse(transform.rotation);
        dir = q * dir;

        _jointList.Add(new JointParam(dir, joint));
    }

    public void RemoveJoint(PlayerLegJoint joint)
    {
        for(int i = 0; i <_jointList.Count; i++)
        {
            if(_jointList[i].joint == joint)
            {
                _jointList.RemoveAt(i);
                break;
            }
        }
    }
}
