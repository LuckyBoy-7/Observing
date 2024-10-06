using System.Collections.Generic;
using Lucky.Extensions;
using Lucky.Framework.Particle;
using Lucky.Managers;
using Lucky.Utilities;
using UnityEngine;
using ParticleSystem = Lucky.Framework.Particle.ParticleSystem;
using static Lucky.Utilities.CameraUtils;

namespace Crumb
{
    public class CrumbManager : Singleton<CrumbManager>
    {
        private const float Width = 100;
        private const float Height = 50;
        private const int Depth = 20;
        public Quadtree<Crumb> Crumbs = new(0, 0, Width, Height, Depth);
        public int number = 10;

        protected override void ManagedUpdate()
        {
            base.ManagedUpdate();
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