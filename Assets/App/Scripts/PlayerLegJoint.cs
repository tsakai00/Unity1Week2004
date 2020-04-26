using System.Collections;
using System.Collections.Generic;
using Lib.Physics;
using UnityEngine;

/// <summary>
/// プレイヤーの足の節
/// </summary>
public class PlayerLegJoint : MonoBehaviour
{
    [SerializeField] private MassPoint2D    _body;
    [SerializeField] private Transform      _viewRoot;
    [SerializeField] private SpriteRenderer _viewEdge;

    public MassPoint2D body { get { return _body; } }

    private MassPoint2D _target = null;

    private const float TOUCH_SCALE = 0.5f;
    private const float BASE_SCALE = 0.3f;
    private float _scaleTgt = 0.3f;
    private float _scaleVel = 0.0f;
    private MoveWall    _hitMoveWall;
    private bool        _isCheckPoint = false;  // 何故かこのクラスをチェックポイントも使っている。その識別用

    private Color _edgeColorA = Color.white;
    private Color _edgeColorB = Color.white;

    public void Init(MassPoint2D target, Color color, bool isCheckPoint = false)
    {
        _target = target;
        _isCheckPoint = isCheckPoint;

        var sprList = gameObject.GetComponentsInChildren<SpriteRenderer>();
        foreach(var spr in sprList)
        {
            spr.color = color;
        }
    }

    /// <summary>
    /// 先端にする
    /// </summary>
    public void SetEdge()
    {
        body.isKinematic = true;

        Vector3 pos = _viewEdge.transform.position;
        pos.z = -1.0f;   // 手前に持ってくる
        _viewEdge.transform.position = pos;
        _viewEdge.transform.localScale = new Vector3(BASE_SCALE, BASE_SCALE, 1.0f);

        ColorUtility.TryParseHtmlString("#fab443", out _edgeColorA);
        ColorUtility.TryParseHtmlString("#fab443", out _edgeColorB);
        _viewEdge.color = _edgeColorA;

        _viewRoot.gameObject.SetActive(false);
    }

    /// <summary>
    /// 壁にタッチしたときのアニメ
    /// </summary>
    public void SetTouch(Vector2 normal)
    {
        _scaleTgt = TOUCH_SCALE;

        float r = Mathf.Atan2(normal.y, normal.x) * Mathf.Rad2Deg;
        _viewRoot.transform.rotation = Quaternion.Euler(0.0f, 0.0f, r);
    }

    public void SetUntouch()
    {
        _scaleTgt = BASE_SCALE;
        RemoveMoveWall();
    }

    public void SetNextUntouch()
    {
        _scaleTgt = BASE_SCALE;
    }

    void FixedUpdate()
    {
        if(_isCheckPoint) { return; }
        if(_target != null) { return; }

        float scl = _viewEdge.transform.localScale.x;
        _scaleVel *= 0.8f;
        _scaleVel += (_scaleTgt - scl) * 0.2f;
        scl += _scaleVel;
        _viewEdge.transform.localScale = new Vector3(scl, scl, 1.0f);

        float t = (_scaleTgt - BASE_SCALE) / (TOUCH_SCALE - BASE_SCALE);
        _viewEdge.color = Color.Lerp(_edgeColorA, _edgeColorB, t);
    }

    void LateUpdate()
    {
        if(_target == null) { return; }

        var v = _target.transform.position - _body.transform.position;
        var r = Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg;
        _viewRoot.transform.rotation = Quaternion.Euler(0.0f, 0.0f, r);
        
        var scl = _viewRoot.transform.localScale;
        scl.x = v.magnitude;
        _viewRoot.transform.localScale = scl;
    }

    public void SetMoveWall(MoveWall move)
    {
        _hitMoveWall = move;
        _hitMoveWall.AddJoint(this);
    }
    public void RemoveMoveWall()
    {
        if(_hitMoveWall != null)
        {
            _hitMoveWall.RemoveJoint(this);
        }

        _hitMoveWall = null;
    }
}
