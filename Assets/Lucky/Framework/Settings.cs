using Lucky.Inputs;
using Lucky.Managers;
using UnityEngine;

namespace Lucky.Framework
{
    public static class Settings
    {

        public static void Initialize()
        {
            SetDefaultKeyboardControls(false);
        }

        public static void SetDefaultKeyboardControls(bool reset)
        {
            Binding.SetExclusive(MenuConfirm, MenuCancel, MenuJournal, MenuLeft, MenuRight, MenuUp, MenuDown);
            if (reset || Esc.Keys.Count <= 0)
            {
                Esc.Keys.Clear();
                Esc.Add(KeyCode.Escape);
            }

            if (reset || Pause.Keys.Count <= 0)
            {
                Pause.Keys.Clear();
                Pause.Add(KeyCode.Escape);
            }

            if (reset || Left.Keys.Count <= 0)
            {
                Left.Keys.Clear();
                // Left.Add(KeyCode.LeftArrow);
                Left.Add(KeyCode.J);
            }

            if (reset || Right.Keys.Count <= 0)
            {
                Right.Keys.Clear();
                // Right.Add(KeyCode.RightArrow);
                Right.Add(KeyCode.L);
            }

            if (reset || Down.Keys.Count <= 0)
            {
                Down.Keys.Clear();
                // Down.Add(KeyCode.DownArrow);
                Down.Add(KeyCode.K);
            }

            if (reset || Up.Keys.Count <= 0)
            {
                Up.Keys.Clear();
                // Up.Add(KeyCode.UpArrow);
                Up.Add(KeyCode.I);
            }

            if (reset || Jump.Keys.Count <= 0)
            {
                Jump.Keys.Clear();
                // Jump.Add(KeyCode.C);
                // Jump.Add(KeyCode.S);
                Jump.Add(KeyCode.W);
                Jump.Add(KeyCode.D);
            }

            if (reset || Grab.Keys.Count <= 0)
            {
                Grab.Keys.Clear();
                // Grab.Add(KeyCode.Z);
                Grab.Add(KeyCode.A);
            }

            if (reset || Dash.Keys.Count <= 0)
            {
                Dash.Keys.Clear();
                // Dash.Add(KeyCode.X);
                Dash.Add(KeyCode.S);
            }

            if (reset || MenuLeft.Keys.Count <= 0)
            {
                MenuLeft.Keys.Clear();
                MenuLeft.Add(KeyCode.LeftArrow);
            }

            if (reset || MenuRight.Keys.Count <= 0)
            {
                MenuRight.Keys.Clear();
                MenuRight.Add(KeyCode.RightArrow);
            }

            if (reset || MenuDown.Keys.Count <= 0)
            {
                MenuDown.Keys.Clear();
                MenuDown.Add(KeyCode.DownArrow);
            }

            if (reset || MenuUp.Keys.Count <= 0)
            {
                MenuUp.Keys.Clear();
                MenuUp.Add(KeyCode.UpArrow);
            }


            if (reset || MenuConfirm.Keys.Count <= 0)
            {
                MenuConfirm.Keys.Clear();
                MenuConfirm.Add(KeyCode.C);
            }

            if (reset || MenuCancel.Keys.Count <= 0)
            {
                MenuCancel.Keys.Clear();
                MenuCancel.Add(KeyCode.X);
            }

            if (reset || MenuJournal.Keys.Count <= 0)
            {
                MenuJournal.Keys.Clear();
                MenuJournal.Add(KeyCode.Tab);
            }
        }

        public static bool Fullscreen;

        public static bool DisableFlashes;

        public static Binding Esc = new Binding();

        public static Binding Pause = new Binding();

        public static Binding Left = new Binding();

        public static Binding Right = new Binding();

        public static Binding Down = new Binding();

        public static Binding Up = new Binding();
        
        public static Binding Jump = new Binding();
        
        public static Binding Grab = new Binding();
        
        public static Binding Dash = new Binding();

        public static Binding MenuLeft = new Binding();

        public static Binding MenuRight = new Binding();

        public static Binding MenuDown = new Binding();

        public static Binding MenuUp = new Binding();

        public static Binding MenuConfirm = new Binding();

        public static Binding MenuCancel = new Binding();

        public static Binding MenuJournal = new Binding();
        public static Vector2 ScreenSize => new Vector2(Screen.width, Screen.height);
        public static Vector2 ScreenHalfSize => ScreenSize / 2;

    }
}