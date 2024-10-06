using Lucky.Extensions;
using Lucky.Framework.Items;
using Lucky.Inputs;

namespace Lucky.Framework.UI.Test
{
    public class Test : ManagedUIBehaviour
    {
        private TextMenu textMenu;

        protected override void Awake()
        {
            base.Awake();
            textMenu = this.NewSonWithComponent<TextMenu>();

            textMenu.Add(textMenu.NewSonWithComponent<Title>().Init(textMenu, "Test Title"));

            textMenu.Add(textMenu.NewSonWithComponent<Header>().Init(textMenu, "Test Header1"));
            textMenu.Add(textMenu.NewSonWithComponent<Button>().Init(textMenu, "Test Button", () => print(123)));
            textMenu.Add(textMenu.NewSonWithComponent<Button>().Init(textMenu, "Test Button", () => print(123)));
            textMenu.Add(textMenu.NewSonWithComponent<Button>().Init(textMenu, "Test Button", () => print(123)));

            textMenu.Add(textMenu.NewSonWithComponent<KeyBindingButton>().Init(textMenu, "MenuLeft", Settings.MenuLeft));
            textMenu.Add(textMenu.NewSonWithComponent<KeyBindingButton>().Init(textMenu, "MenuRight", Settings.MenuRight));
            textMenu.Add(textMenu.NewSonWithComponent<KeyBindingButton>().Init(textMenu, "MenuUp", Settings.MenuUp));
            textMenu.Add(textMenu.NewSonWithComponent<KeyBindingButton>().Init(textMenu, "MenuDown", Settings.MenuDown));
            textMenu.Add(textMenu.NewSonWithComponent<KeyBindingButton>().Init(textMenu, "Confirm", Settings.MenuConfirm));
            textMenu.Add(textMenu.NewSonWithComponent<KeyBindingButton>().Init(textMenu, "Cancel", Settings.MenuCancel));
            textMenu.Add(textMenu.NewSonWithComponent<KeyBindingButton>().Init(textMenu, "Journal", Settings.MenuJournal));

            textMenu.Add(textMenu.NewSonWithComponent<Header>().Init(textMenu, "Test Header2"));
            textMenu.Add(textMenu.NewSonWithComponent<Button>().Init(textMenu, "Test Button", () => print(123)));
            textMenu.Add(textMenu.NewSonWithComponent<Button>().Init(textMenu, "Test Button", () => print(123)));
            textMenu.Add(textMenu.NewSonWithComponent<Button>().Init(textMenu, "Test Button", () => print(123)));
            textMenu.Add(textMenu.NewSonWithComponent<Button>().Init(textMenu, "Test Button", () => print(123)));
            textMenu.Add(textMenu.NewSonWithComponent<Button>().Init(textMenu, "Test Button", () => print(123)));
            textMenu.Add(textMenu.NewSonWithComponent<Button>().Init(textMenu, "Test Button", () => print(123)));
            textMenu.Add(textMenu.NewSonWithComponent<Button>().Init(textMenu, "Test Button", () => print(123)));
            textMenu.Add(textMenu.NewSonWithComponent<Button>().Init(textMenu, "Test Button", () => print(123)));
            textMenu.Add(textMenu.NewSonWithComponent<Button>().Init(textMenu, "Test Button", () => print(123)));
            textMenu.Add(
                textMenu.NewSonWithComponent<OnOff>().Init(textMenu, "Test OnOff", (val) => print(val))
            );

            textMenu.Add(
                textMenu.NewSonWithComponent<Slider>().Init(textMenu, "Test Slider", (val) => print(val))
            );
        }
    }
}