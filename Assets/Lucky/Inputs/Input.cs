using System;
using System.Collections.Generic;
using Lucky.Framework;
using UnityEngine;

namespace Lucky.Inputs
{
    /// <summary>
    /// 对VirtualButton的管理
    /// </summary>
    public static class Input
    {

        #region Keys

        public static void Initialize()
        {
            Esc = new VirtualButton(Settings.Esc, 0.1f);
            Pause = new VirtualButton(Settings.Pause, 0.1f);

            Left = new VirtualButton(Settings.Left, 0);
            Right = new VirtualButton(Settings.Right, 0);
            Up = new VirtualButton(Settings.Up, 0);
            Down = new VirtualButton(Settings.Down, 0);
            Jump = new VirtualButton(Settings.Jump, 0.08f);
            Grab = new VirtualButton(Settings.Grab, 0);
            Dash = new VirtualButton(Settings.Dash, 0.08f);

            MenuLeft = new VirtualButton(Settings.MenuLeft, 0).SetRepeat(0.4f, 0.1f);
            MenuRight = new VirtualButton(Settings.MenuRight, 0).SetRepeat(0.4f, 0.1f);
            MenuUp = new VirtualButton(Settings.MenuUp, 0).SetRepeat(0.4f, 0.1f);
            MenuDown = new VirtualButton(Settings.MenuDown, 0).SetRepeat(0.4f, 0.1f);
            MenuConfirm = new VirtualButton(Settings.MenuConfirm, 0);
            MenuCancel = new VirtualButton(Settings.MenuCancel, 0);
            MenuJournal = new VirtualButton(Settings.MenuJournal, 0);
            MoveX = new VirtualIntegerAxis(Settings.Left, Settings.Right);
            MoveY = new VirtualIntegerAxis(Settings.Down, Settings.Up);
        }

        public static void Update()
        {
            foreach (var button in inputs)
            {
                button.Update();
            }
        }

        public static void FixedUpdate()
        {
            foreach (var button in inputs)
            {
                button.FixedUpdate();
            }
        }

        public static void Register(VirtualInput input) => inputs.Add(input);
        public static void DeRegister(VirtualInput input) => inputs.Remove(input);

        private static List<VirtualInput> inputs = new();
        public static VirtualButton Esc;
        public static VirtualButton Pause;
        public static VirtualButton Left;
        public static VirtualButton Right;
        public static VirtualButton Up;
        public static VirtualButton Down;
        public static VirtualButton Jump;
        public static VirtualButton Grab;
        public static VirtualButton Dash;
        public static VirtualButton MenuLeft;
        public static VirtualButton MenuRight;
        public static VirtualButton MenuUp;
        public static VirtualButton MenuDown;
        public static VirtualButton MenuConfirm;
        public static VirtualButton MenuCancel;
        public static VirtualButton MenuJournal;
        public static VirtualIntegerAxis MoveX;
        public static VirtualIntegerAxis MoveY;

        #endregion

        public static bool AnyKeyDown => UnityEngine.Input.anyKeyDown;

        public static KeyCode GetCurrentPressedKey()
        {
            foreach (KeyCode key in Enum.GetValues(typeof(KeyCode)))
            {
                if (UnityEngine.Input.GetKeyDown(key))
                    return key;
            }

            return KeyCode.None;
        }

        /// 这种只能在Update里用
        public static bool GetKeyDown(KeyCode key) => UnityEngine.Input.GetKeyDown(key);
        public static bool GetKey(KeyCode key) => UnityEngine.Input.GetKey(key);
        public static bool GetKeyUp(KeyCode key) => UnityEngine.Input.GetKeyUp(key);
        public static bool GetMouseButtonDown(int x) => UnityEngine.Input.GetMouseButtonDown(x);

    }
}