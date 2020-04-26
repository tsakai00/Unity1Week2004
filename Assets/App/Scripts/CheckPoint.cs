using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Lib.Physics;
using Lib.Sound;

public class CheckPoint : MonoBehaviour
{
    private const int JOINT_NUM = 16;
    [SerializeField] private MassPoint2D    _jointPrefab;
    [SerializeField] private bool           _isGoal;

    private List<MassPoint2D>    _jointList = new List<MassPoint2D>();

    public void Create(Spring2DManager springMapanger)
    {
        for(int i = 0; i < JOINT_NUM; i++)
        {
            var inst = Instantiate(_jointPrefab, transform);
            _jointList.Add(inst);
        }

        Color col = _isGoal ? Color.yellow : Color.white;

        _jointList[0].isKinematic = true;
        _jointList[0].position = new Vector2(transform.position.x, -5.0f);
        _jointList[JOINT_NUM - 1].isKinematic = true;
        _jointList[JOINT_NUM - 1].position = new Vector2(transform.position.x, 5.0f);
        // 終端のオブジェクトを無理やり消す…
        _jointList[JOINT_NUM - 1].GetComponentInParent<PlayerLegJoint>().Init(null, col, true);
        _jointList[JOINT_NUM - 1].transform.Find("View").gameObject.SetActive(false);
        for(int i = 0; i < JOINT_NUM - 1; i++)
        {
            var aa = _jointList[i].GetComponent<PlayerLegJoint>();
            aa?.Init(_jointList[i + 1], col);
            springMapanger.Add(_jointList[i], _jointList[i + 1], 0.5f, 0.1f);
        }
    }

    private void Hit(Vector2 pos, Vector2 vel)
    {
        int idx = 0;
        float dist = float.MaxValue;
        for(int i = 1; i < JOINT_NUM - 1; i++)
        {
            float d = (_jointList[i].position - pos).sqrMagnitude;
            if(d < dist)
            {
                idx = i;
                dist = d;
            }
        }

        _jointList[idx].position += vel.normalized * 4.0f;
        SoundManager.Instance.PlaySE(SEPath._CHECK_HIT);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        var aa = col.GetComponentInParent<PlayerBody>();
        if(aa == null) { return; }

        Hit(aa.body.position, aa.body.velocity);
    }

    public bool IsGoal()
    {
        return _isGoal;
    }
}
