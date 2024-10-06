using System;
using System.Collections.Generic;
using Lucky.Extensions;
using Lucky.Utilities;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

namespace Lucky.Framework.Items
{
    public class Item : ManagedUIBehaviour
    {
        public bool Selectable;

        /// <summary>
        /// 是否参与menu尺寸相关计算, 例如Headr一般不用, button, slider等一般需要, 这并不会影响item的放置, 只会影响width的计算
        /// </summary>
        public bool IncludeWidthInMeasurement = true;

        /// <summary>
        /// TextMenu, 也就是自己的父级
        /// </summary>
        public TextMenu Container;

        /// <summary>
        /// focus时的抖动
        /// </summary>
        public Wiggler SelectWiggler;


        public bool Hoverable => Selectable && Visible;
        private const string WhiteTextPath = "Fonts/TMP/LuckyUIText/WhiteInner";
        private const string BlackTextPath = "Fonts/TMP/LuckyUIText/BlackInner";


        protected override void Awake()
        {
            base.Awake();
            SelectWiggler = Wiggler.Create(0.25f, 3f);
            SelectWiggler.UseRawDeltaTime = true;
            Add(SelectWiggler);
        }

        public TMP_Text CreateText(bool useWhite = true)
        {
            TMP_Text text = this.LoadAndInstantiate<TMP_Text>(useWhite ? WhiteTextPath : BlackTextPath, true);
            text.fontSize = 60;
            text.rectTransform.anchoredPosition = Vector2.zero;
            text.verticalAlignment = VerticalAlignmentOptions.Top;
            text.horizontalAlignment = HorizontalAlignmentOptions.Left;
            text.rectTransform.anchorMin = text.rectTransform.anchorMax = new(0, 1);
            text.rectTransform.pivot = new(0, 1);

            text.rectTransform.anchoredPosition = Vector2.zero;


            return text;
        }

        public float Width => LeftWidth() + RightWidth();

        public virtual void OnConfirmPressed()
        {
        }

        public virtual void OnAltPressed()
        {
        }

        public virtual void OnLeftPressed()
        {
        }

        public virtual void OnRightPressed()
        {
        }

        public virtual void OnEnter()
        {
            SelectWiggler.Start();
        }

        public virtual void OnExit()
        {
        }

        public virtual void OnStay()
        {
        }

        public virtual float LeftWidth()
        {
            return 0f;
        }

        public virtual float RightWidth()
        {
            return 0f;
        }

        public virtual float Height()
        {
            return 0f;
        }

        /// <summary>
        /// 帮子类处理好了SelectWiggler
        /// </summary>
        /// <param name="position"></param>
        public void UpdatePosition(Vector2 position)
        {
            position += new Vector2(0, -SelectWiggler.Value * 12);
            orig_UpdatePosition(position);
        }

        protected virtual void orig_UpdatePosition(Vector2 position)
        {
        }

    }
}