using System.Collections;
using System.Collections.Generic;
using Lucky.Framework.Extensions;
using TMPro;
using UnityEngine;

namespace Lucky.UI
{
    /// <summary>
    /// 不可选择, 主要当作hint提示信息
    /// </summary>
    public class LuckyUIText : LuckyUI
    {
        public override float FontSize { get; set; } = 42;
        public override Color FontColor { get; set; } = (Color.white * 0.7f).WithA(1);
    }
}