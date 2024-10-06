using System;
using Lucky.Extensions;
using Lucky.Utilities;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Slime
{
    public partial class Slime
    {
        public const float DizzyTime = 3;
        public float dizzyTimer;

        private void DizzyBegin()
        {
            anim.Play("Dizzy");
            anim.speed = 1f;
            intentionAnim.Play("Dizzy");
            dizzyTimer = DizzyTime;
            rb.velocity = Vector2.zero;
        }

        private int DizzyUpdate()
        {
            // 因为有时候被撞就滑出去了, 所以要一直赋值
            rb.velocity = Vector2.zero;
            dizzyTimer -= Timer.FixedDeltaTime();
            if (dizzyTimer < 0)
                return StRun;
            return StDizzy;
        }
    }
}