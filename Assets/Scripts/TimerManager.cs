using System;
using System.Collections;
using System.Collections.Generic;
using Lucky.Extensions;
using Lucky.Managers;
using Lucky.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;
using Coroutine = Lucky.Framework.Coroutine;

namespace DefaultNamespace
{
    public class TimerManager : Singleton<TimerManager>
    {
        public TMP_Text Text;
        public float TimeMultiplier = 2.5f;
        private float curTime;

        // 7:00
        public const float GameStartTime = 420;

        // 7:00 -> 22:00
        public const float GameDuration = 900;

        public PostProcessVolume volume;
        private const float Period1 = 0; // 7:00  色调偏冷, 光线偏暗(微暗)
        private const float Period2 = 300; // 12:00 色调正常, 光线正常
        private const float Period3 = 420; // 14:00  色调偏暖(热的感觉), 光线偏亮
        private const float Period4 = 540; // 16:00 色调正常, 光线正常
        private const float Period5 = 660; // 18:00 黄昏, 色调偏暖, 光线偏暗 
        private const float Period6 = 900; // 22:00, 色调偏冷, 光线偏暗

        private const float EndTime = 850;
        // private const float EndTime = 100;

        private const float StartTime = 10;

        public Image panel;
        private bool isStartEnd;

        public EndSceneText EndSceneText;


        private float Temperature
        {
            get
            {
                List<float> periods = new List<float>
                {
                    Period1, Period2, Period3, Period4, Period5, Period6
                };
                List<float> values = new()
                {
                    -82, // 1
                    0, // 2
                    100, // 3
                    0, // 4
                    45, // 5
                    -50 // 6
                };
                for (var i = 0; i < periods.Count - 1; i++)
                {
                    if (curTime < periods[i + 1] && curTime > periods[i])
                    {
                        return MathUtils.Lerp(values[i], values[i + 1], (curTime - periods[i]) / (periods[i + 1] - curTime));
                    }
                }

                return -1;
            }
        }

        private Color Hue
        {
            get
            {
                List<float> periods = new List<float>
                {
                    Period1, Period2, Period3, Period4, Period5, Period6
                };
                List<Color> colors = new()
                {
                    new Color(255, 144, 121) / 255, // 1
                    new Color(31, 60, 180) / 255, // 2
                    new Color(255, 43, 0) / 255, // 3
                    new Color(31, 60, 180) / 255, // 4
                    new Color(1, 0.4f, 0), // 5
                    Color.black // 6
                };
                for (var i = 0; i < periods.Count - 1; i++)
                {
                    if (curTime < periods[i + 1] && curTime > periods[i])
                    {
                        return Color.Lerp(colors[i], colors[i + 1], (curTime - periods[i]) / (periods[i + 1] - curTime)).WithA(1);
                    }
                }

                return Color.black;
            }
        }

        private float Brightness
        {
            get
            {
                List<float> periods = new List<float>
                {
                    Period1, Period2, Period3, Period4, Period5, Period6
                };
                List<float> values = new()
                {
                    1, // 1
                    1.5f, // 2
                    1.14f, // 3
                    1.5f, // 4
                    0.25f, // 5
                    -4.2f //6                    
                };
                for (var i = 0; i < periods.Count - 1; i++)
                {
                    if (curTime < periods[i + 1] && curTime > periods[i])
                    {
                        return MathUtils.Lerp(values[i], values[i + 1], (curTime - periods[i]) / (periods[i + 1] - curTime));
                    }
                }

                return -1;
            }
        }

        private float Saturation
        {
            get
            {
                List<float> periods = new List<float>
                {
                    Period1, Period2, Period3, Period4, Period5, Period6
                };
                List<float> values = new()
                {
                    50, // 1
                    0, // 2
                    25, // 3
                    0, // 4
                    100, // 5
                    -8 // 6
                };
                for (var i = 0; i < periods.Count - 1; i++)
                {
                    if (curTime < periods[i + 1] && curTime > periods[i])
                    {
                        return MathUtils.Lerp(values[i], values[i + 1], (curTime - periods[i]) / (periods[i + 1] - curTime));
                    }
                }

                return -1;
            }
        }



        protected override void ManagedUpdate()
        {
            base.ManagedUpdate();

            // time
            curTime += Timer.DeltaTime() * TimeMultiplier;

            var (hour, minute) = MathUtils.Divmod(GameStartTime + curTime, 60);
            hour = (int)hour;
            minute = (int)minute;
            Text.text = $"{hour.ToString().PadLeft(2, '0')}:00";
            if (curTime > EndTime && !isStartEnd)
            {
                isStartEnd = true;
                Add(new Coroutine(EndGameCoroutine()));
            }

            if (isStartEnd)
                return;
            float alpha = 1 - MathUtils.Min(1, curTime / StartTime);
            panel.color = panel.color.WithA(alpha);

            // 模拟微微眨眼的效果
            Vignette vignette = volume.profile.GetSetting<Vignette>();
            float v = 0.7f + MathUtils.Sin(Timer.GetTime()) * 0.02f;
            vignette.intensity.value = v;

            // 色温
            ColorGrading grading = volume.profile.GetSetting<ColorGrading>();
            grading.postExposure.value = Brightness;
            grading.saturation.value = Saturation;
            grading.temperature.value = Temperature;
        }

        IEnumerator EndGameCoroutine()
        {
            while (true)
            {
                float duration = 2;
                float elapse = curTime - EndTime;
                panel.color = panel.color.WithA(elapse / duration);
                if (elapse > duration)
                {
                    print("GameOver");
                    EndSceneText.GameOver();
                    yield break;
                }

                yield return null;
            }
        }
    }
}