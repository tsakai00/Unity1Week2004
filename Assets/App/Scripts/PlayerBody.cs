using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Lib.Physics;
using Lib.Sound;
using UnityEngine;

public class PlayerBody : MonoBehaviour
{
    private static int      LEG_NUM = 4;
    private static float    RADIUS  = 1.0f;
    private static float    DAMAGE_RADIUS = 0.35f;

    [SerializeField] private PlayerLeg _legPrefab;
    [SerializeField] private Camera _cam;
    [SerializeField] private PlayerFace _face;

    public MassPoint2D body { get { return _body; } }

    private GameMainController  _mainController;
    private MassPoint2D         _body;
    private List<PlayerLeg>     _legList;
    private int                 _legIdx;
    private bool                _isInput;
    private Vector2             _mousePosition;

    private float _dmgScaleVel = 0.0f;

    public void Create(GameMainController mainController, Spring2DManager springManager, Vector2 pos)
    {
        _mainController = mainController;
        _body = gameObject.GetComponent<MassPoint2D>();
        _body.position = pos;

        _legList = new List<PlayerLeg>(LEG_NUM);
        for(int i = 0; i < LEG_NUM; i++)
        {
            var inst = Instantiate(_legPrefab);
            inst.Create(springManager);
            _legList.Add(inst);
        }

        var posList = new Vector2[] { new Vector2(1f, 1), new Vector2(-1f, 1), new Vector2(1f, -1), new Vector2(-1f, -1) };
        for(int i = 0; i < LEG_NUM; i++)
        {
            _legList[i].SetPosition(pos, pos + posList[i].normalized * RADIUS);
        }

        for(int i = 0; i < LEG_NUM; i++)
        {
            springManager.Add(_body, _legList[i].first.body, 0, 1.0f);
        }
    }

    /// <summary>
    /// リスポーン用。座標を移動
    /// </summary>
    public void Reset(Vector2 pos)
    {
        _body.position = pos;
        _legIdx = 0;
        _isInput = false;

        var posList = new Vector2[] { new Vector2(1f, 1), new Vector2(-1f, 1), new Vector2(1f, -1), new Vector2(-1f, -1) };
        for(int i = 0; i < LEG_NUM; i++)
        {
            _legList[i].SetPosition(pos, pos + posList[i].normalized * RADIUS);
            _legList[i].last.SetUntouch();
        }
    }

    public void InputUpdate(bool isClickOK)
    {
        if(isClickOK)
        {
            _isInput = _isInput || Input.GetMouseButtonDown(0);
        }
        _mousePosition = Util.GetMousePosition(_cam);
    }

    void FixedUpdate()
    {
        // 足の位置
        var leg = _legList[_legIdx].last;
        Vector2 pos = _mousePosition;
        var v = (pos - _body.position);
        float w = v.magnitude;
        if(w != 0)
        {
            v = v / w * Mathf.Min(w, RADIUS);
            Vector2 vv = leg.body.position;
            vv += ((_body.position + v) - vv) * 0.25f;
            leg.body.position = vv;
        }

        // 足を伸ばす
        if(_isInput)
        {
            var hit = Physics2D.Raycast(_body.position, v, 22.0f, LayerMask.GetMask("Map"));
            if(hit.collider != null)
            {
                // 当たった壁に向かって移動
                {
                    leg.SetTouch(hit.normal);
                    leg.body.position = hit.point;

                    _legIdx = (_legIdx + 1) % _legList.Count;
                    var next = (_legIdx + 1) % _legList.Count;

                    _legList[_legIdx].last.SetUntouch();
                    _legList[next].last.SetNextUntouch();

                SoundManager.Instance.PlaySE(SEPath._LEG_TOUCH);
                }

                // いろんな壁との衝突をここで処理しちゃう
                {
                    var aa = hit.collider.GetComponentInParent<MoveWall>();
                    if(aa != null)
                    {
                        leg.SetMoveWall(aa);
                    }
                }
            }

            _isInput = false;
        }

        // ダメージ判定
        if(_mainController.IsGoal() == false && _mainController.IsDamage() == false)
        {
            Vector2 dir = _body.velocity;
            {
                // 壁
                var hit = Physics2D.CircleCast(_body.prevPosition, DAMAGE_RADIUS, dir, dir.magnitude, LayerMask.GetMask("Map"));
                if(hit.collider != null)
                {
                    _mainController.OnPlayerDamage();
                    _dmgScaleVel = 0.5f;
                }
            }
            {
                // チェックポイント
                var hit = Physics2D.CircleCast(_body.prevPosition, DAMAGE_RADIUS, dir, dir.magnitude, LayerMask.GetMask("CheckPoint"));
                if(hit.collider != null)
                {
                    _mainController.CheckPoint(hit.collider.transform.position.x);
                    var check = hit.collider.GetComponentInParent<CheckPoint>();
                    if(check.IsGoal())
                    {
                        _mainController.Goal();
                    }
                }
            }
        }

        // ダメージアニメ
        {
            var scl = _face.transform.localScale.x;
            _dmgScaleVel *= 0.8f;
            _dmgScaleVel += (1.0f - scl) * 0.25f;
            scl += _dmgScaleVel;
            _face.transform.localScale = new Vector3(scl, scl, 1.0f);
        }
    }

    void LateUpdate()
    {
        var v = _legList[_legIdx].last.body.position - _body.position;
        _face.LookEye(v);
    }
}
