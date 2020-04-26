using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Lib.Physics
{
    /// <summary>
    /// 登録したSpring2Dを更新
    /// </summary>
    public class Spring2DManager
    {
        public static Spring2DManager inst = new Spring2DManager();



        private List<MassPoint2D>   _massPointList  = new List<MassPoint2D>();
        private List<Spring2D>      _springList     = new List<Spring2D>();

        // public void Add(Rigidbody2D a, Rigidbody2D b, float width, float softness = 0.5f)
        // {
        //     if(_massPointList.Any(x => x.body == a) == false)
        //     {
        //         _massPointList.Add(new MassPoint2D(a));
        //     }
        //     if(_massPointList.Any(x => x.body == b) == false)
        //     {
        //         _massPointList.Add(new MassPoint2D(b));
        //     }

        //     var aa = _massPointList.FirstOrDefault(x => x.body == a);
        //     var bb = _massPointList.FirstOrDefault(x => x.body == b);
        //     _springList.Add(new Spring2D(aa, bb, width, softness));
        // }

        public void Add(MassPoint2D a, MassPoint2D b, float width, float softness = 0.5f)
        {
            if(_massPointList.Any(x => x == a) == false)
            {
                _massPointList.Add(a);
            }
            if(_massPointList.Any(x => x == b) == false)
            {
                _massPointList.Add(b);
            }

            _springList.Add(new Spring2D(a, b, width, softness));
        }

        public void Clear()
        {
            _massPointList.Clear();
            _springList.Clear();
        }

        /// <summary>
        /// 更新。内部的にRigidBody2Dを使用するので、実質FixedUpdate()で呼ぶこと
        /// </summary>
        public void Stretch(float dt)
        {
            foreach(var i in _massPointList)
            {
                i.PreProcess(dt);
            }

            float invDt = 1.0f / dt;
            int num   = _springList.Count;
            for(int j = 0; j < 2; j++)
            {
                for(int i = 0; i < num; i++)        { _springList[i].Stretch(); }
                //for(int i = num - 1; i >= 0; i--)   { _springList[i].Stretch(); }
                foreach(var i in _massPointList)
                {
                    i.PostProcess(invDt);
                }
            }
        }
    }
}