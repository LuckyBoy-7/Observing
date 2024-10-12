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
            anim.MaskInteraction = SpriteMaskInteraction.VisibleInsideMask;
            
            intentionAnim = Animator.CreateById("intention");
            Add(intentionAnim);
            intentionAnim.MaskInteraction = SpriteMaskInteraction.VisibleInsideMask;
            intentionAnim.transform.localPosition = Vector3.up * 1.5f;

            StateMachine = new StateMachine();
            StateMachine.SetCallbacks(StRun, "Run", RunBegin, RunEnd, RunUpdate);
            StateMachine.SetCallbacks(StLove, "Love", LoveBegin, LoveEnd, LoveUpdate);
            StateMachine.SetCallbacks(StEat, "Eat", EatBegin, EatEnd, EatUpdate);
            StateMachine.SetCallbacks(StSleep, "Sleep", SleepBegin, SleepEnd, SleepUpdate);
            StateMachine.SetCallbacks(StDeath, "Death", DeathBegin, DeathEnd, DeathUpdate);
            StateMachine.SetCallbacks(StPickedup, "Pickedup", PickedupBegin, PickedupEnd, PickedupUpdate);
            StateMachine.SetCallbacks(StDizzy, "Dizzy", DizzyBegin, DizzyEnd, DizzyUpdate);
            Add(StateMachine);
        }

        protected override void ManagedFixedUpdate()
        {
            // 走太远直接kill
            if (transform.position.magnitude > KillRadius)
            {
                StateMachine.State = StDeath;
                return;
            }

            // 掉体力
            if (!StateMachine.AnyEqual(StPickedup, StSleep, StDizzy))
            {
                CurrentEnergy -= EnergyDropSeed * Timer.FixedDeltaTime();
                if (CurrentEnergy < 0)
                    StateMachine.State = StDeath;
            }

            base.ManagedFixedUpdate();

            DebugState = StateMachine.State;
        }

        public void Kill()
        {
            SlimeSpawner.Instance.WantLoveSlimes.Remove(this);
            ObjectPoolManager.Instance.Release(this);
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
                // Gizmos.DrawWireSphere(transform.position, LoveViewRadius);
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
            Color = Color.WithA(1);
            intentionAnim.Color = intentionAnim.Color.WithA(1);
            
            StateMachine.State = StRun;
        }

        public void OnRelease()
        {
            gameObject.SetActive(false);
            SlimeSpawner.Instance.Slimes.Remove(this);
        }
    }
}