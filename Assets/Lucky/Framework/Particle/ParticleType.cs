using System;
using Lucky.Utilities;
using UnityEngine;

namespace Lucky.Framework.Particle
{
    /// <summary>
    /// Particle的数据
    /// </summary>
    [Serializable]
    public class ParticleType
    {

        #region Life

        [Header("Life")] public float LifeMin;
        public float LifeMax;
        public bool Realtime;

        #endregion

        #region Texture

        [Header("Texture")] public Sprite Source; // 源图
        public Chooser<Sprite> SourceChooser; // 多个源图

        #endregion

        #region Color

        public enum ColorModes
        {
            Static, // 不做处理, 跟None差不多
            Choose, // 颜色从Color和Color2里面抽一个
            Blink, // 颜色在Color1和Color2之间交替
            Lerp // 颜色从1 lerp 到 2
        }

        public enum FadeModes
        {
            None, // 不渐变
            Linear, // 线性的
            Late, // 也是线性的, 就是更晚开始淡出
            InAndOut // in-out缓入缓出的
        }

        [Header("Color")] public Color Color; // 颜色1, 理论上没什么特殊情况的时候例子用的就是这个颜色
        public Color Color2; // 颜色2
        public ColorModes ColorMode; // 颜色模式
        public FadeModes FadeMode; // 淡入淡出模式

        #endregion

        #region Move

        [Header("Move")] public float SpeedMin; // 最小速度
        public float SpeedMax; // 最大速度
        [Tooltip("增加加速度的速率")]
        public float SpeedMultiplier; // 速度倍数
        public Vector2 Acceleration; // 加速度
        public float Friction; // 摩擦力
        public float Direction; // 方向
        public float DirectionRange; // 方向转动最大角度

        #endregion

        #region Scale

        [Header("Scale")] public float Size; // 大小
        public float SizeRange;  // 大小变化范围
        public bool ScaleOut;  // 死亡的时候刚好缩放为0, 即Scale "Out"


        #endregion

        #region Rotation

        public enum RotationModes
        {
            None, // 不旋转
            Random, // 随机的
            SameAsSpeedDirection // 面朝速度方向
        }

        [Header("Rotate")] public float SpinMin; // 旋转最小速度
        public float SpinMax; // 旋转最大速度
        public float SpinFriction; 
        public bool RandomFlipSpinDir; // 是否翻转旋转方向
        public RotationModes RotationMode; // 旋转模式

        #endregion

        public ParticleType()
        {
            Color = (Color2 = Color.white);
            ColorMode = ColorModes.Static;
            FadeMode = FadeModes.None;
            SpeedMin = (SpeedMax = 0f);
            SpeedMultiplier = 1f;
            Acceleration = Vector2.zero;
            Friction = 0f;
            Direction = (DirectionRange = 0f);
            LifeMin = (LifeMax = 0f);
            Size = 2f;
            SizeRange = 0f;
            SpinMin = (SpinMax = 0f);
            RandomFlipSpinDir = false;
            RotationMode = RotationModes.None;
        }

        public ParticleType(ParticleType copyFrom)
        {
            Source = copyFrom.Source;
            SourceChooser = copyFrom.SourceChooser;
            Color = copyFrom.Color;
            Color2 = copyFrom.Color2;
            ColorMode = copyFrom.ColorMode;
            FadeMode = copyFrom.FadeMode;
            SpeedMin = copyFrom.SpeedMin;
            SpeedMax = copyFrom.SpeedMax;
            SpeedMultiplier = copyFrom.SpeedMultiplier;
            Acceleration = copyFrom.Acceleration;
            Friction = copyFrom.Friction;
            Direction = copyFrom.Direction;
            DirectionRange = copyFrom.DirectionRange;
            LifeMin = copyFrom.LifeMin;
            LifeMax = copyFrom.LifeMax;
            Size = copyFrom.Size;
            SizeRange = copyFrom.SizeRange;
            RotationMode = copyFrom.RotationMode;
            SpinMin = copyFrom.SpinMin;
            SpinMax = copyFrom.SpinMax;
            SpinFriction = copyFrom.SpinFriction;
            RandomFlipSpinDir = copyFrom.RandomFlipSpinDir;
            ScaleOut = copyFrom.ScaleOut;
            Realtime = copyFrom.Realtime;
        }

        public Particle Apply(Particle particle, Vector2 position)
        {
            return Apply(particle, position, Direction);
        }

        public Particle Apply(Particle particle, Vector2 position, Color color)
        {
            return Apply(particle, null, position, Direction, color);
        }

        public Particle Apply(Particle particle, Vector2 position, float direction)
        {
            return Apply(particle, null, position, direction, Color);
        }

        public Particle Apply(Particle particle, Vector2 position, Color color, float direction)
        {
            return Apply(particle, null, position, direction, color);
        }

        public Particle Apply(Particle particle, ManagedBehaviour entity, Vector2 position, float direction, Color color)
        {
            particle.Track = entity; // particle track的entity, 用来定位位置
            particle.Type = this; // 给particle提供一个自身的引用
            particle.Position = position; // 相对位置, 没有entitiy就是绝对位置
            // 有chooser的话抽一个texture
            if (SourceChooser != null)
            {
                particle.Source = SourceChooser.Choose();
            }
            // 自己定义了Source就用自己的
            else if (Source != null)
            {
                particle.Source = Source;
            }
            // 都没有就用Draw的fallback
            else
            {
                if (Draw.Particle == null)
                    Debug.LogWarning("Draw.Particle is None");
                particle.Source = Draw.Particle;
            }


            // 有range的话就roll一个
            if (SizeRange != 0f)
                particle.StartSize = particle.Size = Size - SizeRange * 0.5f + RandomUtils.NextFloat(SizeRange);
            else
                particle.StartSize = particle.Size = Size;

            // color
            if (ColorMode == ColorModes.Choose)
                particle.StartColor = particle.Color = RandomUtils.Choose(color, Color2);
            else
                particle.StartColor = particle.Color = color;

            float radians = direction - DirectionRange * 0.5f + RandomUtils.NextFloat() * DirectionRange;
            particle.Speed = MathUtils.RadiansToVector(radians, RandomUtils.Range(SpeedMin, SpeedMax));
            particle.StartLife = particle.Life = RandomUtils.Range(LifeMin, LifeMax);

            // 自转
            particle.Rotation = RotationMode switch
            {
                RotationModes.Random => RandomUtils.NextRadians(),
                RotationModes.SameAsSpeedDirection => radians,
                _ => 0f
            };

            particle.Spin = RandomUtils.Range(SpinMin, SpinMax);
            if (RandomFlipSpinDir)
                particle.Spin *= RandomUtils.Choose(1, -1);
            particle.SpinFriction = SpinFriction;

            return particle;
        }
    }
}