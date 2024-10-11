using System;
using System.Collections;
using System.Collections.Generic;
using Lucky.Framework.Extensions;
using Lucky.Framework.Managers.ObjectPool_;
using Lucky.Framework.Utilities;
using TMPro;
using UnityEngine;

namespace Lucky.UI
{

    public class LuckyUIInteract : LuckyUI
    {
        protected List<TMP_Text> showingTexts = new();
        private bool isSelected = false;
        private Wiggler enterWiggler;

        private void Start()
        {
            showingTexts.Add(text);
        }

        public virtual void OnEnter()
        {
            isSelected = true;

            // 下沉wiggle
            float origY = RectTransform.anchoredPosition.y;
            enterWiggler = Wiggler.Create(0.3f, 2.5f, v => { RectTransform.anchoredPosition = RectTransform.anchoredPosition.WithY(origY - v * 12); });
        }

        public virtual void OnExit()
        {
            isSelected = false;
            text.color = Color.white;
        }

        public virtual void OnStay()
        {
            // 闪烁
            if (Timer.BetweenInterval(0.15f))
                text.color = new Color(0.8f, 0.8f, 0.2f, 1);
            else
                text.color = new Color(0.6f, 0.8f, 0.2f, 1);
        }

        protected override void ManagedFixedUpdate()
        {
            base.ManagedFixedUpdate();
            if (isSelected)
                OnStay();
        }
    }
}