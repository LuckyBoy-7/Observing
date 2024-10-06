using System;
using Lucky.Extensions;
using Lucky.Utilities;
using UnityEngine;
using static Lucky.Utilities.MathUtils;

namespace Lucky.Framework.Particle
{
    public class Particle : ManagedBehaviour
    {
        protected virtual void Kill()
        {
            ParticleSystem.Instance.ReleaseParticle(this);
            gameObject.SetActive(false);
        }

        public SpriteRenderer sr;
        public ManagedBehaviour Track;
        public ParticleType Type;
        public Sprite Source;
        public Color Color;
        public Color StartColor;
        public Vector2 Position;
        public Vector2 Speed;
        public float Size;
        public float StartSize;
        public float Life; // 随时间变化, <0 粒子死亡
        public float StartLife; // 定值, 表示存活时间
        public float Rotation;
        public float Spin;
        public float SpinFriction;

        protected override void ManagedFixedUpdate()
        {
            base.ManagedFixedUpdate();
            float delta = Timer.FixedDeltaTime(Type.Realtime);

            // 生命随时间流逝
            Life -= delta;
            if (Life <= 0f)
            {
                Kill();
                return;
            }

            // 面朝速度方向
            if (Type.RotationMode == ParticleType.RotationModes.SameAsSpeedDirection)
            {
                if (Speed != Vector2.zero)
                    Rotation = Speed.Radians();
            }
            else // 旋转
            {
                Rotation += Spin * delta;
                Spin = Approach(Spin, 0, SpinFriction * delta);
            }

            // [0, 1]
            float left = Life / StartLife;
            float alpha = Type.FadeMode switch
            {
                ParticleType.FadeModes.Linear => left,
                ParticleType.FadeModes.Late => Min(1f, left / 0.25f),
                ParticleType.FadeModes.InAndOut => left switch
                {
                    > 0.75f => 1f - (left - 0.75f) / 0.25f, // linear
                    < 0.25f => left / 0.25f, // linear
                    _ => 1f // static
                },
                _ => 1f
            };

            Color = Type.ColorMode switch
            {
                ParticleType.ColorModes.Static => StartColor,
                ParticleType.ColorModes.Lerp => Color.Lerp(Type.Color2, StartColor, left),
                ParticleType.ColorModes.Blink => (Timer.BetweenInterval(Life, 0.1f) ? StartColor : Type.Color2),
                ParticleType.ColorModes.Choose => StartColor,
                _ => Color
            };

            Color = Color.WithA(alpha);

            Position += Speed * delta;
            Speed += Type.Acceleration * delta;
            Speed = Approach(Speed, Vector2.zero, Type.Friction * delta);
            Speed *= Pow(Type.SpeedMultiplier, delta);

            if (Type.ScaleOut)
            {
                Size = StartSize * Ease.CubicEaseInOut(left);
            }
        }

        public override void Render()
        {
            base.Render();
            Vector2 pos = Position;
            if (Track != null)
                pos += (Vector2)Track.transform.position;

            sr.sprite = Source;
            sr.color = Color;
            transform.position = pos;
            transform.eulerAngles = new Vector3(0, 0, RadiansToDegree(Rotation));
            transform.localScale = Vector3.one * Size;
        }

        public virtual Particle ResetData()
        {
            transform.position = Vector3.one * int.MaxValue;
            gameObject.SetActive(true);
            return this;
        }

    }
}