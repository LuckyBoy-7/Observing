using System;

namespace Lucky.Framework.Managers.ObjectPool_
{
    public interface IRecycle
    {
        public void OnGet();
        public void OnRelease();

    }
}