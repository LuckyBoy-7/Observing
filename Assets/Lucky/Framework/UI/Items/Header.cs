using Lucky.Extensions;
using TMPro;
using UnityEngine;

namespace Lucky.Framework.Items
{
    public class Header : Item
    {
        private TMP_Text text;

        public Item Init(TextMenu container, string title)
        {
            Container = container;
            IncludeWidthInMeasurement = true;
            Selectable = false;

            text = CreateText();
            text.text = title;
            text.fontSize = 40;
            text.color = (Color.white * 0.8f).WithA(1);
            text.verticalAlignment = VerticalAlignmentOptions.Bottom;
            return this;
        }

        public override float LeftWidth()
        {
            return text.preferredWidth;
        }

        public override float Height()
        {
            return text.preferredHeight + 30;
        }

        protected override void orig_UpdatePosition(Vector2 position)
        {
            RectTransform.anchoredPosition = position;
            text.rectTransform.anchoredPosition = new Vector2(0, -Height());
        }
    }
}