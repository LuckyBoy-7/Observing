/// item.ValueWiggler = Wiggler.Create(0.25f, 3f);
// item.SelectWiggler = Wiggler.Create(0.25f, 3f);
// item.ValueWiggler.UseRawDeltaTime = item.SelectWiggler.UseRawDeltaTime = true;

using System;
using TMPro;
using UnityEngine;

namespace Lucky.Framework.Items
{
    public class Button : Item
    {
        private TMP_Text text;
        private Action callback;

        public Item Init(TextMenu container, string content, Action callback)
        {
            Container = container;
            this.callback = callback;
            IncludeWidthInMeasurement = true;
            Selectable = true;

            text = CreateText();
            text.text = content;
            text.fontSize = 60;
            return this;
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
        }

        public override void OnConfirmPressed()
        {
            base.OnConfirmPressed();
            callback?.Invoke();
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