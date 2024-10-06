using System;
using System.Collections.Generic;
using Lucky.Extensions;
using Lucky.Utilities;
using TMPro;
using UnityEngine;

namespace Lucky.Framework.Items
{
    public class Option<T> : Item
    {
        private TMP_Text leftText;
        private TMP_Text rightText;
        private TMP_Text bracketLeft;
        private TMP_Text bracketRight;
        private List<Tuple<string, T>> valuePairs = new();
        private Action<T> callback;
        private int idx = -1;
        private float maxRightWidth;
        private int valueDir;

        /// <summary>
        /// 更改值时的抖动
        /// </summary>
        public Wiggler ValueWiggler;

        private float valueWiggleStrength = 12;


        public Option<T> Init(TextMenu container, string content, Action<T> callback)
        {
            Container = container;
            this.callback = callback;

            IncludeWidthInMeasurement = true;
            Selectable = true;

            ValueWiggler = Wiggler.Create(0.3f, 3f);
            ValueWiggler.UseRawDeltaTime = true;
            Add(ValueWiggler);

            leftText = CreateText();
            leftText.text = content;

            rightText = CreateText();
            rightText.text = "";
            rightText.horizontalAlignment = HorizontalAlignmentOptions.Center;

            bracketLeft = CreateText();
            bracketLeft.text = "<";

            bracketRight = CreateText();
            bracketRight.horizontalAlignment = HorizontalAlignmentOptions.Right;
            bracketRight.text = ">";

            AfterInit();
            return this;
        }

        protected virtual void AfterInit()
        {
        }

        public Option<T> Add(string label, T value, bool selected = false)
        {
            valuePairs.Add(new(label, value));
            if (selected)
            {
                idx = valuePairs.Count - 1;
                UpdateText();
                OnChangeValue();
            }

            bracketLeft.color = idx > 0 ? Container.HighlightColor : Container.DisabledColor;
            bracketRight.color = idx < valuePairs.Count - 1 ? Container.HighlightColor : Container.DisabledColor;
            RecalculateWidth();
            return this;
        }

        public override float LeftWidth()
        {
            return leftText.renderedWidth * 2;
        }

        public override float RightWidth()
        {
            return maxRightWidth + 120f;
        }


        public override float Height()
        {
            return leftText.renderedHeight + 12;
        }

        protected override void orig_UpdatePosition(Vector2 position)
        {
            RectTransform.anchoredPosition = position;
            leftText.rectTransform.anchoredPosition = Vector2.zero;
            float leftAdd = valueDir == -1 ? -ValueWiggler.Value * valueWiggleStrength : 0;
            bracketLeft.rectTransform.anchoredPosition = new Vector2(Container.Width - RightWidth() + leftAdd, 0);
            float rightAdd = valueDir == 1 ? ValueWiggler.Value * valueWiggleStrength : 0;
            bracketRight.rectTransform.anchoredPosition = new Vector2(Container.Width + rightAdd, 0);
            rightText.rectTransform.anchoredPosition = new Vector2(Container.Width - RightWidth() / 2 + leftAdd + rightAdd, 0);
        }

        public override void OnStay()
        {
            leftText.color = Container.HighlightColor;
            bracketLeft.color = idx > 0 ? Container.HighlightColor : Container.DisabledColor;
            rightText.color = Container.HighlightColor;
            bracketRight.color = idx < valuePairs.Count - 1 ? Container.HighlightColor : Container.DisabledColor;
        }

        public override void OnExit()
        {
            leftText.color = Color.white;
            rightText.color = Color.white;
            bracketLeft.color = idx > 0 ? Color.white : Container.DisabledColor;
            bracketRight.color = idx < valuePairs.Count - 1 ? Color.white : Container.DisabledColor;
        }

        public override void OnRightPressed()
        {
            if (idx == valuePairs.Count - 1)
                return;
            idx += 1;
            base.OnRightPressed();
            OnChangeValue();
            UpdateText();

            valueDir = 1;
            ValueWiggler.Start();
        }

        public override void OnLeftPressed()
        {
            if (idx == 0)
                return;
            idx -= 1;
            OnChangeValue();
            UpdateText();

            valueDir = -1;
            ValueWiggler.Start();
        }

        private void UpdateText()
        {
            rightText.text = valuePairs[idx].Item1;
        }

        private void OnChangeValue()
        {
            callback?.Invoke(valuePairs[idx].Item2);
        }

        private void RecalculateWidth()
        {
            string backup = rightText.text;
            rightText.text = valuePairs[^1].Item1;
            rightText.ForceMeshUpdate();
            maxRightWidth = MathUtils.Max(maxRightWidth, rightText.renderedWidth);
            rightText.text = backup;
        }
    }
}