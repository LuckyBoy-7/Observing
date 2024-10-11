using System;
using System.Collections;
using System.Collections.Generic;
using Lucky.Framework.Extensions;
using Lucky.Framework;
using Lucky.Framework.Managers.ObjectPool_;
using Lucky.Framework.Utilities;
using UnityEngine;
using Animator = Lucky.Framework.Animation.Animator;

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
        private Animator anim;
        [HideInInspector] public Animator intentionAnim;
        public Collider2D collider;

        public float KillRadius;

        public Color Color
        {
            get => anim.Color;
            set => anim.Color = value;
        }

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();

            anim = Animator.CreateById("slime");
            Add(anim);

            intentionAnim = Animator.CreateById("intention");
            Add(intentionAnim);
            intentionAnim.transform.localPosition = Vector3.up * 1.5f;

            StateMachine = new StateMachine();
            StateMachine.SetCallbacks(StRun, RunUpdate, null, RunBegin, RunEnd);
            StateMachine.SetCallbacks(StLove, LoveUpdate, null, LoveBegin, LoveEnd);
            StateMachine.SetCallbacks(StEat, EatUpdate, null, EatBegin, EatEnd);
            StateMachine.SetCallbacks(StSleep, SleepUpdate, null, SleepBegin, SleepEnd);
            StateMachine.SetCallbacks(StDeath, DeathUpdate, null, DeathBegin, DeathEnd);
            StateMachine.SetCallbacks(StPickedup, PickedupUpdate, null, PickedupBegin, PickedupEnd);
            StateMachine.SetCallbacks(StDizzy, DizzyUpdate, null, DizzyBegin, DizzyEnd);
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

            if (StateMachine.State != StPickedup && StateMachine.State != StSleep)
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
            anim.transform.SetScaleX(MathUtils.Sign2(rb.velocity.x));
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
            Color = Color.WithA(1);
            intentionAnim.Color = intentionAnim.Color.WithA(1);
            intentionAnim.PlayEmpty();
        }

        public void OnRelease()
        {
            gameObject.SetActive(false);
            SlimeSpawner.Instance.Slimes.Remove(this);
        }
    }
}