using System;
using System.Collections.Generic;
using System.Reflection;
using Lucky.Managers;
using UnityEngine;
using UnityEngine.Pool;

namespace Lucky.Managers.ObjectPool_
{
    public class ObjectPoolManager : Singleton<ObjectPoolManager>
    {
        private Dictionary<Type, ObjectPool<IRecycle>> typeToRecycles = new();
        private const string Path = "IRecycle/";


        public T Get<T>() where T : MonoBehaviour, IRecycle
        {
            if (!typeToRecycles.ContainsKey(typeof(T)))
                typeToRecycles[typeof(T)] = new(() => Instantiate(Resources.Load<T>(Path + typeof(T).Name)));


            IRecycle retval = typeToRecycles[typeof(T)].Get();
            retval.OnGet();
            return (T)retval;
        }

        public void Release<T>(T value) where T : IRecycle
        {
            typeToRecycles[typeof(T)].Release(value);
            value.OnRelease();
        }
    }
}