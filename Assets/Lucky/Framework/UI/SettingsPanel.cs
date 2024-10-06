using System.Collections;
using Lucky.Extensions;
using Lucky.Framework.Items;
using Lucky.Inputs;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Button = Lucky.Framework.Items.Button;
using Slider = Lucky.Framework.Items.Slider;

namespace Lucky.Framework.UI
{
    public class SettingsPanel : UIPanel
    {
        private TextMenu textMenu;


        protected override bool IsFocused
        {
            get => textMenu.Focused;
            set => textMenu.Focused = value;
        }

        protected override void Awake()
        {
            base.Awake();

            textMenu = this.NewSonWithComponent<TextMenu>();

            textMenu.Add(textMenu.NewSonWithComponent<Title>().Init(textMenu, "Settings"));

            textMenu.Add(textMenu.NewSonWithComponent<Button>().Init(textMenu, "Keyboard Config", GoTo<KeyBindingPanel>));

            textMenu.Add(
                textMenu.NewSonWithComponent<OnOff>().Init(
                    textMenu, "FullScreen", (val) => { Screen.fullScreenMode = val ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed; }
                )
            );
        }


        public override IEnumerator OnEnter()
        {
            IsFocused = true;
            RectTransform.anchoredPosition = Vector3.zero;
            textMenu.RectTransform.anchoredPosition = Vector2.zero;
            textMenu.ResetPositionImmediately();
            yield return null;
        }

        public override IEnumerator OnExit()
        {
            yield return null;
            IsFocused = false;
            gameObject.SetActive(false);
        }
    }
}