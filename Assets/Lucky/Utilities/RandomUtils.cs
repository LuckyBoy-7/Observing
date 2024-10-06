using UnityEngine;
using Random = System.Random;

namespace Lucky.Utilities
{
    public static class RandomUtils
    {
        public static readonly Random Random = new();

        /// <summary>
        /// 生成单位圆内均匀分布的随机点
        /// </summary>
        public static Vector2 InsideUnitCircle
        {
            get
            {
                // 拒绝采样
                float x, y;
                do
                {
                    x = NextFloat();
                    y = NextFloat();
                } while (x * x + y * y > 1);

                return new Vector2(x - 0.5f, y - 0.5f);
            }
        }

        /// <summary>
        /// 随机一个[0, 1)的float
        /// </summary>
        public static float NextFloat()
        {
            return (float)Random.NextDouble();
        }

        /// <summary>
        /// 随机一个[0, 1) * val的float
        /// </summary>
        public static float NextFloat(float max)
        {
            return NextFloat() * max;
        }

        public static float NextFloat(int max) => NextFloat((float)max);

        /// <summary>
        /// 随机一个[0 ~ 2pi(])的radians
        /// </summary>
        public static float NextRadians()
        {
            return NextFloat() * Mathf.PI * 2;
        }

        /// <summary>
        /// 随机一个[0 ~ 2pi] * max的radians
        /// </summary>
        public static float NextRadians(float max)
        {
            return NextFloat() * Mathf.PI * 2 * max;
        }

        /// <summary>
        /// 随机一个[0, max)的int
        /// </summary>
        public static int Range(int max) => Range(0, max);

        /// <summary>
        /// 随机一个[min, max)的int
        /// </summary>
        public static int Range(int min, int max)
        {
            return min + Random.Next(max - min);
        }

        /// <summary>
        /// 随机一个[min, max)的float
        /// </summary>
        public static float Range(float min, float max)
        {
            return min + NextFloat(max - min);
        }

        /// <summary>
        /// 在传入的数组中抽一个值
        /// </summary>
        public static T Choose<T>(params T[] choices)
        {
            return choices[Random.Next(choices.Length)];
        }

        /// <summary>
        /// 随机一个[min Vector2, max Vector2)之间的Vector2
        /// </summary>
        public static Vector2 Range(Vector2 min, Vector2 max)
        {
            return min + new Vector2(NextFloat(max.x - min.x), NextFloat(max.y - min.y));
        }

        public static int Random01() => Choose(0, 1);

        public static Vector2 RandomPointInRect(Rect rect)
        {
            float x = Range(rect.x, rect.xMax);
            float y = Range(rect.y, rect.yMax);
            return new Vector2(x, y);
        }
    }
}