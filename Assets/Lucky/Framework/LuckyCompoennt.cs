using UnityEngine;

namespace Lucky.Framework
{
    public class LuckyCompoennt
    {
        public ManagedBehaviour Entity;
        public bool Active = true;
        
        public virtual void Added()
        {
            
        }

        public virtual void Update()
        {
            
        }

        public virtual void FixedUpdate()
        {
            
        }

        public virtual void Removed()
        {
            
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