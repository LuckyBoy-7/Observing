using UnityEngine;

namespace Lucky.Extensions
{
    public static class TransformExtensions
    {
        public static void SetPositionX(this Transform orig, float x) => orig.position = orig.position.WithX(x);
        public static void SetPositionY(this Transform orig, float y) => orig.position = orig.position.WithY(y);
        public static void SetPositionZ(this Transform orig, float z) => orig.position = orig.position.WithZ(z);
        
        public static void AddPositionX(this Transform orig, float x) => orig.position = orig.position.WithX(orig.position.x + x);
        public static void AddPositionY(this Transform orig, float y) => orig.position = orig.position.WithY(orig.position.y + y);
        public static void AddPositionZ(this Transform orig, float z) => orig.position = orig.position.WithZ(orig.position.z + z);
        
        // 2d eulerAngles就够了
        public static void SetRotationX(this Transform orig, float x) => orig.eulerAngles = orig.eulerAngles.WithX(x);
        public static void SetRotationY(this Transform orig, float y) => orig.eulerAngles = orig.eulerAngles.WithY(y);
        public static void SetRotationZ(this Transform orig, float z) => orig.eulerAngles = orig.eulerAngles.WithZ(z);
        
        public static void SetScaleX(this Transform orig, float x) => orig.localScale = orig.localScale.WithX(x);
        public static void SetScaleY(this Transform orig, float y) => orig.localScale = orig.localScale.WithY(y);
        public static void SetScaleZ(this Transform orig, float z) => orig.localScale = orig.localScale.WithZ(z);
    }
}