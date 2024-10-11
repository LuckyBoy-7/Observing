using System;
using Lucky.Framework.Extensions;
using Lucky.Framework.Utilities;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Slime
{
    public partial class Slime
    {
        public const float PickedupTime = 4;

        private void PickedupBegin()
        {
            rb.velocity = Vector2.zero;
            collider.enabled = false;
            
            anim.Play("shock");
            anim.Speed = 1f;
            intentionAnim.PlayEmpty();
        }

        private void PickedupEnd()
        {
            collider.enabled = true;
        }

        private int PickedupUpdate()
        {
            return StPickedup;
        }
    }
}