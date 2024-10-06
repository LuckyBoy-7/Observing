using System;

namespace Lucky.Managers.ObjectPool_
{
    public interface IRecycle
    {
        public void OnGet();
        public void OnRelease();

    }
}