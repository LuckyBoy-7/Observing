using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lucky.UI
{

    /// <summary>
    /// 可选择, 点击后(键盘上的某个按键, 由更上级驱动)调用对应函数
    /// </summary>
    public class LuckyUIButton : LuckyUIInteract
    {
        public Action callback;

        public LuckyUIButton Init(string content, Action callback)
        {
            base.Init(content);
            this.callback = callback;
            return this;
        }

        public virtual void OnSelected()
        {
            callback?.Invoke();
        }

    }
}