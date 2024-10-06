using Lucky.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Lucky.Framework.Items
{
    /// <summary>
    /// 支点在右方
    /// </summary>
    public class KeyIcon : Item
    {
        private Image image;
        private TMP_Text text;
        public KeyCode key;

        public void Init(KeyCode key)
        {
            this.key = key;
            RectTransform.SetPivot(new(1, 0.5f));
            RectTransform.sizeDelta = Vector2.one * 73f;
            image = gameObject.AddComponent<Image>();
            image.sprite = Resources.Load<Sprite>("Primatives/Circle");

            text = CreateText(false);
            text.text = TranslateKeyCode(key);
            text.horizontalAlignment = HorizontalAlignmentOptions.Center;
            text.verticalAlignment = VerticalAlignmentOptions.Geometry;
            text.rectTransform.SetAnchor(new Vector2(0.5f, 0.5f));
            text.rectTransform.anchoredPosition = Vector2.zero;
            text.fontSize = GetFontSize(key);

            // text.rectTransform.anchoredPosition += GetOffset(key);
        }


        public override float RightWidth() => RectTransform.sizeDelta.x;

        private string TranslateKeyCode(KeyCode key)
        {
            switch (key)
            {
                case KeyCode.LeftArrow:
                    return "←";
                case KeyCode.RightArrow:
                    return "→";
                case KeyCode.UpArrow:
                    return "↑";
                case KeyCode.DownArrow:
                    return "↓";
                case KeyCode.Escape:
                    return "Esc";
                default:
                    return key.ToString();
            }
        }

        private float GetFontSize(KeyCode key) 
        {
            switch (key)
            {
                case KeyCode.Tab:
                    return 32;
                case KeyCode.Escape:
                    return 32;
                default:
                    return 50;
            }
        }
    }
}