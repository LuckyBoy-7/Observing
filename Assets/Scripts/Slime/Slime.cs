using System;
using System.Collections;
using System.Collections.Generic;
using Lucky.Extensions;
using Lucky.Framework;
using Lucky.Managers.ObjectPool_;
using Lucky.Utilities;
using UnityEngine;

namespace Slime
{
    [RequireComponent(typeof(Rigidbody2D))]
    public partial class Slime : ManagedBehaviour, IRecycle
    {
        public StateMachine StateMachine;
        public const int StRun = 0;
        public const int StLove = 1;
        public const int StEat = 2;
        public const int StSleep = 3;
        public const int StDeath = 4;
        public const int StPickedup = 5;
        public const int StDizzy = 6;
        public int DebugState = 0;

        private Rigidbody2D rb;
        public SpriteRenderer sr;
        public SpriteRenderer intentionSr;
        public Animator anim;
        public Animator intentionAnim;
        public Collider2D collider;

        public float KillRadius;

        public Color Color
        {
            get => sr.color;
            set => sr.color = value;
        }

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            intentionAnim.speed = 0.1f;

            StateMachine = new StateMachine();
            StateMachine.SetCallbacks(StRun, RunUpdate, null, RunBegin, RunEnd);
            StateMachine.SetCallbacks(StLove, LoveUpdate, null, LoveBegin, LoveEnd);
            StateMachine.SetCallbacks(StEat, EatUpdate, null, EatBegin, null);
            StateMachine.SetCallbacks(StSleep, SleepUpdate, null, SleepBegin, null);
            StateMachine.SetCallbacks(StDeath, DeathUpdate, null, DeathBegin, null);
            StateMachine.SetCallbacks(StPickedup, PickedupUpdate, null, PickedupBegin, PickedupEnd);
            StateMachine.SetCallbacks(StDizzy, DizzyUpdate, null, DizzyBegin, null);
            Add(StateMachine);

            CurrentEnergy = MaxEnergy;
        }

        protected override void ManagedFixedUpdate()
        {
            if (transform.position.magnitude > KillRadius)
            {
                StateMachine.State = StDeath;
                return;
            }

            if (StateMachine.State != StPickedup)
                CurrentEnergy -= EnergyDropSeed * Timer.FixedDeltaTime();
            if (CurrentEnergy < 0)
                StateMachine.State = StDeath;
            base.ManagedFixedUpdate();

            DebugState = StateMachine.State;
        }

        public int Kill()
        {
            SlimeSpawner.Instance.TryRemoveWantLove(this);
            ObjectPoolManager.Instance.Release(this);
            return StRun;
        }

        public override void Render()
        {
            base.Render();
            sr.transform.SetScaleX(MathUtils.Sign2(rb.velocity.x));
        }


        private void OnDrawGizmos()
        {
            // 死亡外圈
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(Vector3.zero, KillRadius);

            if (StateMachine.State == StRun)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(transform.position, targetPos);

                // to love
                Gizmos.color = Color.magenta;
                Gizmos.DrawWireSphere(transform.position, LoveViewRadius);
            }
            else if (StateMachine.State == StEat)
            {
                // to love
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(transform.position, targetPos);
            }
        }

        public void OnGet()
        {
            gameObject.SetActive(true);
            SlimeSpawner.Instance.Slimes.Add(this);

            collider.enabled = true;
            CurrentEnergy = MaxEnergy;
            StateMachine.State = StRun;
            sr.color = sr.color.WithA(1);
            intentionSr.color = intentionSr.color.WithA(1);
        }

        public void OnRelease()
        {
            gameObject.SetActive(false);
            SlimeSpawner.Instance.Slimes.Remove(this);
        }
    }
}