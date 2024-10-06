using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class AudioParams
{
    [System.Serializable]
    public class Pitch
    {
        public enum Variation
        {
            Small,
            Medium,
            Large,
            VerySmall,
        }

        /// 音高，相当于频率
        public float pitch;

        /// 直接设置
        public Pitch(float exact)
        {
            pitch = exact;
        }

        /// 两数之间抽一个
        public Pitch(float minRandom, float maxRandom)
        {
            pitch = Random.Range(minRandom, maxRandom);
        }

        /// 选一个预定义的enum
        public Pitch(Variation randomVariation)
        {
            switch (randomVariation)
            {
                case Variation.VerySmall:
                    pitch = Random.Range(0.95f, 1.05f);
                    break;
                case Variation.Small:
                    pitch = Random.Range(0.9f, 1.1f);
                    break;
                case Variation.Medium:
                    pitch = Random.Range(0.75f, 1.25f);
                    break;
                case Variation.Large:
                    pitch = Random.Range(0.5f, 1.5f);
                    break;
            }
        }
    }

    [System.Serializable]
    public class Repetition
    {
        public float maxRepetitionFrequency;
        public string entryId;

        public Repetition(float maxRepetitionFrequency, string entryId = "")
        {
            this.maxRepetitionFrequency = maxRepetitionFrequency;
            this.entryId = entryId;
        }
    }

    [System.Serializable]
    public class Randomization
    {
        public bool isNoRepeating;

        public Randomization(bool isNoRepeating = true)
        {
            this.isNoRepeating = isNoRepeating;
        }
    }

    [System.Serializable]
    public class Distortion
    {
        /// 沉闷的 
        public bool isMuffled;
    }
}