using System.Collections.Generic;
using Lucky.Extensions;
using UnityEngine;

namespace Lucky.Utilities
{
    public static class ColorUtils
    {
        private static string pattern = "0123456789ABCDEF";

        /// <summary>
        /// "所有"颜色, 主要用作调试
        /// </summary>
        private static List<Color> allColors = new List<Color>()
        {
            Color.black,
            Color.blue,
            Color.cyan,
            Color.gray,
            Color.green,
            Color.magenta,
            Color.red,
            Color.white,
            Color.yellow,
        };


        /// <summary>
        /// 返回一个[0, 255]整形对应的16进制字符串
        /// </summary>
        /// <param name="x">[0, 255]</param>
        /// <returns></returns>
        private static string ToHex(int x)
        {
            x = Mathf.Clamp(x, 0, 255);
            return $"{pattern[x / 16]}{pattern[x % 16]}";
        }

        public static string Wrap(string orig, string surround) => $"<color={surround}>{orig}</color>";

        public static string Wrap(string orig, Color color)
        {
            string r = ToHex((int)(color.r * 255));
            string g = ToHex((int)(color.g * 255));
            string b = ToHex((int)(color.b * 255));
            string a = ToHex((int)(color.a * 255));
            return $"<color=#{r}{g}{b}{a}>{orig}</color>";
        }

        public static Color GetRandomColor() => allColors.Choice();

        public static Color GetRandomLerpColor(int number = 2)
        {
            Color color = Color.black;
            for (int i = 0; i < number; i++)
            {
                Color c = allColors.Choice();
                color += c / number;
            }

            return color.WithA(1);
        }

        /// <summary>
        /// 将传入的16进制字符转化为十进制
        /// </summary>
        public static byte HexToByte(char c)
        {
            return (byte)"0123456789ABCDEF".IndexOf(char.ToUpper(c));
        }

        public static Color HexToColor(string hex)
        {
            int num = 0;
            // #可省选项
            if (hex.Length >= 1 && hex[0] == '#')
            {
                num = 1;
            }

            // 剩下数字长度够6的话就parse返回
            if (hex.Length - num >= 6)
            {
                // [0, 1]
                float num2 = (HexToByte(hex[num]) * 16 + HexToByte(hex[num + 1])) / 255f;
                float num3 = (HexToByte(hex[num + 2]) * 16 + HexToByte(hex[num + 3])) / 255f;
                float num4 = (HexToByte(hex[num + 4]) * 16 + HexToByte(hex[num + 5])) / 255f;
                return new Color(num2, num3, num4);
            }

            // 字符串长度不够的话，就把它当作一个数字来解析
            int num5;
            if (int.TryParse(hex.Substring(num), out num5))
            {
                return HexToColor(num5);
            }

            // 还不行就fallback
            return Color.white;
        }

        public static Color HexToColor(int hex)
        {
            return new Color
            {
                a = byte.MaxValue,
                r = (byte)(hex >> 16),
                g = (byte)(hex >> 8),
                b = (byte)hex
            };
        }

    }
}