using System;
using System.Collections;
using System.Collections.Generic;
using Lucky.Framework.Extensions;
using Lucky.Framework.Managers;
using Lucky.Framework.Utilities;
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


        private List<float> timeLine = new List<float>()
        {
            Period1,
            Period2,
            Period3,
            Period4,
            Period5,
            Period6,
        };

        private SerialSegments<float> temperature = new SerialSegments<float>
        (
            new List<float>() { -82, 0, 100, 0, 45, -50 },
            MathUtils.Lerp
        );

        private float Temperature => temperature.GetByExistTimes(timeLine, curTime);

        private SerialSegments<Color> hue = new SerialSegments<Color>
        (
            new List<Color>()
            {
                new Color(255, 144, 121) / 255,
                new Color(31, 60, 180) / 255,
                new Color(255, 43, 0) / 255,
                new Color(31, 60, 180) / 255,
                new Color(1, 0.4f, 0),
                Color.black
            },
            (c1, c2, k) => Color.Lerp(c1, c2, k).WithA(1)
        );

        private Color Hue => hue.GetByExistTimes(timeLine, curTime);

        private SerialSegments<float> brightness = new SerialSegments<float>
        (
            new List<float>() { 1, 1.5f, 1.14f, 1.5f, 0.25f, -4.2f },
            MathUtils.Lerp
        );

        private float Brightness => brightness.GetByExistTimes(timeLine, curTime);

        private SerialSegments<float> saturation = new SerialSegments<float>
        (
            new List<float>() { 50, 0, 25, 0, 100, -8 },
            MathUtils.Lerp
        );

        private float Saturation => saturation.GetByExistTimes(timeLine, curTime);


        protected override void ManagedFixedUpdate()
        {
            base.ManagedFixedUpdate();

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
            // 一开始淡入
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
            grading.colorFilter.value = Hue; // 居然忘加了, 难受
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