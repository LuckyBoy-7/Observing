using System;
using Lucky.Framework;
using Lucky.Utilities;
using UnityEngine;

namespace Lucky.Inputs
{
    /// <summary>
    /// 对binding的管理, 主要提供buffer, 和重复输入的功能
    /// </summary>
    public class VirtualButton : VirtualInput
    {
        public bool Repeating { get; private set; }
        public Binding Binding;
        public float BufferTime;

        private float firstRepeatTime; // 按下按键的那一刻经过多少时间被视作开始重复输入(常用于菜单中焦点的移动, 一开始顿一下, 然后自动移动)
        private float multiRepeatTime; // 当button被视作重复输入时, 多久trigger一次
        private float repeatTimer;
        private bool canRepeat;

        private float bufferTimer;
        private bool consumed;
        public bool isPressedBackup; // 用于联系update和fixedUpdate
        private bool isPressed; // 用于联系update和fixedUpdate

        public VirtualButton(Binding binding, float bufferTime)
        {
            Binding = binding;
            BufferTime = bufferTime;
        }

        public VirtualButton SetRepeat(float repeatTime) => SetRepeat(repeatTime, repeatTime);

        public VirtualButton SetRepeat(float firstRepeatTime, float multiRepeatTime)
        {
            this.firstRepeatTime = firstRepeatTime;
            this.multiRepeatTime = multiRepeatTime;
            canRepeat = this.firstRepeatTime > 0f;
            if (!canRepeat)
            {
                Repeating = false;
            }

            return this;
        }

        public override void Update()
        {
            // 是否看作按下按键, 按下或者buffer还在且按着的时候
            if (Binding.Pressed())
                isPressed = true;
        }

        public override void FixedUpdate()
        {
            consumed = false;
            bufferTimer -= Timer.FixedDeltaTime();
            isPressedBackup = isPressed;
            // 是否看作按下按键, 按下或者buffer还在且按着的时候
            bool isTreatedPressed = false;
            if (isPressed)
            {
                bufferTimer = BufferTime;
                isPressed = false;
                isTreatedPressed = true;
            }
            else if (Binding.Check())
            {
                isTreatedPressed = true;
            }

            if (!isTreatedPressed)
            {
                Repeating = false;
                repeatTimer = Single.NegativeInfinity;
                bufferTimer = 0f;
                return;
            }

            if (canRepeat)
            {
                Repeating = false;
                if (Single.IsNegativeInfinity(repeatTimer))
                {
                    repeatTimer = firstRepeatTime;
                    return;
                }

                repeatTimer -= Time.deltaTime;
                if (repeatTimer <= 0f)
                {
                    Repeating = true; // 重复模拟输入的那一帧会变成true, 后续又变成false
                    repeatTimer = multiRepeatTime;
                }
            }
        }

        public bool Check => Binding.Check();

        public bool Pressed => !consumed && (bufferTimer > 0f || Repeating || isPressedBackup);

        public bool Released => Binding.Released();

        public void ConsumeBuffer()
        {
            bufferTimer = 0f;
        }

        public void ConsumePress()
        {
            bufferTimer = 0f;
            consumed = true;
        }

        public static implicit operator bool(VirtualButton button)
        {
            return button.Check;
        }

    }
}