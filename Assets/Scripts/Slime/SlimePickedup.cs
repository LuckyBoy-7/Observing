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
            // intentionAnim.Play("Empty");
            rb.velocity = Vector2.zero;
            collider.enabled = false;
            intentionAnim.Play("Empty");
            intentionSr.sprite = null;
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