using UnityEngine;

namespace Lucky.Framework
{
    public class LuckyComponent
    {
        public ManagedBehaviour Entity;
        public bool Active;
        public virtual bool Visible { get; set; }

        public LuckyComponent()
        {
            Active = true;
            Visible = true;
        }

        public LuckyComponent(bool active, bool visible)
        {
            Active = active;
            Visible = visible;
        }

        public virtual void Added(ManagedBehaviour entity)
        {
            Entity = entity;
        }

        public virtual void Update()
        {
        }

        public virtual void FixedUpdate()
        {
        }

        public virtual void Removed()
        {
            Entity = null;
        }

        public virtual void Render()
        {
        }

        public virtual void RemoveSelf()
        {
            Entity.Remove(this);
        }

        protected void print(object o) => Debug.Log(o);
    }
}