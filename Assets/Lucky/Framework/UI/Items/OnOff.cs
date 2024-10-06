namespace Lucky.Framework.Items
{
    public class OnOff : Option<bool>
    {
        protected override void AfterInit()
        {
            base.AfterInit();
            Add("ON", true, true);
            Add("OFF", false, false);
        }
    }
}