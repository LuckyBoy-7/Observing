using System;
using System.Collections;
using System.Collections.Generic;
using Lucky.Extensions;
using UnityEngine;
using Input = Lucky.Inputs.Input;

namespace Lucky.Framework.UI
{
    /// <summary>
    /// Unity动态生成UI, 改个位置怎么这么费劲啊, 我还是都用transform把, 反正需求不是很高, 嵌套什么问题也不是很大 
    /// </summary>
    public abstract class UIPanel : ManagedUIBehaviour
    {
        private bool isTransitioning;
        protected virtual bool IsFocused { get; set; } = true;
        private UIPanel fromUIPanel = null;


        protected override void Awake()
        {
            base.Awake();
            UIPanelManager.Instance.UIPanels.Add(this);
        }

        public abstract IEnumerator OnEnter();

        public abstract IEnumerator OnExit();

        public void GoTo<T>() where T : UIPanel
        {
            if (isTransitioning)
                return;
            isTransitioning = true;


            UIPanel other = null;
            foreach (var uiPanel in UIPanelManager.Instance.UIPanels)
            {
                if (uiPanel is T)
                {
                    other = uiPanel;
                    break;
                }
            }

            // transition的时候两者都不能update

            Coroutine coroutine = new Coroutine(OnExit());
            Add(coroutine);
            coroutine.OnFinish += () =>
            {
                isTransitioning = false;
                other.gameObject.SetActive(true);
                other.Add(new Coroutine(other.OnEnter()));
                other.fromUIPanel = this;
            };
        }

        protected override void ManagedUpdate()
        {
            base.ManagedUpdate();
            if (Input.MenuCancel.Pressed && fromUIPanel != null && IsFocused)
            {
                Coroutine coroutine = new Coroutine(OnExit());
                Add(coroutine);
                coroutine.OnFinish += () =>
                {
                    isTransitioning = false;
                    fromUIPanel.gameObject.SetActive(true);
                    fromUIPanel.Add(new Coroutine(fromUIPanel.OnEnter()));
                };
            }
        }
    }
}