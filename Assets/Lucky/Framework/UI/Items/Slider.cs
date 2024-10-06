namespace Lucky.Framework.Items
{
    public class Slider : Option<int>
    {
        protected override void AfterInit()
        {
            base.AfterInit();
            for (int i = 0; i <= 10; i++)
            {
                Add(i.ToString(), i, i == 0);
            }
        }
    }
}