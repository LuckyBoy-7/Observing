using UnityEngine;

namespace Lucky.Framework.Tests
{
    public class DrawTests : ManagedBehaviour
    {
        public override void Render()
        {
            base.Render();
            
            Draw.Point(new Vector2(0, 0), Color.red);
            // Draw.Line(new Vector2(1, 1), new Vector2(1, 10), Color.red);
            // Draw.Line(new Vector2(1, 1), new Vector2(20, 8), Color.green);
            // Draw.Line(new Vector2(1, 1), new Vector2(-2, 8), Color.yellow);
            // Draw.Circle(Vector2.zero, 3f, Color.blue);
            // Draw.Circle(Vector2.zero, 10f, Color.blue);
            // Draw.Rect(0, 0, 10, 20, Color.white);
            Draw.HollowRect(0, 0, 10, 20, Color.white);
        }
    }
}