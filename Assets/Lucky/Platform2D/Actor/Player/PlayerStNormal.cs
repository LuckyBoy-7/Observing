using System;
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

        // 最大下落速度(不按按键的时候)
        public const float MaxFallSpeedY = 160f; // 最大下落速度
        private const float MaxFastFallSpeedY = 240f;
        private const float FallAccel = 300f;

        // 重力
        private const float Gravity = 900f; // 重力加速度
        private const float HalfGravThreshold = 40f; // y方向速度在范围内时重力减半

        // move
        private const float MaxRunSpeed = 90; // 最大移动速度

        private const float RunAccel = 1000; // 移动时的加速度
        private const float RunReduce = 400; // 移动时的加速度
        private const float AirMult = 0.65f;

        // Jump
        private const float JumpBoostH = 40f; // 起跳时获得的额外水平速度
        private const float JumpSpeedV = 105f; // 起跳时跳跃y方向速度
        private const float JumpGraceTime = 0.1f; // 狼跳时间
        private const float VarJumpTime = 0.2f; // 跳跃初期上升时间
        private const int WallJumpCheckDistH = 3; // 踢墙跳

        // retention
        private const float WallSpeedRetentionTime = 0.06f;

        // slide
        private const float WallSlideTime = 1.2f;
        public const float WallSlideStartSpeedY = 20f; // 开始下滑时的最小速度


        private float varJumpSpeed;
        private float varJumpTimer;
        private float jumpGraceTimer;
        private float wallSpeedRetentionTimer;
        private float wallSpeedRetained;
        private float maxFall;
        private int wallSlideDir;
        private float wallSlideTimer = WallSlideTime;

        private int NormalUpdate()
        {
            // to climb, 必须按着抓, 速度不向上, 面朝方向和速度方向一样, 或者不动的时候
            if (Input.Grab.Check && rb.velocity.y <= 0 && Sign(rb.velocity.x) != -(int)Facing && !IsTired)
            {
                // 进入攀爬状态
                if (CollideCheckBy((int)Facing * ClimbCheckDistH * Vector2.right))
                {
                    return StClimb;
                }

                // 向上吸附
                if (Input.MoveY >= 0f)
                    for (int i = 2; i <= ClimbUpCheckDist; i++)
                    {
                        if (CollideCheckBy(Vector2.up * i + Vector2.right * (int)Facing * ClimbCheckDistH) && !CollideCheckBy(Vector2.up * i))
                        {
                            MoveV(i);
                            return StClimb;
                        }
                    }
            }

            if (DashTriggered)
            {
                return StartDash();
            }

            // 左右移动
            float accelX = Abs(rb.velocity.x) > MaxRunSpeed && Sign(rb.velocity.x) == moveX ? RunReduce : RunAccel;
            if (!onGround)
                accelX *= AirMult;
            rb.SetSpeedX(Approach(rb.velocity.x, MaxRunSpeed * moveX, accelX * Timer.FixedDeltaTime()));

            // 坠落
            if (Input.MoveY.Value == -1f && -rb.velocity.y >= MaxFallSpeedY)
            {
                maxFall = Approach(maxFall, MaxFastFallSpeedY, FallAccel * Timer.FixedDeltaTime());
                float mid = (MaxFallSpeedY + MaxFastFallSpeedY) / 2;
                if (-rb.velocity.y >= mid)
                {
                    float kCloseToFastFall = Min(1f, (-rb.velocity.y - mid) / (MaxFastFallSpeedY - mid));
                    sr.transform.SetScaleX(Lerp(1f, 0.5f, kCloseToFastFall));
                    sr.transform.SetScaleY(Lerp(1f, 1.5f, kCloseToFastFall));
                }
            }
            else
            {
                maxFall = Approach(maxFall, MaxFallSpeedY, FallAccel * Timer.FixedDeltaTime());
            }

            if (!onGround)
            {
                float fallSpeed = maxFall;
                // 对应主动滑墙和没体力硬抓两种情况, 前提是不按着下
                if ((moveX == (int)Facing || (moveX == 0 && Input.Grab.Check)) && Input.MoveY.Value != -1)
                {
                    if (rb.velocity.y <= 0f && wallSlideTimer > 0f && CollideCheckBy(Vector2.right * (int)Facing))
                    {
                        wallSlideDir = (int)Facing;
                        fallSpeed = Lerp(160f, 20f, wallSlideTimer / WallSlideTime);
                    }
                }

                float g = Input.Jump.Check && Abs(rb.velocity.y) < HalfGravThreshold ? Gravity / 2 : Gravity;
                rb.SetSpeedY(Approach(rb.velocity.y, -fallSpeed, g * Timer.FixedDeltaTime()));
            }

            // 跳的上升过程
            if (varJumpTimer > 0f)
            {
                if (Input.Jump.Check)
                    rb.SetSpeedY(Max(rb.velocity.y, varJumpSpeed));
                else
                    varJumpTimer = 0f;
            }

            // 开始跳
            if (Input.Jump.Pressed)
            {
                if (jumpGraceTimer > 0f)
                {
                    Jump();
                }
                else
                {
                    if (WallJumpCheck(1))
                    {
                        // 面朝右向右抓跳
                        if (Facing == Facings.Right && Input.Grab.Check && Stamina > 0)
                        {
                            ClimbJump();
                        }
                        else
                        {
                            WallJump(-1);
                        }
                    }
                    else if (WallJumpCheck(-1))
                    {
                        // 面朝左向左抓跳
                        if (Facing == Facings.Left && Input.Grab.Check && Stamina > 0)
                        {
                            ClimbJump();
                        }
                        else
                        {
                            WallJump(1);
                        }
                    }
                }
            }


            return StNormal;
        }

        private void Jump()
        {
            Input.Jump.ConsumeBuffer();
            if (moveX != 0)
                rb.AddSpeedX(JumpBoostH * moveX);
            rb.SetSpeedY(JumpSpeedV);
            varJumpSpeed = JumpSpeedV;
            varJumpTimer = VarJumpTime;
            wallSlideTimer = WallSlideTime;
        }

        private bool WallJumpCheck(int dir)
        {
            return CollideCheckBy(Vector2.right * dir * WallJumpCheckDistH);
        }
    }
}