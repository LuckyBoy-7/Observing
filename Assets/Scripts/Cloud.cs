using System;
using System.Collections;
using System.Collections.Generic;
using Lucky.Extensions;
using Lucky.Utilities;
using UnityEngine;

public class Cloud : MonoBehaviour
{
    public float MinScale;
    public float MaxScale;
    public float MinMoveSpeed;
    public float MaxMoveSpeed;
    public List<Sprite> sprites;
    private SpriteRenderer sr;
    private float speed;
    private float elapse;
    public Vector2 Dir;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();

        sr.sprite = sprites.Choice();
        float k = RandomUtils.NextFloat();
        float scale = MinScale + (MaxScale - MinScale) * k;
        transform.localScale = Vector3.one * scale;

        speed = MinMoveSpeed + (MaxMoveSpeed - MinMoveSpeed) * (1 - k);
        Dir = Dir.Rotate(RandomUtils.NextFloat(0.5f));
    }

    private void FixedUpdate()
    {
        transform.position += speed * Timer.FixedDeltaTime() * (Vector3)Dir;
        elapse += Timer.FixedDeltaTime();
        if (elapse > 100)
            Destroy(gameObject);
    }

}