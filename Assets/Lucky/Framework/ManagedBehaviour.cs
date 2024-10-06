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

        public List<LuckyCompoennt> Components = new();
        private List<LuckyCompoennt> ComponentsToAdd = new();
        private List<LuckyCompoennt> ComponentsToRemove = new();

        public void Add(LuckyCompoennt component)
        {
            ComponentsToAdd.Add(component);
        }

        public void Remove(LuckyCompoennt component)
        {
            ComponentsToRemove.Add(component);
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

        protected virtual void ManagedUpdate()
        {
            foreach (var luckyCompoennt in Components)
            {
                if (luckyCompoennt.Active)
                    luckyCompoennt.Update();
            }
        }

        protected virtual void ManagedFixedUpdate()
        {
            // 移除待移除的组件
            foreach (var luckyCompoennt in ComponentsToRemove)
            {
                Components.Remove(luckyCompoennt);
                luckyCompoennt.Removed();
            }

            ComponentsToRemove.Clear();

            foreach (var luckyComponent in ComponentsToAdd)
            {
                luckyComponent.Entity = this;
                luckyComponent.Added();
                Components.Add(luckyComponent);
            }

            ComponentsToAdd.Clear();

            foreach (var luckyCompoennt in Components)
            {
                if (luckyCompoennt.Active)
                    luckyCompoennt.FixedUpdate();
            }
        }

        public override sealed void Update()
        {
            if (CanUpdate)
            {
                ManagedUpdate();
            }
        }

        public override sealed void FixedUpdate()
        {
            if (CanUpdate)
            {
                ManagedFixedUpdate();
                Render();  // 卡爆了(
            }
            // DebugRender();
        }

        public virtual void Render()
        {
            foreach (var luckyComponent in ComponentsToAdd)
            {
                luckyComponent.Render();
            }
        }
        
        public virtual void DebugRender()
        {
        }

        #endregion

    }

}