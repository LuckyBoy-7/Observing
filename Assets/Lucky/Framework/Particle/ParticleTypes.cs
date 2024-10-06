using Lucky.Utilities;
using UnityEngine;

namespace Lucky.Framework.Particle
{
    public class ParticleTypes
    {
        public static ParticleType Template;
        public static ParticleType Crumb;

        public static void Initialize()
        {
            Template = new ParticleType
            {
                LifeMin = 0.4f,
                LifeMax = 0.6f,
                Size = 1f,
                SizeRange = 0f,
                DirectionRange = 6.2831855f,
                SpeedMin = 4f,
                SpeedMax = 8f,
                FadeMode = ParticleType.FadeModes.Late,
                Color = ColorUtils.HexToColor("a5fff7"),
                Color2 = ColorUtils.HexToColor("6de081"),
                ColorMode = ParticleType.ColorModes.Blink
            };

            Crumb = new ParticleType
            {
                LifeMin = 100000,
                LifeMax = 100000,
                Size = 1f,
                Direction = 2.39f,
                DirectionRange = 1.21f,
                SpeedMin = 50,
                SpeedMax = 140,
                FadeMode = ParticleType.FadeModes.None,
                Color = Color.white,
                ColorMode = ParticleType.ColorModes.Static,
                SpeedMultiplier = 0.45f,
                Acceleration = new Vector2(-7.8f, 1.4f),
                Friction = 85,
                SpinMin = 0.3f,
                SpinMax = 1f,
                SpinFriction = 0.6f,
                RotationMode = ParticleType.RotationModes.Random
            };
        }
    }
}