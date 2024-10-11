using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Lucky.Framework
{

    public class ManagedBehaviour : ManagedBehaviourBase
    {

        #region Components

        public LuckyComponentList Components;

        public void Add(LuckyComponent component)
        {
            Components ??= new(this);
            Components.Add(component);
        }

        public void Remove(LuckyComponent component)
        {
            Components ??= new(this);
            Components.Remove(component);
        }

        #endregion

        #region Tags

        public int Tag { get; set; }

        public bool HasAnyTag(int x) => (Tag | x) != 0;

        #endregion

        private bool CanUpdate
        {
            get
            {
                switch (GameManager.Instance.GameState)
                {
                    case GameManager.GameStateType.Play:
                        return true;
                    case GameManager.GameStateType.Pause:
                        return HasAnyTag(Tags.PauseUpdate);
                    case GameManager.GameStateType.Frozen:
                        return HasAnyTag(Tags.FrozenUpdate);
                    case GameManager.GameStateType.Transition:
                        return HasAnyTag(Tags.TransitionUpdate);
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private bool visible = true;

        public bool Visible
        {
            get => visible;
            set
            {
                if (visible == value)
                    return;
                visible = value;
                GetComponent<Renderer>().enabled = visible;
            }
        }

        #region Update

        // protected virtual void ManagedUpdate()
        // {
        //     Components.Update();
        // }

        protected virtual void ManagedFixedUpdate()
        {
            Components?.FixedUpdate();
        }

        /// <summary>
        /// 把Update Ban了
        /// </summary>
        public override sealed void Update()
        {
            // if (CanUpdate)
            // {
            //     ManagedUpdate();
            // }
        } 

        public override sealed void FixedUpdate()
        {
            if (CanUpdate)
            {
                ManagedFixedUpdate();
                Render(); // 卡爆了(
            }
            // DebugRender();
        }

        public virtual void Render()  // 虽然不是真正意义上的render, 但还是在这儿做个逻辑上的分离
        {
            Components?.Render();
        }

        #endregion

    }

}