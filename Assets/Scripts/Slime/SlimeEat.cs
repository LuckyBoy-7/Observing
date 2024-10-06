using System;
using Crumb;
using Lucky.Extensions;
using Lucky.Utilities;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Slime
{
    public partial class Slime
    {
        public const float HungryMoveSpeed = 5f;
        public const float MaxEnergy = 120;
        public float CurrentEnergy;
        public const float HungryThreshold = 60;
        public const float FullThreshold = 110;
        public const float EnergyDropSeed = 10;
        public const float EatSpeed = 25;

        private void EatBegin()
        {
            anim.Play("Run");
            anim.speed = 2f;
            intentionAnim.Play("Bread");
        }

        private void EatEnd()
        {
            anim.speed = 2f;
            intentionAnim.Play("Empty");
        }

        private int EatUpdate()
        {   
            if (CurrentEnergy > FullThreshold)
                return RandomUtils.Choose(StRun, StSleep);
            // 还没初始化或者到达目的地了, 就roll一个targetPos
            if (targetPos == Vector2.one * MathUtils.MaxValue || this.Dist(targetPos) < 0.1f)
            {
                targetPos = transform.position + (Vector3)RandomUtils.InsideUnitCircle * targetPosChooseRadius;
            }

            // 找相对近的面包吃
            Crumb.Crumb crumb = null;
            foreach (var c in CrumbManager.Instance.Crumbs.GetDeepestValueList(transform.position))
            {
                if (crumb == null || this.Dist(c) < this.Dist(crumb))
                {
                    crumb = c;
                }
            }

            if (crumb)
                targetPos = crumb.transform.position;


            // 移动
            if (this.Dist(targetPos) < 0.3f)
                rb.velocity = Vector2.zero;
            else
                rb.velocity = this.Dir(targetPos) * HungryMoveSpeed;
            return StEat;
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            // 进食
            var crumb = other.GetComponent<Crumb.Crumb>();
            if (crumb)
            {
                float get = crumb.TakeEnergy(EatSpeed * Timer.FixedDeltaTime());
                CurrentEnergy = MathUtils.Min(CurrentEnergy + get, MaxEnergy);
            }
        }
    }
}