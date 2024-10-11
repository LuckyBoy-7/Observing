using System;
using Lucky.Framework.Extensions;
using Lucky.Framework.Utilities;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Slime
{
    public partial class Slime
    {
        private Vector2 targetPos = MathUtils.GreatVector2;
        private float targetPosChooseRadius = 10; // 在该半径内定位targetPos

        public float RunSpeed = 2;
        private float tryFindLoverDelayTimer;

        private void RunBegin()
        {
            anim.Play("run");
            anim.Speed = 1f;
            tryFindLoverDelayTimer = 2f;
        }

        private void RunEnd()
        {
            anim.PlayEmpty();
        }

        private int RunUpdate()
        {
            // 饿了
            if (CurrentEnergy < HungryThreshold)
                return StEat;


            // tolove
            if (!WantLove)
            {
                SlimeSpawner.Instance.WantLoveSlimes.Remove(this);
                tryFindLoverDelayTimer = 2f;
            }
            else
            {
                tryFindLoverDelayTimer -= Timer.FixedDeltaTime();
                if (tryFindLoverDelayTimer < 0)
                {
                    SlimeSpawner.Instance.WantLoveSlimes.Add(this);
                    // 暂时没啥好算法, 所以就随便找个距离内的就行
                    Slime other = SlimeSpawner.Instance.GetSlimeWantLoveInDist(this, 1000);
                    if (other)
                    {
                        lover = other;
                        other.lover = this;
                        SlimeSpawner.Instance.WantLoveSlimes.Remove(this);
                        SlimeSpawner.Instance.WantLoveSlimes.Remove(other);
                        other.StateMachine.State = StLove;
                        return StLove;
                    }
                }
            }

            // 还没初始化或者到达目的地了, 就roll一个targetPos
            if (targetPos == MathUtils.GreatVector2 || this.Dist(targetPos) < 0.1f)
            {
                targetPos = RandomUtils.RandomPosAroundPoint(transform.position, targetPosChooseRadius);
            }

            rb.velocity = this.Dir(targetPos) * RunSpeed;
            return StRun;
        }
    }
}