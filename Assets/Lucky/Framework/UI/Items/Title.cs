using Lucky.Extensions;
using TMPro;
using UnityEngine;

namespace Lucky.Framework.Items
{
    public class Title : Item
    {
        private TMP_Text text;

        public Item Init(TextMenu container, string title, float fontSize=110)
        {
            Container = container;
            IncludeWidthInMeasurement = false;
            Selectable = false;

            text = CreateText();
            text.text = title;
            text.fontSize = fontSize;
            text.color = (Color.white * 0.9f).WithA(1);
            text.horizontalAlignment = HorizontalAlignmentOptions.Center;
            return this;
        }


        public override float Height()
        {
            return text.renderedHeight * 1.5f;
        }

        protected override void orig_UpdatePosition(Vector2 position)
        {
            RectTransform.anchoredPosition = position;
            text.rectTransform.anchoredPosition = new Vector2(Container.Width / 2, 0);
        }

    }

}