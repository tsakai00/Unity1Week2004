using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lib.Physics
{
    /// <summary>
    /// 自前の距離バネ
    /// </summary>
    public class Spring2D
    {
        private MassPoint2D _a;
        private MassPoint2D _b;
        private float _width;
        private float _sqrWidth;
        private float _softness;

        public Spring2D(MassPoint2D a, MassPoint2D b, float width, float softness = 0.5f)
        {
            _a = a;
            _b = b;
            _width = width;
            _sqrWidth = width * width;
            _softness = softness;
        }

        public void Stretch()
        {
            // Vector2 v = (_b.position + _b.velocity) - (_a.position + _a.velocity);
            // float d = v.sqrMagnitude;
            // float w = (d + _sqrWidth) * (_a.invMass + _b.invMass);
            // if(w > 0.0f)
            // {
            //     v *= (d - _sqrWidth) / w * _softness;
            //     _a.velocity += v * _a.invMass;
            //     _b.velocity -= v * _b.invMass;
            // }

            if(_a.isKinematic && _b.isKinematic) { return; }

            Vector2 v = (_b.position + _b.velocity) - (_a.position + _a.velocity);
            float d = v.sqrMagnitude;
            float w = (d + _sqrWidth) * (_a.invMass + _b.invMass);
            if(w > 0.0f)
            {
                v *= (d - _sqrWidth) / w * _softness;
                if(_a.isKinematic)
                {
                    _b.velocity -= v * (_a.invMass + _b.invMass);
                }
                else if(_b.isKinematic)
                {
                    _a.velocity += v * (_a.invMass + _b.invMass);
                }
                else
                {
                    _a.velocity += v * _a.invMass;
                    _b.velocity -= v * _b.invMass;
                }
            }

            // Vector2 v = (_b.position + _b.velocity) - (_a.position + _a.velocity);
            // float d = v.magnitude;
            // if(d > 0.0f)
            // {
            //     v = v * (d - _width) / (d * (_b.invMass + _a.invMass)) * _softness;
            //     _a.velocity += v * _a.invMass;
            //     _b.velocity -= v * _b.invMass;
            // }
        }
    }
}