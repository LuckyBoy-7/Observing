using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Lucky.Framework.Extensions;
using Lucky.Framework.Managers.ObjectPool_;
using Lucky.Framework.Utilities;
using TMPro;
using UnityEngine;

namespace Lucky.UI
{
    /// <summary>
    /// 可按左右选择对应项, 类似enum
    /// </summary>
    public class LuckyUISelect : LuckyUIInteract
    {
        private List<object> values;
        private Action<object> callback;
        private int idx = -1;
        private int startIdx;

        private TMP_Text rightText;
        private TMP_Text bracketLeft;
        private TMP_Text bracketRight;
        private RectTransform rectTransform1;
        private RectTransform rectTransform2;
        private RectTransform rectTransform3;
        private float origPosX1;
        private float origPosX2;
        private float origPosX3;
        private const float LeftRightWiggleStrength = 20;

        public LuckyUISelect Init<T>(string content, List<T> values, int startIdx, Action<T> callback)
        {
            base.Init(content);
            this.values = values.Cast<object>().ToList();
            this.callback = val => callback((T)val);

            if (startIdx < 0)
                startIdx += values.Count;
            this.startIdx = MathUtils.Clamp(startIdx, 0, this.values.Count - 1);
            return this;
        }

        private void Start()
        {
            // 先放好右尖括号的位置
            bracketRight = CreateText();
            bracketRight.text = ">";
            bracketRight.horizontalAlignment = HorizontalAlignmentOptions.Right;
            float bracketLength = 50; // 大概就是 "空格 + >" 的长度
            // 左边文本的左边到中间的距离
            float gap = -GetComponent<RectTransform>().anchoredPosition.x;
            bracketRight.rectTransform.anchoredPosition = new Vector2(gap * 2, 0);

            // 中间文本
            rightText = CreateText();
            float maxLength = values.Max(
                (val) =>
                {
                    rightText.text = val.ToString();
                    return rightText.preferredWidth;
                }
            );
            rightText.horizontalAlignment = HorizontalAlignmentOptions.Center;
            rightText.rectTransform.anchoredPosition = new Vector2(gap * 2 - bracketLength - maxLength / 2, 0);

            // 左文本
            bracketLeft = CreateText();
            bracketLeft.text = "<";
            bracketLeft.horizontalAlignment = HorizontalAlignmentOptions.Left;
            // 左边文本的左边到中间的距离
            bracketLeft.rectTransform.anchoredPosition = new Vector2(gap * 2 - bracketLength * 2 - maxLength, 0);


            ChangeIdx(startIdx);

            rectTransform1 = bracketLeft.GetComponent<RectTransform>();
            rectTransform2 = rightText.GetComponent<RectTransform>();
            rectTransform3 = bracketRight.GetComponent<RectTransform>();
            origPosX1 = rectTransform1.anchoredPosition.x;
            origPosX2 = rectTransform2.anchoredPosition.x;
            origPosX3 = rectTransform3.anchoredPosition.x;
        }

        public virtual void OnChoose(int dir) // 左-1 右1
        {
            if (dir == -1)
                TryMoveLeft();
            else if (dir == 1)
                TryMoveRight();
        }

        public bool TryMoveLeft()
        {
            if (idx == 0)
                return false;
            ChangeIdx(idx - 1);

            // Wiggler wiggler = Wiggler.Create(
            //     0.3f, 3f, v =>
            //     {
            //         rectTransform1.anchoredPosition = rectTransform1.anchoredPosition.WithX(origPosX1 - v * LeftRightWiggleStrength);
            //         rectTransform2.anchoredPosition = rectTransform2.anchoredPosition.WithX(origPosX2 - v * LeftRightWiggleStrength);
            //     }
            // );

            return true;
        }

        public bool TryMoveRight()
        {
            if (idx == values.Count - 1)
                return false;
            ChangeIdx(idx + 1);

            // Wiggler wiggler = Wiggler.Create(
            //    0.3f, 3f, v =>
            //     {
            //         rectTransform2.anchoredPosition = rectTransform2.anchoredPosition.WithX(origPosX2 + v * LeftRightWiggleStrength);
            //         rectTransform3.anchoredPosition = rectTransform3.anchoredPosition.WithX(origPosX3 + v * LeftRightWiggleStrength);
            //     }
            // );
            return true;
        }

        public void ChangeIdx(int idx)
        {
            this.idx = idx;
            callback(values[idx]);

            rightText.text = $"{values[this.idx]}";
        }


        public override void OnExit()
        {
            base.OnExit();
            text.color = rightText.color = bracketLeft.color = bracketRight.color = Color.white;
        }

        public override void OnStay()
        {
            // 闪烁
            Color color;
            if (Timer.BetweenInterval(0.15f))
                color = new Color(0.8f, 0.8f, 0.2f, 1);
            else
                color = new Color(0.6f, 0.8f, 0.2f, 1);

            bracketLeft.color = idx > 0 ? color : Color.gray;
            bracketRight.color = idx < values.Count - 1 ? color : Color.gray;

            text.color = color;
            rightText.color = color;
        }
    }
}