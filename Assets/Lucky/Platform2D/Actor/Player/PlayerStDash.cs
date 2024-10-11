using System.Collections;
using Lucky.Framework.Extensions;
using Lucky.Framework;
using Lucky.Platform2D.Enums;
using Lucky.Framework.Utilities;
using UnityEngine;
using Input = Lucky.Framework.Inputs.Input;
using static Lucky.Framework.Utilities.MathUtils;

namespace Lucky.Platform2D.Actor.Player
{
    public partial class Player
    {
        private const float DashCooldown = 0.2f; // 冲刺冷却时间
        private const float DashRefillCooldown = 0.1f; // 可以回复冲刺的冷却时间
        private const float DashSpeed = 240f; // 初始冲刺速度
        private const float EndDashSpeed = 160f; // 结束冲刺时的速度
        private const float EndDashUpMult = 0.75f;

        public bool DashTriggered => Input.Dash.Pressed && CanDash;
        public bool CanDash => dashCooldownTimer <= 0f && Dashes > 0;

        public int Dashes;
        public int MaxDashes = 1;
        private float dashCooldownTimer;
        private float dashRefillCooldownTimer;
        private float dashTrailTimer; // 生成残影的持续时间
        private int dashTrailCounter; // 生成的个数, 通过timer不断消减
        private Vector2 beforeDashSpeed; // 生成的个数, 通过timer不断消减
        public Vector2 DashDir; // 生成的个数, 通过timer不断消减

        private void DashBegin()
        {
            wallSlideTimer = WallSlideTime;
            if (Time.timeScale > 0.25f)
                GameManager.Instance.Freeze(0.0166666f);
                // GameManager.Instance.Freeze(0.05f);
            dashCooldownTimer = DashCooldown;
            dashRefillCooldownTimer = DashRefillCooldown;
            dashTrailTimer = 0f;
            dashTrailCounter = 0;
            beforeDashSpeed = rb.velocity;
            rb.velocity = Vector2.zero;
            DashDir = Vector2.zero;
        }

        private int DashUpdate()
        {
            if (dashTrailTimer > 0f)
            {
                dashTrailTimer -= Timer.FixedDeltaTime();
                if (dashTrailTimer <= 0f)
                {
                    CreateTrail();
                    dashTrailCounter--;
                    if (dashTrailCounter > 0)
                    {
                        dashTrailTimer = 0.1f;
                    }
                }
            }

            // jump/climb jump
            if (Input.Jump.Pressed)
            {
                if (WallJumpCheck(1))
                {
                    if (Facing == Facings.Right && Input.Grab.Check && Stamina > 0f)
                    {
                        ClimbJump();
                    }
                    else
                    {
                        WallJump(-1);
                    }

                    return StNormal;
                }

                if (WallJumpCheck(-1))
                {
                    if (Facing == Facings.Left && Input.Grab.Check && Stamina > 0f)
                    {
                        ClimbJump();
                    }
                    else
                    {
                        WallJump(1);
                    }

                    return StNormal;
                }
            }

            return StDash;
        }

        private IEnumerator DashCoroutine()
        {
            yield return null;

            Vector2 dir = lastAim;
            DashDir = dir;
            Vector2 speed = dir * DashSpeed;
            // 如果冲刺前速度更大且跟冲刺方向一致, 则速度保留
            if (Sign(beforeDashSpeed.x) == Sign(speed.x) && Abs(beforeDashSpeed.x) > Abs(speed.x))
            {
                speed.x = beforeDashSpeed.x;
            }

            rb.velocity = speed;

            // view
            if (DashDir.x != 0f)
                Facing = (Facings)Sign(DashDir.x);


            // 撞地
            if (onGround && DashDir.x != 0f && DashDir.y < 0f && rb.velocity.y < 0f)
            {
                DashDir.x = Sign(DashDir.x);
                DashDir.y = 0f;
                rb.SetSpeedY(0);
                rb.MulSpeedX(1.2f);
            }

            CreateTrail();
            dashTrailTimer = 0.08f;
            dashTrailCounter = 1;

            yield return 0.15f;

            CreateTrail();
            if (DashDir.y >= 0f)
            {
                rb.velocity = DashDir * EndDashSpeed;
            }

            if (rb.velocity.y > 0f)
            {
                rb.MulSpeedY(EndDashUpMult);
            }

            StateMachine.State = 0;
        }

        public int StartDash()
        {
            Dashes = Max(0, Dashes - 1);
            Input.Dash.ConsumeBuffer();
            return StDash;
        }

        public bool RefillDash()
        {
            if (Dashes < MaxDashes)
            {
                Dashes = MaxDashes;
                return true;
            }

            return false;
        }
    }
}