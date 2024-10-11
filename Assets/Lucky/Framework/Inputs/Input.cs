using System;
using System.Collections.Generic;
using Lucky.Framework;
using Lucky.Framework.Collections;
using Lucky.Framework.Utilities;
using UnityEngine;

namespace Lucky.Framework.Inputs
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

            foreach (KeyCode key in Enum.GetValues(typeof(KeyCode)))
            {
                keyStates[key].Append(UnityEngine.Input.GetKey(key));
                while (keyStates[key].Count > 2)
                    keyStates[key].PopLeft();
            }

            foreach (int i in Itertools.Range(3))
            {
                mouseStates[i].Append(UnityEngine.Input.GetMouseButton(i));
                while (mouseStates[i].Count > 2)
                    mouseStates[i].PopLeft();
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

        // 表示对应帧的key的状态, queue的大小为2
        private static DefaultDict<KeyCode, Deque<bool>> keyStates = new(() => new());
        private static DefaultDict<int, Deque<bool>> mouseStates = new(() => new());

        public static bool GetKeyDown(KeyCode key) => (keyStates[key][0] == false) && (keyStates[key][1] == true);
        public static bool GetKey(KeyCode key) => UnityEngine.Input.GetKey(key);
        public static bool GetKeyUp(KeyCode key) => (keyStates[key][0] == true) && (keyStates[key][1] == false);
        public static bool GetMouseButtonDown(int x) => (mouseStates[x][0] == false) && (mouseStates[x][1] == true);
        public static bool GetMouseButton(int x) => UnityEngine.Input.GetMouseButton(x);
        public static bool GetMouseButtonUp(int x) => (mouseStates[x][0] == true) && (mouseStates[x][1] == false);
        public static Vector2 mousePosition => UnityEngine.Input.mousePosition;

    }
}