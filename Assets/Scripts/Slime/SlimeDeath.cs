using System;
using Lucky.Extensions;
using Lucky.Utilities;
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
            anim.Play("Death");
            anim.speed = 1f;
            intentionAnim.Play("Death");
            deathTimer = DeathTime;
            rb.velocity = Vector2.zero;
            collider.enabled = false;
        }

        private int DeathUpdate()
        {
            // 因为有时候被撞就滑出去了, 所以要一直赋值
            rb.velocity = Vector2.zero;
            deathTimer -= Timer.FixedDeltaTime();
            float alpha = MathUtils.Min(deathTimer / DeathTime + 0.2f, 1);
            sr.color = sr.color.WithA(alpha);
            intentionSr.color = intentionSr.color.WithA(alpha);
            if (deathTimer < 0)
            {
                return Kill();
            }

            return StDeath;
        }
    }
}