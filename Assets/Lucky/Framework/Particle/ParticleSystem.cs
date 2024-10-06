using System;
using System.Collections.Generic;
using Lucky.Collections;
using Lucky.Extensions;
using Lucky.Managers;
using Lucky.Utilities;
using UnityEngine;

namespace Lucky.Framework.Particle
{
    /// <summary>
    /// 对particle进行统一管理, 操控他们的create update render
    /// </summary>
    public class ParticleSystem : Singleton<ParticleSystem>
    {
        private DefaultDict<Type, List<Particle>> particles = new(() => new());

        public void ReleaseParticle<T>(T particle) where T : Particle
        {
            // 如果使用gettype拿到的是子类的类型, 如果直接typeof(particle), 那么在不指定T, c#自动识别的情况下会当作Particle
            particles[particle.GetType()].Add(particle);
        }

        public void CheckAndMakeSure<T>() where T : Particle
        {
            if (particles[typeof(T)].Count == 0)
            {
                T particle = Instantiate(Resources.Load<T>($"Particles/{typeof(T).Name}"));
                particles[typeof(T)].Add(particle);
            }
        }

        /// <summary>
        /// 把type的数据传到particle里
        /// </summary>
        public void Emit<T>(ParticleType type, Vector2 position) where T : Particle
        {
            CheckAndMakeSure<T>();
            type.Apply(particles[typeof(T)].Pop().ResetData(), position);
        }

        public void Emit<T>(ParticleType type, Vector2 position, float direction) where T : Particle
        {
            CheckAndMakeSure<T>();
            type.Apply(particles[typeof(T)].Pop().ResetData(), position, direction);
        }

        public void Emit<T>(ParticleType type, Vector2 position, Color color) where T : Particle
        {
            CheckAndMakeSure<T>();
            type.Apply(particles[typeof(T)].Pop().ResetData(), position, color);
        }

        public void Emit<T>(ParticleType type, Vector2 position, Color color, float direction) where T : Particle
        {
            CheckAndMakeSure<T>();
            type.Apply(particles[typeof(T)].Pop().ResetData(), position, color, direction);
        }

        public void Emit<T>(ParticleType type, int amount, Vector2 position, Vector2 positionRange) where T : Particle
        {
            for (int i = 0; i < amount; i++)
            {
                Emit<T>(type, RandomUtils.Range(position - positionRange, position + positionRange));
            }
        }

        public void Emit<T>(ParticleType type, int amount, Vector2 position, Vector2 positionRange, float direction) where T : Particle
        {
            for (int i = 0; i < amount; i++)
            {
                Emit<T>(type, RandomUtils.Range(position - positionRange, position + positionRange), direction);
            }
        }

        public void Emit<T>(ParticleType type, int amount, Vector2 position, Vector2 positionRange, Color color) where T : Particle
        {
            for (int i = 0; i < amount; i++)
            {
                Emit<T>(type, RandomUtils.Range(position - positionRange, position + positionRange), color);
            }
        }

        public void Emit<T>(ParticleType type, int amount, Vector2 position, Vector2 positionRange, Color color, float direction) where T : Particle
        {
            for (int i = 0; i < amount; i++)
            {
                Emit<T>(type, RandomUtils.Range(position - positionRange, position + positionRange), color, direction);
            }
        }

        public void Emit<T>(ParticleType type, ManagedBehaviour track, int amount, Vector2 position, Vector2 positionRange) where T : Particle
        {
            for (int i = 0; i < amount; i++)
            {
                CheckAndMakeSure<T>();
                type.Apply(
                    particles[typeof(T)].Pop().ResetData(), track, RandomUtils.Range(position - positionRange, position + positionRange), type.Direction,
                    type.Color
                );
            }
        }

        public void Emit<T>(ParticleType type, ManagedBehaviour track, int amount, Vector2 position, Vector2 positionRange, float direction) where T : Particle
        {
            for (int i = 0; i < amount; i++)
            {
                CheckAndMakeSure<T>();
                type.Apply(
                    particles[typeof(T)].Pop().ResetData(), track, RandomUtils.Range(position - positionRange, position + positionRange), direction, type.Color
                );
            }
        }
    }
}