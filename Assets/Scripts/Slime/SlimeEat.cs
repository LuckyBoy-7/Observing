using System;
using System.Linq;
using Crumb;
using Lucky.Framework.Extensions;
using Lucky.Framework.Utilities;
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
            anim.Play("run");
            anim.Speed = 2f;
            intentionAnim.Play("bread");
        }

        private void EatEnd()
        {
            intentionAnim.PlayEmpty();
        }

        private int EatUpdate()
        {
            if (CurrentEnergy > FullThreshold)
                return RandomUtils.Choose(StRun, StSleep);
            // 还没初始化或者到达目的地了, 就roll一个targetPos
            if (targetPos == MathUtils.GreatVector2 || this.Dist(targetPos) < 0.1f)
            {
                targetPos = RandomUtils.RandomPosAroundPoint(transform.position, targetPosChooseRadius);
            }

            // 找相对近的面包吃
            Crumb.Crumb crumb = CrumbManager.Instance.Crumbs.GetDeepestValueList(transform.position).ClosestValue(this.Dist, null);
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
                float canEatAmount = MathUtils.Min(MaxEnergy - CurrentEnergy, EatSpeed * Timer.FixedDeltaTime());
                float eatAmount = crumb.TakeEnergy(canEatAmount);
                CurrentEnergy = MathUtils.Min(CurrentEnergy + eatAmount, MaxEnergy);
            }
        }
    }
}