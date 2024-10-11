using Lucky.Framework.Extensions;
using Lucky.Framework;
using Lucky.Platform2D.Enums;
using Lucky.Framework.Utilities;
using UnityEngine;
using Input = Lucky.Framework.Inputs.Input;
using static Lucky.Framework.Utilities.MathUtils;

namespace Lucky.Platform2D.Actor.Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    public partial class Player : ManagedBehaviour
    {
        // debug
        [Header("DebugInfo")] public Vector2 SpeedDebug;
        public Facings FacingsDebug = Facings.Right;
        public int StateName;
        public bool IsLoggingState;
        public float MaxSpeedX;

        [Header("Debug")] public bool IsInfiniteStamina;

        [Header("Others")] public Collider2D Hitbox;
        public SpriteRenderer sr;
        private const int StNormal = 0;
        private const int StClimb = 1;
        private const int StDash = 2;

        private int moveX;
        private Rigidbody2D rb;
        private StateMachine StateMachine;
        private RaycastHit2D[] collideCheckHelper = new RaycastHit2D[1];
        private Facings Facing = Facings.Right;
        private Vector2 lastAim = Vector2.right;
        private Vector2 preSpeed;
        public bool onGround;

        public static readonly Color NormalHairColor = ColorUtils.HexToColor("AC3232");
        public static readonly Color UsedHairColor = ColorUtils.HexToColor("44B7FF");

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();

            StateMachine = new StateMachine();
            StateMachine.SetCallbacks(StNormal, "Normal", null, null, NormalUpdate);
            StateMachine.SetCallbacks(StClimb, "Climb", ClimbBegin, null, ClimbUpdate);
            StateMachine.SetCallbacks(StDash, "Dash", DashBegin, null, DashUpdate, DashCoroutine);
            Add(StateMachine);
        }

        protected override void ManagedFixedUpdate()
        {
            // debug
            if (IsInfiniteStamina)
            {
                Stamina = ClimbMaxStamina;
            }

            // 是否在地面
            onGround = CollideCheckBy(Vector2.down) && rb.velocity.y <= 0;

            // 狼跳
            if (onGround)
            {
                jumpGraceTimer = JumpGraceTime;
            }
            else if (jumpGraceTimer > 0f)
            {
                jumpGraceTimer -= Timer.FixedDeltaTime();
            }

            // 冲刺冷却
            if (dashCooldownTimer > 0f)
            {
                dashCooldownTimer -= Timer.FixedDeltaTime();
            }

            // 回复冲刺
            if (dashRefillCooldownTimer > 0f)
            {
                dashRefillCooldownTimer -= Timer.FixedDeltaTime();
            }
            else if (onGround)
            {
                RefillDash();
            }

            if (wallSlideDir != 0)
            {
                wallSlideTimer = Max(wallSlideTimer - Timer.FixedDeltaTime(), 0f);
                wallSlideDir = 0;
            }

            // 偷体力
            if (wallBoostTimer > 0f)
            {
                wallBoostTimer -= Timer.FixedDeltaTime();
                if (moveX == wallBoostDir)
                {
                    rb.SetSpeedX(WallJumpSpeedH * moveX);
                    Stamina += ClimbJumpStaminaCost;
                    wallBoostTimer = 0f;
                }
            }

            // 贴地回体力
            if (onGround)
            {
                Stamina = ClimbMaxStamina;
                wallSlideTimer = WallSlideTime;
            }

            // 跳跃上升时间
            if (varJumpTimer > 0)
                varJumpTimer -= Timer.FixedDeltaTime();


            // 强制移动(或者说模拟输入)
            if (forceMoveXTimer > 0f)
            {
                forceMoveXTimer -= Timer.FixedDeltaTime();
                moveX = forceMoveX;
            }
            else
            {
                moveX = Input.MoveX.Value;
            }

            // 当前按键方向
            if (Input.MoveX.Value != 0 || Input.MoveY.Value != 0)
                lastAim = new Vector2(Input.MoveX.Value, Input.MoveY.Value).normalized;

            // retention
            if (wallSpeedRetentionTimer > 0f)
            {
                // 逆向了直接取消Retention
                if (Sign(rb.velocity.x) == -Sign(wallSpeedRetained))
                {
                    wallSpeedRetentionTimer = 0f;
                }
                else if (!CollideCheckBy(Vector2.right * Sign(wallSpeedRetained)))
                {
                    // 返还速度
                    rb.SetSpeedX(wallSpeedRetained);
                    wallSpeedRetentionTimer = 0f;
                }
                else
                {
                    wallSpeedRetentionTimer -= Timer.FixedDeltaTime();
                }
            }

            // logic view
            if (moveX != 0 && StateMachine.State != StClimb)
            {
                Facings facings = (Facings)moveX;
                Facing = facings;
            }

            // todo: =============================================================================
            // component
            base.ManagedFixedUpdate();
            UpdateSprite();

            // debug
            preSpeed = rb.velocity;
            SpeedDebug = rb.velocity;
            StateMachine.Log = IsLoggingState;
            FacingsDebug = Facing;
            StateName = StateMachine.State;
            MaxSpeedX = Max(MaxSpeedX, Abs(rb.velocity.x));
        }

        public override void Render()
        {
            base.Render();

            if (Dashes == 0)
            {
                sr.color = UsedHairColor;
            }
            else if (Dashes == 1)
            {
                sr.color = NormalHairColor;
            }

            // 体力不足闪红
            bool flash = Timer.OnInterval(0.1f);
            if (IsTired && flash)
            {
                sr.color = Color.red;
            }


            // 翻转图像
            sr.transform.SetScaleX(Abs(sr.transform.localScale.x) * (int)Facing);
        }

        private void UpdateSprite()
        {
            sr.transform.SetScaleX(Approach(Abs(sr.transform.localScale.x), 1f, 1.75f * Timer.FixedDeltaTime()));
            sr.transform.SetScaleY(Approach(Abs(sr.transform.localScale.y), 1f, 1.75f * Timer.FixedDeltaTime()));
        }

        private bool CollideCheckBy(Vector2 vec)
        {
            return Hitbox.Cast(vec.normalized, collideCheckHelper, vec.magnitude) > 0;
        }

        // 生成残影
        public void CreateTrail()
        {
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (preSpeed.x != rb.velocity.x)
                OnCollideH();
            else if (preSpeed.x != rb.velocity.x)
                OnCollideV();
        }

        private void OnCollideV()
        {
            rb.SetSpeedY(0);
        }

        private void OnCollideH()
        {
            if (wallSpeedRetentionTimer <= 0f)
            {
                wallSpeedRetained = preSpeed.x;
                wallSpeedRetentionTimer = WallSpeedRetentionTime;
            }
        }

        /// <summary>
        /// 因为rb的movePosition不能及时更新, 然后又没什么好的方法, 只能这样了
        /// </summary>
        /// <param name="x"></param>
        private void MoveH(float x)
        {
            if (Hitbox.Cast(Vector2.right * Sign(x), collideCheckHelper, Abs(x)) > 0)
            {
                RaycastHit2D hit = collideCheckHelper[0];
                transform.AddPositionX(hit.distance * Sign(x) - Sign(x) * SmallValue);
                Physics2D.SyncTransforms();
                return;
            }

            transform.AddPositionX(x);
            Physics2D.SyncTransforms();
        }

        private void MoveV(float y)
        {
            if (Hitbox.Cast(Vector2.up * Sign(y), collideCheckHelper, Abs(y)) > 0)
            {
                RaycastHit2D hit = collideCheckHelper[0];
                transform.AddPositionY(hit.distance * Sign(y) - Sign(y) * SmallValue);
                Physics2D.SyncTransforms();
                return;
            }

            transform.AddPositionY(y);
            Physics2D.SyncTransforms();
        }
    }
}