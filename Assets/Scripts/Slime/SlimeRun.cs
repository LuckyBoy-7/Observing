using System;
using Lucky.Extensions;
using Lucky.Utilities;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Slime
{
    public partial class Slime
    {
        private Vector2 targetPos = Vector2.one * MathUtils.MaxValue;
        private float targetPosChooseRadius = 10; // 在该半径内定位targetPos

        public float RunSpeed = 2;
        private float tryFindLoverDelayTimer;

        private void RunBegin()
        {
            anim.Play("Run");
            // intentionSr.enabled = false;
            // intentionAnim.Play("Empty");  // empty好像没用, 因为它并不会更改图片
            anim.speed = 1f;
            tryFindLoverDelayTimer = 2f;
        }
        
        private void RunEnd()
        {
            anim.Play("Run");
            // intentionSr.enabled = true;
        }

        private int RunUpdate()
        {
            anim.Play("Run");  // 可能激活的瞬间调用无效
            if (CurrentEnergy < HungryThreshold)
                return StEat;


            if (!WantLove)
            {
                SlimeSpawner.Instance.TryRemoveWantLove(this);
                tryFindLoverDelayTimer = 2f;
            }
            else
            {
                tryFindLoverDelayTimer -= Timer.FixedDeltaTime();
                if (tryFindLoverDelayTimer < 0)
                {
                    SlimeSpawner.Instance.TryAddWantLove(this);
                    Slime other = SlimeSpawner.Instance.GetSlimeWantLoveInDist(this, 1000);
                    // Slime other = SlimeSpawner.Instance.GetSlimeWantLoveInDist(this, LoveViewRadius);
                    if (other)
                    {
                        lover = other;
                        other.lover = this;
                        other.StateMachine.State = StLove;
                        return StLove;
                    }
                }
            }

            // 还没初始化或者到达目的地了, 就roll一个targetPos
            if (targetPos == Vector2.one * MathUtils.MaxValue || this.Dist(targetPos) < 0.1f)
            {
                targetPos = transform.position + (Vector3)RandomUtils.InsideUnitCircle * targetPosChooseRadius;
            }

            rb.velocity = this.Dir(targetPos) * RunSpeed;
            return StRun;
        }
    }
}