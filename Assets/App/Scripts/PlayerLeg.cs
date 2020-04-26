using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Lib.Physics;
using UnityEngine;

public class PlayerLeg : MonoBehaviour
{
    public const int     JOINT_NUM       = 16;
    public const float   JOINT_WIDTH     = 0.01f;
    public const float   JOINT_SOFTNESS  = 0.2f;

    [SerializeField] private PlayerLegJoint _jointPrefab;

    private List<PlayerLegJoint>        _legJointList;

    public void SetPosition(Vector2 beginPos, Vector2 endPos)
    {
        int num = _legJointList.Count;
        for(int i = 0; i < num; i++)
        {
            _legJointList[i].body.position = Vector2.Lerp(beginPos, endPos, (float)i / (num - 1));
        }
    }

    public PlayerLegJoint first { get { return _legJointList.First(); } }
    public PlayerLegJoint last  { get { return _legJointList.Last(); } }
    public List<PlayerLegJoint> jointList { get { return _legJointList; } }

    public void Create(Spring2DManager springManager)
    {
        List<PlayerLegJoint> list = new List<PlayerLegJoint>(JOINT_NUM);
        for(int i = 0; i < JOINT_NUM; i++)
        {
            list.Add(Instantiate(_jointPrefab, transform));
        }

        Color ca, cb;
        ColorUtility.TryParseHtmlString("#de7518", out ca);
        ColorUtility.TryParseHtmlString("#fab443", out cb);
        for(int i = 0; i < JOINT_NUM - 1; i++)
        {
            float t = (float)(i) / (JOINT_NUM - 2);
            Color c = Color.Lerp(ca, cb, (float)(i) / JOINT_NUM);
            list[i].Init(list[i + 1].body, c);
        }

        list[JOINT_NUM - 1].SetEdge();
        for(int i = 0; i < JOINT_NUM - 1; i++)
        {
            var a = list[i];
            var b = list[i + 1];
            springManager.Add(a.body, b.body, JOINT_WIDTH, JOINT_SOFTNESS);
        }

        _legJointList = list;
    }
}
