using System.Collections.Generic;
using Lucky.Framework.Collections;
using Lucky.Framework.Extensions;
using Lucky.Framework.Particle;
using Lucky.Framework.Managers;
using Lucky.Framework.Utilities;
using UnityEngine;
using ParticleSystem = Lucky.Framework.Particle.ParticleSystem;
using static Lucky.Framework.Utilities.CameraUtils;
using Input = Lucky.Framework.Inputs.Input;

namespace Crumb
{
    public class CrumbManager : Singleton<CrumbManager>
    {
        private const float Width = 100;
        private const float Height = 50;
        private const int Depth = 20;
        public QuadTree<Crumb> Crumbs = new(0, 0, Width, Height, Depth);
        public int number = 10;

        protected override void ManagedFixedUpdate()
        {
            base.ManagedFixedUpdate();
            if (Input.GetMouseButtonDown(0))
            {
                Vector2 bottomRight = new Vector2(HalfWidth, -HalfHeight);
                ThrowCumbs(number, bottomRight, HalfHeight * 1.5f);
            }
        }

        public void ThrowCumbs(int amount, Vector2 position, float rangeX)
        {
            // ParticleSystem.Instance.Emit<Crumb>(ParticleTypes.Crumb, amount, position, position.WithX(position.x - RandomUtils.NextFloat(rangeX) / 2));
            position.y -= 5;
            ParticleSystem.Instance.Emit<Crumb>(
                ParticleTypes.Crumb, amount, position, new Vector2(rangeX / 2, 1)
            );
        }
    }
}