using System;
using System.Collections.Generic;
using Lucky.Managers;

namespace Lucky.Framework.UI
{
    /// <summary>
    /// 因为Unity动态生成搞UI好像有点麻烦, 所以还是直接在编辑器里摆好, 然后用Manager管理吧
    /// </summary>
    public class UIPanelManager : Singleton<UIPanelManager>
    {
        public List<UIPanel> UIPanels = new();

        private void Start()
        {
            foreach (var uiPanel in UIPanels)
            {
                if (uiPanel is not MainMenu)
                    uiPanel.gameObject.SetActive(false);
            }
        }
    }
}