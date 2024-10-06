using System.Collections;
using Lucky.Extensions;
using Lucky.Framework.Items;
using Lucky.Inputs;
using UnityEngine;
using UnityEngine.UI;
using Button = Lucky.Framework.Items.Button;
using Slider = Lucky.Framework.Items.Slider;

namespace Lucky.Framework.UI
{
    public class KeyBindingPanel : UIPanel
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

            textMenu.Add(textMenu.NewSonWithComponent<Title>().Init(textMenu, "Keyboard Config"));

            textMenu.Add(textMenu.NewSonWithComponent<Header>().Init(textMenu, "GAMEPLAY"));
            textMenu.Add(textMenu.NewSonWithComponent<KeyBindingButton>().Init(textMenu, "Left", Settings.Left));
            textMenu.Add(textMenu.NewSonWithComponent<KeyBindingButton>().Init(textMenu, "Right", Settings.Right));
            textMenu.Add(textMenu.NewSonWithComponent<KeyBindingButton>().Init(textMenu, "Up", Settings.Up));
            textMenu.Add(textMenu.NewSonWithComponent<KeyBindingButton>().Init(textMenu, "Down", Settings.Down));

            textMenu.Add(textMenu.NewSonWithComponent<Header>().Init(textMenu, "MENU\nEACH ACTION REQUIRES 1 UNIQUE BIND"));
            textMenu.Add(textMenu.NewSonWithComponent<KeyBindingButton>().Init(textMenu, "MenuLeft", Settings.MenuLeft));
            textMenu.Add(textMenu.NewSonWithComponent<KeyBindingButton>().Init(textMenu, "MenuRight", Settings.MenuRight));
            textMenu.Add(textMenu.NewSonWithComponent<KeyBindingButton>().Init(textMenu, "MenuUp", Settings.MenuUp));
            textMenu.Add(textMenu.NewSonWithComponent<KeyBindingButton>().Init(textMenu, "MenuDown", Settings.MenuDown));
            textMenu.Add(textMenu.NewSonWithComponent<KeyBindingButton>().Init(textMenu, "Confirm", Settings.MenuConfirm));
            textMenu.Add(textMenu.NewSonWithComponent<KeyBindingButton>().Init(textMenu, "Cancel", Settings.MenuCancel));
            textMenu.Add(textMenu.NewSonWithComponent<KeyBindingButton>().Init(textMenu, "Journal", Settings.MenuJournal));
            textMenu.Add(textMenu.NewSonWithComponent<KeyBindingButton>().Init(textMenu, "Pause", Settings.Pause));
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