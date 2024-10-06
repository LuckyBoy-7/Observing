using System;
using System.Collections;
using System.Collections.Generic;
using Lucky.Extensions;
using Lucky.Framework.UI;
using Lucky.Inputs;
using TMPro;
using UnityEngine;
using Input = Lucky.Inputs.Input;

namespace Lucky.Framework.Items
{
    public class KeyBindingButton : Item
    {
        private TMP_Text text;
        private Binding binding;
        private List<KeyIcon> keyIcons = new();
        private TextMenu hintTextMenu;

        public Item Init(TextMenu container, string content, Binding binding)
        {
            Container = container;
            this.binding = binding;
            IncludeWidthInMeasurement = true;
            Selectable = true;

            text = CreateText();
            text.text = content;
            text.fontSize = 60;

            foreach (var keyCode in this.binding.Keys)
            {
                AddIcon(keyCode);
            }

            return this;
        }

        private void AddIcon(KeyCode key)
        {
            KeyIcon icon = this.NewUISonWithComponent<KeyIcon>();
            keyIcons.Add(icon);
            icon.Init(key);
        }

        public override float LeftWidth()
        {
            return text.renderedWidth;
        }

        public override float Height()
        {
            return text.renderedHeight + 12;
        }

        protected override void orig_UpdatePosition(Vector2 position)
        {
            RectTransform.anchoredPosition = position;
            text.rectTransform.anchoredPosition = Vector2.zero;
            float iconWidth = keyIcons.Count > 0 ? keyIcons[0].Width : 0;
            for (var i = 0; i < keyIcons.Count; i++)
            {
                keyIcons[i].RectTransform.anchoredPosition = new Vector2(Container.Width - i * (iconWidth + 10), 0);
            }
        }

        public override void OnConfirmPressed()
        {
            base.OnConfirmPressed();
            // 生成hint panel
            hintTextMenu = this.NewUIRootWithComponent<TextMenu>();
            hintTextMenu.AutoScroll = false;
            hintTextMenu.ShowOverlay = true;
            hintTextMenu.Add(hintTextMenu.NewUISonWithComponent<Title>().Init(hintTextMenu, "Press Key for".ToUpper()));
            hintTextMenu.Add(hintTextMenu.NewUISonWithComponent<Title>().Init(hintTextMenu, text.text.ToUpper(), 80));
            hintTextMenu.transform.position = Settings.ScreenHalfSize;
            Container.Focused = false;
            this.Add(new Coroutine(ReceiveInputCoroutine()));
        }

        protected IEnumerator ReceiveInputCoroutine()
        {
            yield return null; // 防止进来的时候按键影响到
            while (!Input.AnyKeyDown)
                yield return null;
            KeyCode currentKey = Input.GetCurrentPressedKey();
            if (binding.Add(currentKey))
            {
                AddIcon(currentKey);
            }

            Destroy(hintTextMenu.gameObject);
            yield return null; // 防止出去的时候按键影响到
            Container.Focused = true;
        }

        public override void OnAltPressed()
        {
            base.OnAltPressed();
            binding.ClearKeyboard();

            List<KeyIcon> iconToRemove = new();
            foreach (var keyIcon in keyIcons)
            {
                if (!binding.Keys.Contains(keyIcon.key))
                {
                    iconToRemove.Add(keyIcon);
                }
            }

            foreach (var keyIcon in iconToRemove)
            {
                Destroy(keyIcon.gameObject);
                keyIcons.Remove(keyIcon);
            }
        }

        public override void OnStay()
        {
            text.color = Container.HighlightColor;
        }

        public override void OnExit()
        {
            text.color = Color.white;
        }

    }
}