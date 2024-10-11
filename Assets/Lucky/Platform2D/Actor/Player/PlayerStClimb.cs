using Lucky.Framework.Extensions;
using Lucky.Framework.Utilities;
using UnityEngine;
using Input = Lucky.Framework.Inputs.Input;
using static Lucky.Framework.Utilities.MathUtils;

namespace Lucky.Platform2D.Actor.Player
{
    public partial class Player
    {
        private const int ClimbUpCheckDist = 3; // 向上吸附的容错, 由于建立在非像素的基础上, 所以容错稍微大点
        private const int ClimbCheckDistH = 2;
        private const float ClimbEnterSpeedYMult = 0.2f;
        private const float ClimbNoMoveTime = 0.1f; // 无法控制移动的持续时间
        private const float ClimbUpSpeedY = 45f;
        private const float ClimbDownSpeedY = 80f;
        private const float ClimbAccel = 900f; // 加速度
        private const float WallJumpForceTime = 0.16f;
        private const float WallJumpSpeedH = 130f;
        public const float ClimbMaxStamina = 110f; // 最大体力
        private const float ClimbUpStaminaCost = 45.454544f; // 向上攀爬每秒消耗体力
        private const float ClimbStillStaminaCost = 10f; // 抓墙不动每秒消耗体力
        private const float ClimbJumpStaminaCost = 27.5f;
        private const float ClimbJumpBoostTime = 0.2f;
        public const float ClimbTiredThreshold = 20f;

        private float climbNoMoveTimer;
        private int forceMoveX;
        private float forceMoveXTimer;
        public float Stamina;
        public float wallBoostTimer;
        public int wallBoostDir;

        private bool IsTired => CheckStamina < ClimbTiredThreshold;

        // 为偷体力保留了一部分时间
        private float CheckStamina => wallBoostTimer > 0f ? Stamina + ClimbJumpStaminaCost : Stamina;

        private void ClimbBegin()
        {
            rb.SetSpeedX(0);
            rb.SetSpeedY(rb.velocity.y * ClimbEnterSpeedYMult);
            climbNoMoveTimer = ClimbNoMoveTime;
            wallSlideTimer = WallSlideTime;
            // 水平吸附
            MoveH((float)Facing * ClimbCheckDistH);
        }

        private int ClimbUpdate()
        {
            if (climbNoMoveTimer > 0)
                climbNoMoveTimer -= Timer.FixedDeltaTime();

            if (Input.Jump.Pressed)
            {
                if (moveX == -(int)Facing) // 朝反方向跳
                {
                    WallJump(-(int)Facing);
                }
                else // 中性抓跳或者向前抓跳
                {
                    ClimbJump();
                }

                return StNormal;
            }
            
            // 切到冲刺状态
            if (DashTriggered)
            {
                return StartDash();
            }

            // 离开了墙, 一般是滑下去了
            if (!CollideCheckBy(Vector2.right * (int)Facing))
            {
                return StNormal;
            }

            if (!Input.Grab)
            {
                return StNormal;
            }


            if (climbNoMoveTimer <= 0f)
            {
                float speedY = 0;
                if (Input.MoveY.Value == 1)
                {
                    speedY = ClimbUpSpeedY;
                }
                else if (Input.MoveY.Value == -1)
                {
                    speedY = -ClimbDownSpeedY;
                }

                rb.SetSpeedY(Approach(rb.velocity.y, speedY, ClimbAccel * Timer.FixedDeltaTime()));
            }

            if (Input.MoveY.Value == 1) // 正在向上爬
            {
                Stamina -= ClimbUpStaminaCost * Timer.FixedDeltaTime();
            }
            else if (Input.MoveY.Value == 0)
            {
                Stamina -= ClimbStillStaminaCost * Timer.FixedDeltaTime();
            }

            // 没体力了
            if (Stamina <= 0f)
            {
                return StNormal;
            }
            return StClimb;
        }

        private void WallJump(int dir)
        {
            Input.Jump.ConsumeBuffer();
            jumpGraceTimer = 0f;
            if (moveX != 0)
            {
                forceMoveX = dir;
                forceMoveXTimer = WallJumpForceTime;
            }

            rb.SetSpeedX(WallJumpSpeedH * dir);
            rb.SetSpeedY(JumpSpeedV);
            varJumpTimer = VarJumpTime;
            varJumpSpeed = JumpSpeedV;
            wallSlideTimer = WallSlideTime;
        }

        private void ClimbJump()
        {
            // 扣体力
            if (!onGround)
            {
                Stamina -= ClimbJumpStaminaCost;
            }

            // 偷体力
            if (moveX == 0)
            {
                wallBoostDir = -(int)Facing;
                wallBoostTimer = ClimbJumpBoostTime;
            }

            // 抓跳
            Jump();
        }
        public void RefillStamina()
        {
            Stamina = 110f;
        }
    }
}