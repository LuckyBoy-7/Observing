using System;
using System.Collections;
using System.Collections.Generic;
using Lucky.Framework.Extensions;
using Lucky.Framework;
using TMPro;
using UnityEngine;

namespace Lucky.UI
{

    [RequireComponent(typeof(RectTransform))]
    public class LuckyUI : ManagedBehaviour
    {
        public virtual float FontSize { get; set; } = 60;
        public virtual Color FontColor { get; set; } = Color.white;
        public string textPath = "Fonts/TMP/LuckyUIText/LuckyUIText";

        public TMP_Text text;
        protected RectTransform RectTransform;

        private void Awake()
        {
            RectTransform = GetComponent<RectTransform>();
        }

        public virtual LuckyUI Init(string content)
        {
            text = CreateText();
            text.text = content;
            return this;
        }

        protected TMP_Text CreateText()
        {
            TMP_Text text = this.LoadAndInstantiate<TMP_Text>(textPath, true);
            text.rectTransform.anchoredPosition = Vector2.zero;
            text.fontSize = FontSize;
            text.color = FontColor;
            text.verticalAlignment = VerticalAlignmentOptions.Middle; // 垂直方向居中对齐
            text.horizontalAlignment = HorizontalAlignmentOptions.Left; // 水平方向向左对齐

            return text;
        }

    }
}