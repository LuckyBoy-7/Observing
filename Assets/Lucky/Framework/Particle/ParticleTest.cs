using System;
using Lucky.Extensions;
using Lucky.Utilities;
using UnityEngine;
using Input = Lucky.Inputs.Input;

namespace Lucky.Framework.Particle
{
    public class ParticleTest : ManagedBehaviour
    {
        public ParticleType particle;

        public int amount = 15;
        [Range(0, 60)] public float rangeX = 20;
        [Range(0, 60)] public float rangeY = 20;

        private void Awake()
        {
            particle = new ParticleType(ParticleTypes.Crumb);
        }

        protected override void ManagedUpdate()
        {
            base.ManagedUpdate();
            if (Input.GetKeyDown(KeyCode.Return))
            {
                ParticleSystem.Instance.Emit<Particle>(particle, this, amount, Vector2.zero, new Vector2(rangeX, rangeY));
            }
        }

        private void OnDrawGizmos()
        {
            // 生成区域
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(transform.position, new Vector3(rangeX, rangeY) * 2);
            // 运动方向
            Gizmos.color = Color.red;
            float length = 8;
            Vector2 startDir = MathUtils.RadiansToVector(particle.Direction - particle.DirectionRange / 2, length);
            int resolution = 18;
            for (int i = 0; i < resolution; i++)
            {
                Gizmos.DrawRay(transform.position, startDir.Rotate(particle.DirectionRange / resolution * i));
            }
        }
    }
}