using System;
using System.Collections.Generic;
using Lucky.Extensions;
using Lucky.Managers;
using Lucky.Managers.ObjectPool_;
using Lucky.Utilities;
using UnityEngine;

namespace Slime
{
    public class SlimeSpawner : Singleton<SlimeSpawner>
    {
        public int initialSlimeNumber = 20;

        private List<Color> colors = new List<Color>()
        {
            Color.red,
            Color.green,
            Color.blue,
            Color.cyan,
            Color.yellow,
            Color.magenta,
            Color.white,
            Color.gray,
            new Color(0.9f, 0.6f, 0)
        };

        public HashSet<Slime> Slimes = new();
        private HashSet<Slime> wantLoveSlimes = new();
        public Magnifier Magnifier;

        private void Awake()
        {
            for (int i = 0; i < initialSlimeNumber; i++)
            {
                Vector2 randomPos = RandomUtils.InsideUnitCircle * Camera.main.orthographicSize * 2;
                SpawnSlime(randomPos, colors.Choice());
            }
        }

        protected override void ManagedFixedUpdate()
        {
            base.ManagedFixedUpdate();

            if (Slimes.Count < 4)
            {
                Vector2 randomPos = RandomUtils.InsideUnitCircle * Camera.main.orthographicSize * 2;
                while (Magnifier.Collider.OverlapPoint(randomPos))
                {
                    randomPos = RandomUtils.InsideUnitCircle * Camera.main.orthographicSize * 2;
                }

                SpawnSlime(randomPos, colors.Choice());
            }
        }

        public void SpawnSlime(Vector2 pos, Color color)
        {
            Slime slime = ObjectPoolManager.Instance.Get<Slime>();
            slime.transform.position = pos;
            slime.Color = color;
        }

        public Slime GetSlimeWantLoveInDist(Slime from, float dist)
        {
            foreach (var slime in wantLoveSlimes)
            {
                if (slime == from)
                    continue;
                if (slime.StateMachine.State == Slime.StRun)
                {
                    if (from.Dist(slime) <= dist)
                        return slime;
                }
            }

            return null;
        }

        public void TryAddWantLove(Slime slime)
        {
            wantLoveSlimes.Add(slime);
        }

        public void TryRemoveWantLove(Slime slime)
        {
            wantLoveSlimes.Remove(slime);
        }
    }
}