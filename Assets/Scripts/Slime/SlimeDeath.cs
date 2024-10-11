using System;
using Lucky.Framework.Extensions;
using Lucky.Framework.Utilities;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Slime
{
    public partial class Slime
    {
        public const float DeathTime = 1;
        public float deathTimer;

        private void DeathBegin()
        {
            deathTimer = DeathTime;
            rb.velocity = Vector2.zero;
            collider.enabled = false;
            
            anim.Play("death");
            anim.Speed = 1f;
            intentionAnim.Play("death");
        }

        private void DeathEnd()
        {
            intentionAnim.PlayEmpty();
        }

        private int DeathUpdate()
        {
            // 因为有时候被撞就滑出去了, 所以要一直赋值
            rb.velocity = Vector2.zero;
            deathTimer -= Timer.FixedDeltaTime();
            float alpha = MathUtils.Min(deathTimer / DeathTime + 0.2f, 1);
            Color = Color.WithA(alpha);
            intentionAnim.Color = intentionAnim.Color.WithA(alpha);
            if (deathTimer < 0)
            {
                Kill();
            }

            return StDeath;
        }
    }
}