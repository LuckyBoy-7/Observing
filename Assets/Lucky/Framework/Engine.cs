using System;
using Lucky.Extensions;
using Lucky.Framework.Particle;
using Lucky.Inputs;
using Lucky.Interactive;
using Lucky.Managers;
using Lucky.Managers.ObjectPool_;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using Input = Lucky.Inputs.Input;
using ParticleSystem = Lucky.Framework.Particle.ParticleSystem;

namespace Lucky.Framework
{
    /// <summary>
    /// 在设置界面保证Engine最先调用, 然后Engine去初始化各种Manager, 以保证更新顺序正确
    /// </summary>
    public class Engine : MonoBehaviour
    {
        protected virtual void Awake()
        {
            DontDestroyOnLoad(gameObject);
            Time.fixedDeltaTime = 1 / 60f;
            Settings.Initialize();
            Input.Initialize();
            ParticleTypes.Initialize();
            this.AddComponent<ObjectPoolManager>();
            this.AddComponent<GameCursor>();
            this.AddComponent<EventManager>();
            this.AddComponent<ParticleSystem>();
            Draw.Initialize();  // 为了适配static不clear
        }

        protected virtual void Update()
        {
            // Input有最高优先级
            Input.Update();
        }

        protected void FixedUpdate()
        {
            Input.FixedUpdate();
            // 清空绘图
            Draw.DrawBegin();
        }

        private void LateUpdate()
        {
            #if UNITY_EDITOR
            if (Input.GetKey(KeyCode.LeftControl))
            {
                if (Input.GetKeyDown(KeyCode.Q))
                    Time.timeScale /= 2;
                else if (Input.GetKeyDown(KeyCode.E))
                    Time.timeScale *= 2;
            }
            #endif
        }
    }
}