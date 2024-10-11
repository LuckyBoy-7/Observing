using System;
using Lucky.Framework.Extensions;
using Lucky.Framework.Utilities;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Slime
{
    public partial class Slime
    {
        public const float SleepTime = 4;
        public float sleepTimer;

        private void SleepBegin()
        {
            anim.Play("sleep");
            anim.Speed = 1f;
            intentionAnim.Play("sleep");
            sleepTimer = SleepTime;
            rb.velocity = Vector2.zero;
        }
        
        private void SleepEnd()
        {
            intentionAnim.PlayEmpty();
        }

        private int SleepUpdate()
        {
            // 因为有时候被撞就滑出去了, 所以要一直赋值
            rb.velocity = Vector2.zero;
            sleepTimer -= Timer.FixedDeltaTime();
            if (sleepTimer < 0)
                return StRun;
            return StSleep;
        }
    }
}