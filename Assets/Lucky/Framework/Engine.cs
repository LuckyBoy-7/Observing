/*
我突然意识到想根据unity的cc框架写个ec框架似乎是不可能的, 我真的没办法干预unity的生命周期啊, 但我又想像蔚蓝那样做到精确完美
自己强行弄个Engine和添加LuckyComponent之类的只让我感到臃肿和难受(虽然功能勉强实现了), 而且到了后面肯定越来越难维护

也是勉强在Input里让FixedUpdate也能精确获取输入了, 感觉以后直接不写Update, 全写FixedUpdate里好了
 */

using System;
using Lucky.Framework.Extensions;
using Lucky.Framework.Particle;
using Lucky.Framework.Inputs;
using Lucky.Framework.Interactive;
using Lucky.Framework.Managers;
using Lucky.Framework.Managers.ObjectPool_;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using Input = Lucky.Framework.Inputs.Input;
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
            ParticleSystem.Initialize();
            ParticleTypes.Initialize();
            this.AddComponent<ObjectPoolManager>();
            this.AddComponent<GameCursor>();
            this.AddComponent<EventManager>();
            this.AddComponent<ParticleSystem>();
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