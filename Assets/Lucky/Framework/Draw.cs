using System.Collections.Generic;
using Lucky.Extensions;
using UnityEngine;
using static Lucky.Utilities.MathUtils;

namespace Lucky.Framework
{
    public class Draw
    {
        private static List<SpriteRenderer> Pool = new();
        private static int curIdx; // 指向第一个可使用的位置
        private static Sprite dotSprite;
        public static Sprite Particle;
        private static Vector3 GreatVector3 = new Vector3(1, 1) * int.MaxValue;


        // public static MTexture Particle;
        // public static MTexture Pixel;
        // private static Rectangle rect;


        internal static void Initialize()
        {
            dotSprite = Resources.Load<Sprite>("Primatives/Pixels/PixelDot");
            Particle = Resources.Load<Sprite>("Primatives/Pixels/Particle");
            Pool.Clear();
            curIdx = 0;
        }

        public static void CreateSpriteRenderer()
        {
            var sr = new GameObject($"draw unit: {Pool.Count}").AddComponent<SpriteRenderer>();
            Pool.Add(sr);
            sr.sprite = dotSprite;
            sr.transform.position = GreatVector3;
        }

        public static void DrawBegin()
        {
            while (curIdx > 0)
            {
                var sr = Pool[--curIdx];
                sr.transform.position = GreatVector3;
            }
        }

        private static SpriteRenderer GetSpriteRenderer()
        {
            while (curIdx >= Pool.Count)
                CreateSpriteRenderer();
            return Pool[curIdx++];
        }


        public static void Point(Vector2 at, Color color)
        {
            Debug.Log(at);
            at = at.Floor() + Vector2.one * 0.5f;
            var sr = GetSpriteRenderer();
            sr.transform.position = at;
            sr.transform.localScale = Vector3.one;
            sr.color = color;
            sr.transform.localRotation = Quaternion.identity;
        }

        public static void Line(Vector2 start, Vector2 end, Color color)
        {
            // 保证start在end左边
            if (start.x > end.x)
                (start, end) = (end, start);
            var (dx, dy) = end - start;

            if (dx == 0) // 竖线
            {
                int dir = Sign(dy);
                while (Sign(end.y - start.y) != -dir)
                {
                    Point(start, color);
                    start.y += dir;
                }

                return;
            }

            float k;
            // 斜线, 但是斜率低
            if (Abs(dy) < dx)
            {
                k = dy / dx;
                for (int i = 0; i <= (int)dx; i++)
                {
                    Point(start, color);
                    start += new Vector2(1, k);
                }

                return;
            }

            // 斜线, 但是斜率高
            k = dx / dy;
            int dirY = Sign(dy);
            for (int i = 0; i <= (int)Abs(dy); i++)
            {
                Point(start, color);
                start += new Vector2(dirY * k, dirY);
            }
        }

        public static void Line(float x1, float y1, float x2, float y2, Color color)
        {
            Line(new Vector2(x1, y1), new Vector2(x2, y2), color);
        }

        public static void LineAngle(Vector2 start, float angleRadians, float length, Color color)
        {
            Vector2 end = RadiansToVector(angleRadians, length);
            Line(start, end, color);
        }

        public static void LineAngle(float startX, float startY, float angle, float length, Color color)
        {
            LineAngle(new Vector2(startX, startY), angle, length, color);
        }

        public static void Circle(Vector2 position, float radius, Color color, int resolution = 7)
        {
            Vector2 start = Vector2.right * radius;
            Vector2 perpendicular = start.Perpendicular();
            for (int i = 1; i <= resolution; i++)
            {
                Vector2 start2 = RadiansToVector(i * PI(0.5f) / resolution, radius);
                Vector2 perpendicular2 = start2.Perpendicular();
                // 一口气四个象限都画一部分
                // 1
                Line(position + start, position + start2, color);
                // 3
                Line(position - start, position - start2, color);
                // 2
                Line(position + perpendicular, position + perpendicular2, color);
                // 4
                Line(position - perpendicular, position - perpendicular2, color);
                start = start2;
                perpendicular = perpendicular2;
            }
        }

        public static void Circle(float x, float y, float radius, Color color, int resolution)
        {
            Circle(new Vector2(x, y), radius, color, resolution);
        }

        /// <summary>
        /// 支点依然在左下角
        /// </summary>
        public static void Rect(float x, float y, float width, float height, Color color)
        {
            var sr = GetSpriteRenderer();
            sr.transform.position = new Vector3(x, y) + new Vector3(width, height) / 2;
            sr.color = color;
            sr.transform.localScale = new Vector3(width, height);
            sr.transform.localRotation = Quaternion.identity;
        }

        public static void Rect(Vector2 position, float width, float height, Color color)
        {
            Rect(position.x, position.y, width, height, color);
        }


        /// <summary>
        /// 支点依然在左下角
        /// </summary>
        public static void HollowRect(float x, float y, float width, float height, Color color)
        {
            Vector2 bottomLeft = new Vector2(x, y);
            Vector2 bottomRight = new Vector2(x + width - 1, y);
            Vector2 topLeft = new Vector2(x, y + height - 1);
            Rect(bottomLeft, width, 1, color);
            Rect(bottomLeft, 1, height, color);
            Rect(bottomRight, 1, height, color);
            Rect(topLeft, width, 1, color);
        }

        public static void HollowRect(Vector2 position, float width, float height, Color color)
        {
            HollowRect(position.x, position.y, width, height, color);
        }
    }
}