using System;
using Lucky.Framework.Extensions;
using Lucky.Framework.Utilities;
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
            dizzyTimer = DizzyTime;
            rb.velocity = Vector2.zero;
            
            anim.Play("dizzy");
            anim.Speed = 1f;
            intentionAnim.Play("dizzy");
        }
        
        private void DizzyEnd()
        {
            intentionAnim.PlayEmpty();
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