using System;
using System.Collections.Generic;
using Lucky.Extensions;
using Lucky.Framework;
using Lucky.Framework.Particle;
using Lucky.Utilities;
using UnityEngine;

namespace Crumb
{
    // todo: 重生到中间的bug
    public class Crumb : Particle
    {
        public List<Sprite> bigSprites = new();
        public List<Sprite> middleSprites = new();
        public List<Sprite> smallSprites = new();
        private Sprite bigSprite;
        private Sprite middleSprite;
        private Sprite smallSprite;
        public float TotalAmount = 100;
        private float curAmount;
        private bool hasAdded = false;

        private void Awake()
        {
            ResetData();
        }

        public override Particle ResetData()
        {
            base.ResetData();
            bigSprite = bigSprites.Choice();
            middleSprite = middleSprites.Choice();
            smallSprite = smallSprites.Choice();
            curAmount = TotalAmount;
            hasAdded = false;
            return this;
        }

        protected override void ManagedFixedUpdate()
        {
            base.ManagedFixedUpdate();
            if (Speed == Vector2.zero && !hasAdded)
            {
                if (!CameraUtils.InBounds(transform.position))
                {
                    Kill();
                    return;
                }

                hasAdded = true;
                CrumbManager.Instance.Crumbs.Add(this, transform.position);
            }
        }

        protected override void Kill()
        {
            base.Kill();
            CrumbManager.Instance.Crumbs.Remove(this, transform.position);
        }

        public override void Render()
        {
            base.Render();
            if (curAmount > 50)
                sr.sprite = bigSprite;
            else if (curAmount > 30)
                sr.sprite = middleSprite;
            else
                sr.sprite = smallSprite;
            transform.localScale = Vector3.one * MathUtils.Min(1, curAmount / TotalAmount + 0.3f);
        }

        public float TakeEnergy(float want)
        {
            if (want < curAmount)
            {
                curAmount -= want;
                return want;
            }

            Kill();
            return curAmount;
        }

    }
}