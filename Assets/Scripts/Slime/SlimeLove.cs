using System;
using Lucky.Extensions;
using Lucky.Utilities;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Slime
{
    public partial class Slime
    {
        [Header("Love")] public float LoveChaseSpeed = 2.5f;
        public bool WantLove => CurrentEnergy >= LoveEnergyThreshold;
        public float LoveViewRadius = 10f;
        public Slime lover;
        public float BreedTime = 2.5f;
        private float breedTimer;
        public const float LoveEnergyThreshold = 80;

        private void LoveBegin()
        {
            // 因为可能是preState过来的, 所以此时lover没了
            if (lover == null)
            {
                StateMachine.State = StRun;
                return;
            }
            anim.Play("Run");
            anim.speed = 1.5f;
            breedTimer = 0;
            intentionAnim.Play("Love");
        }

        private void LoveEnd()
        {
            if (lover && lover.StateMachine.State != StPickedup)
                lover.StateMachine.State = StRun;
            lover = null;
        }

        private int LoveUpdate()
        {
            if (CurrentEnergy < HungryThreshold)
                return StEat;
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
            if (breedTimer > 0)
                return;
            if (other.transform.GetComponent<Slime>() == lover)
                breedTimer = BreedTime;
        }
    }
}