using System;
using Lucky.Framework;
using Lucky.Framework.Utilities;
using UnityEngine;
using Animator = Lucky.Framework.Animation.Animator;

namespace DefaultNamespace
{
    public class Butterfly : ManagedBehaviour
    {
        private Animator anim;
        private BezierCurve curve;
        private float k = 0;
        public float Speed = 1;

        private void Awake()
        {
            anim = Animator.CreateById("butterfly");
            Add(anim);
            anim.MaskInteraction = SpriteMaskInteraction.VisibleInsideMask;
            curve = new()
            {
                Begin = RandomUtils.RandomPointInRect(CameraUtils.Bounds),
                End = RandomUtils.RandomPointInRect(CameraUtils.Bounds),
                Control = RandomUtils.RandomPointInRect(CameraUtils.Bounds)
            };
            anim.Color = ColorUtils.GetRandomLerpColor(2);
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
                anim.FlipY = false;
            }
            else
            {
                anim.FlipY = true;
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