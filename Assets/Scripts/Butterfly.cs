using System;
using Lucky.Framework;
using Lucky.Utilities;
using UnityEngine;

namespace DefaultNamespace
{
    public class Butterfly : ManagedBehaviour
    {
        public Animator anim;
        private BezierCurve curve;
        private float k = 0;
        public float Speed = 1;
        public SpriteRenderer sr;

        private void Awake()
        {
            anim.speed = 0.1f;
            curve = new()
            {
                Begin = RandomUtils.RandomPointInRect(CameraUtils.Bounds),
                End = RandomUtils.RandomPointInRect(CameraUtils.Bounds),
                Control = RandomUtils.RandomPointInRect(CameraUtils.Bounds)
            };
            sr.color = ColorUtils.GetRandomLerpColor(2);
        }

        protected override void ManagedFixedUpdate()
        {
            base.ManagedFixedUpdate();
            k += Timer.FixedDeltaTime() * Speed;
            transform.position = curve.GetPoint(k);
            if (k > 1)
            {
                curve.Begin = transform.position;
                curve.End = RandomUtils.RandomPointInRect(CameraUtils.Bounds);
                curve.Control = RandomUtils.RandomPointInRect(CameraUtils.Bounds);
                k = 0;
            }

            Vector2 speedVec = curve.GetPoint(k + Timer.FixedDeltaTime() * Speed) - curve.GetPoint(k);

            float angle = MathUtils.SignedAngle(Vector2.right, speedVec);
            transform.eulerAngles = new Vector3(0, 0, angle);
            if (angle is > -90 and < 90)
            {
                sr.flipY = false;
            }
            else
            {
                sr.flipY = true;
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.black;
            Gizmos.DrawSphere(curve.Begin, 1);
            Gizmos.DrawSphere(curve.End, 1);
            Gizmos.DrawSphere(curve.Control, 1);
        }
    }
}