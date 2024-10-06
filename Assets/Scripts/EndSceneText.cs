using System;
using System.Collections;
using System.Collections.Generic;
using Lucky.Extensions;
using Lucky.Framework;
using Lucky.Utilities;
using TMPro;
using UnityEngine;
using Coroutine = Lucky.Framework.Coroutine;

public class EndSceneText : ManagedBehaviour
{
    public float FadeOutTime = 3;
    private float fadeOutElapse = 0;
    private TMP_Text text;
    private const float musicFadeOutTime = 3;
    private float musicFadeTimer = musicFadeOutTime;
    public AudioSource audio;
    private bool isStartEnd;

    private void Awake()
    {
        text = GetComponent<TMP_Text>();
    }

    public void GameOver()
    {
        Add(new Coroutine(End()));
    }

    protected override void ManagedUpdate()
    {
        base.ManagedUpdate();
        if (isStartEnd)
        {
            float v = Mathf.Sin(Timer.GetTime());
            transform.eulerAngles = new Vector3(0, 0, v);
        }
    }


    private IEnumerator End()
    {
        isStartEnd = true;
        // 文字淡出
        while (fadeOutElapse < FadeOutTime)
        {
            fadeOutElapse += Timer.FixedDeltaTime();
            text.color = text.color.WithA(fadeOutElapse / FadeOutTime);
            yield return null;
        }

        yield return 1;
        while (musicFadeTimer > 0)
        {
            musicFadeTimer -= Timer.FixedDeltaTime();
            audio.volume = Ease.CubicEaseInOut(musicFadeTimer / musicFadeOutTime);
            yield return null;
        }

        yield return 1;

        print("Game True Over");
        Application.Quit();
    }
}