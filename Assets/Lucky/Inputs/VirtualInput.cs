namespace Lucky.Inputs
{
    public abstract class VirtualInput
    {
        public VirtualInput()
        {
            Input.Register(this);
        }

        public void Deregister()
        {
            Input.Register(this);
        }

        public abstract void Update();
        public abstract void FixedUpdate();

        public enum OverlapBehaviors  // 同轴的两个方向一起按下是什么行为, 例如左右都按着
        {
            CancelOut,  // 不动
            TakeOlder,  // 用先前的
            TakeNewer  // 用后来的 
        }
    }
}