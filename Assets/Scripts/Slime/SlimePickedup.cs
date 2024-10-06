using System;
using Lucky.Extensions;
using Lucky.Utilities;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Slime
{
    public partial class Slime
    {
        public const float PickedupTime = 4;

        private void PickedupBegin()
        {
            anim.Play("Shock");
            anim.speed = 1f;
            intentionSr.enabled = false;
            // intentionAnim.Play("Empty");
            rb.velocity = Vector2.zero;
            collider.enabled = false;
        }

        private void PickedupEnd()
        {
            intentionSr.enabled = true;
            collider.enabled = true;
        }

        private int PickedupUpdate()
        {
            return StPickedup;
        }
    }
}