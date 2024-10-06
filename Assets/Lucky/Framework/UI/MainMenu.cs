using System.Collections;
using Lucky.Extensions;
using Lucky.Framework.Items;
using Lucky.Inputs;
using UnityEngine;

namespace Lucky.Framework.UI
{
    public class MainMenu : UIPanel
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

            textMenu.Add(textMenu.NewSonWithComponent<Title>().Init(textMenu, "Lucky Game!!!"));

            textMenu.Add(textMenu.NewSonWithComponent<Button>().Init(textMenu, "Play", () => print("Play")));
            textMenu.Add(textMenu.NewSonWithComponent<Button>().Init(textMenu, "Settings", GoTo<SettingsPanel>));
            textMenu.Add(textMenu.NewSonWithComponent<Button>().Init(textMenu, "Credits", () => print("Credits")));
            textMenu.Add(textMenu.NewSonWithComponent<Button>().Init(textMenu, "Quit", Application.Quit));
        }

        public override IEnumerator OnEnter()
        {
            IsFocused = true;
            textMenu.ResetPositionImmediately();
            yield return null;
        }

        public override IEnumerator OnExit()
        {
            yield return null; // 因为如果先删的话就会导致组件不能被entity调用了
            IsFocused = false;
            gameObject.SetActive(false);
        }
    }
}