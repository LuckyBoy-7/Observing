using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Lucky.Framework
{
    public class LuckyComponentList : IEnumerable<LuckyComponent>, IEnumerable
    {
        public ManagedBehaviour Entity { get; set; }

        private List<LuckyComponent> components; // 当前LuckyComponent列表, 即能被更新和渲染的
        private List<LuckyComponent> toAdd; // 等待被添加的, 因为有时候在遍历的时候会生成的componnet
        private List<LuckyComponent> toRemove; // 等待被删除的, 因为有时候在遍历的时候会删除一些componnet
        private HashSet<LuckyComponent> current; // 防止正在添加时方便判重的
        private HashSet<LuckyComponent> adding; // 防止将要添加时方便判重的
        private HashSet<LuckyComponent> removing; // 防止将要删除时方便判重的
        private LockModes lockMode;

        public enum LockModes
        {
            Open,
            Locked,
            Error
        }

        public LuckyComponentList(ManagedBehaviour entity)
        {
            Entity = entity;
            components = new List<LuckyComponent>();
            toAdd = new List<LuckyComponent>();
            toRemove = new List<LuckyComponent>();
            current = new HashSet<LuckyComponent>();
            adding = new HashSet<LuckyComponent>();
            removing = new HashSet<LuckyComponent>();
        }

        public LockModes LockMode
        {
            get => lockMode;
            set
            {
                lockMode = value;
                if (toAdd.Count > 0)
                {
                    foreach (LuckyComponent component in toAdd)
                    {
                        if (!current.Contains(component))
                        {
                            current.Add(component);
                            components.Add(component);
                            component.Added(Entity);
                        }
                    }

                    adding.Clear();
                    toAdd.Clear();
                }

                if (toRemove.Count > 0)
                {
                    foreach (LuckyComponent component in toRemove)
                    {
                        if (current.Contains(component))
                        {
                            current.Remove(component);
                            components.Remove(component);
                            component.Removed();
                        }
                    }

                    removing.Clear();
                    toRemove.Clear();
                }
            }
        }

        public void Add(LuckyComponent component)
        {
            switch (lockMode)
            {
                case LockModes.Open:
                    if (!current.Contains(component))
                    {
                        current.Add(component);
                        components.Add(component);
                        component.Added(Entity);
                    }

                    break;
                case LockModes.Locked:
                    if (!current.Contains(component) && !adding.Contains(component))
                    {
                        adding.Add(component);
                        toAdd.Add(component);
                    }

                    break;
                case LockModes.Error:
                    throw new Exception("Cannot add or remove Entities at this time!");
                default:
                    return;
            }
        }

        public void Remove(LuckyComponent component)
        {
            switch (lockMode)
            {
                case LockModes.Open:
                    if (current.Contains(component))
                    {
                        current.Remove(component);
                        components.Remove(component);
                        component.Removed();
                    }

                    break;
                case LockModes.Locked:
                    if (current.Contains(component) && !removing.Contains(component))
                    {
                        removing.Add(component);
                        toRemove.Add(component);
                    }

                    break;
                case LockModes.Error:
                    throw new Exception("Cannot add or remove Entities at this time!");
                default:
                    return;
            }
        }

        public void Add(IEnumerable<LuckyComponent> components)
        {
            foreach (LuckyComponent component in components)
            {
                Add(component);
            }
        }

        public void Remove(IEnumerable<LuckyComponent> components)
        {
            foreach (LuckyComponent component in components)
            {
                Remove(component);
            }
        }

        public void RemoveAll<T>() where T : LuckyComponent
        {
            Remove(GetAll<T>());
        }

        public void Add(params LuckyComponent[] components)
        {
            foreach (LuckyComponent component in components)
            {
                Add(component);
            }
        }

        public void Remove(params LuckyComponent[] components)
        {
            foreach (LuckyComponent component in components)
            {
                Remove(component);
            }
        }

        public int Count
        {
            get { return components.Count; }
        }

        public LuckyComponent this[int index]
        {
            get
            {
                if (index < 0 || index >= components.Count)
                {
                    throw new IndexOutOfRangeException();
                }

                return components[index];
            }
        }

        public IEnumerator<LuckyComponent> GetEnumerator()
        {
            return components.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }


        public void Update()
        {
            LockMode = LockModes.Locked;
            foreach (LuckyComponent component in components)
            {
                if (component.Active)
                    component.Update();
            }

            LockMode = LockModes.Open;
        }

        public void FixedUpdate()
        {
            LockMode = LockModes.Locked;
            foreach (LuckyComponent component in components)
            {
                if (component.Active)
                    component.FixedUpdate();
            }

            LockMode = LockModes.Open;
        }

        public void Render()
        {
            LockMode = LockModes.Error; // render的时候不要有添加删除的逻辑判断
            foreach (LuckyComponent component in components)
            {
                if (component.Visible)
                {
                    component.Render();
                }
            }

            LockMode = LockModes.Open;
        }


        public T Get<T>() where T : LuckyComponent
        {
            foreach (LuckyComponent component in components)
            {
                if (component is T)
                    return component as T;
            }

            return default;
        }

        public IEnumerable<T> GetAll<T>() where T : LuckyComponent
        {
            foreach (LuckyComponent component in components)
            {
                if (component is T)
                    yield return component as T;
            }
        }
    }
}