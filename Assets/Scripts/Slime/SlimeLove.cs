using System;
using Lucky.Framework.Extensions;
using Lucky.Framework.Utilities;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Slime
{
    public partial class Slime
    {
        [Header("Love")] public float LoveChaseSpeed = 2.5f; // 求爱时的追逐速度
        private bool WantLove => CurrentEnergy >= LoveEnergyThreshold; // 吃饱了才有力气去爱
        private const float LoveEnergyThreshold = 80;
        public Slime lover; // 爱人
        public float BreedTime = 2.5f; // 生孩子所需时间
        private float breedTimer;

        private void LoveBegin()
        {
            breedTimer = -1;
            
            anim.Play("run");
            anim.Speed = 1.5f;
            intentionAnim.Play("love");
        }

        private void LoveEnd()
        {
            intentionAnim.PlayEmpty();
            if (lover && lover.StateMachine.State != StPickedup)
                lover.StateMachine.State = StRun;
            lover = null;
        }

        private int LoveUpdate()
        {
            // 饿了
            if (CurrentEnergy < HungryThreshold)
                return StEat;
            // 生孩子
            if (breedTimer > 0)
            {
                breedTimer -= Timer.FixedDeltaTime();
                if (breedTimer < 0)
                {
                    Vector2 pos = (transform.position + lover.transform.position) / 2;
                    SlimeSpawner.Instance.SpawnSlime(pos, ((Color + lover.Color) / 2).WithA(1));
                    return StRun;
                }
            }

            targetPos = lover.transform.position;
            rb.velocity = this.Dir(targetPos) * LoveChaseSpeed;
            return StLove;
        }

        private void OnCollisionStay2D(Collision2D other)
        {
            if (breedTimer == -1 && other.transform.GetComponent<Slime>() == lover)
                breedTimer = BreedTime;
        }
    }
}