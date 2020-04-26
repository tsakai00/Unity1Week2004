using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lib.Physics
{
    /// <summary>
    /// RigidBody2Dを一時的に自前計算用の質点に変換
    /// </summary>
    // public class MassPoint2D
    // {
    //     public Rigidbody2D  body        { get; private set; }
    //     public Vector2      position    { get; set; }
    //     public float        mass        { get; private set; }
    //     public float        invMass     { get; private set; }
    //     public Vector2      velocity    { get { return position - _prevPosition; } }

    //     private Vector2 _prevPosition;

    //     public MassPoint2D(Rigidbody2D body)
    //     {
    //         Init(body);
    //     }

    //     public void Init(Rigidbody2D body)
    //     {
    //         this.body = body;
    //         mass = body.isKinematic ? 1000000.0f : body.mass;
    //         invMass = 1.0f / mass;
    //     }

    //     public void PreProcess(float dt)
    //     {
    //         var vel = body.velocity * dt;
    //         _prevPosition = body.position;
    //         position = (body.position + vel);
    //     }

    //     public void PostProcess(float invDt)
    //     {
    //         // invDt = 1.0f / dt;
    //         body.velocity = velocity * mass * invDt;
    //         body.position = position;
    //     }
    // }

    public class MassPoint2D : MonoBehaviour
    {
        [SerializeField] private float  _mass           = 1.0f;
        [SerializeField] private bool   _isKinematic    = false;
        [SerializeField] private float  _dec            = 0.95f;
        [SerializeField] private float  _gravityScale   = 1.0f;

        public Vector2  position;
        public Vector2  prevPosition    { get; private set; }
        public float    mass            { get { return _mass; } set { _mass = Mathf.Max(value, 0.000001f); invMass = 1.0f / mass; } }
        public float    invMass         { get; private set; }
        public Vector2  velocity;
        public bool     isKinematic     { get { return _isKinematic; } set { _isKinematic = value; } }

        // public MassPoint2D(Vector2 position, float mass)
        // {
        //     Init(position, mass);
        // }

        void Awake()
        {
            Init(transform.position);
        }

        public void Init(Vector2 position)
        {
            this.position = position;
            this.prevPosition = position;
            this.velocity = Vector2.zero;
            this.invMass = 1.0f / mass;
        }

        public void PreProcess(float dt)
        {
            
        }

        public void PostProcess(float invDt)
        {
            prevPosition = position;
            if(isKinematic == false)
            {
                velocity *= _dec;
                velocity.y -= 0.001f * _gravityScale;

                position += velocity;
            }

            transform.position = position;
        }
    }
}
