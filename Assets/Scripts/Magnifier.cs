using System;
using System.Collections;
using System.Collections.Generic;
using Lucky.Framework;
using Lucky.Interactive;
using Lucky.Utilities;
using Slime;
using UnityEngine;

public class Magnifier : Interactable
{
    private HashSet<Slime.Slime> slimes = new();
    private bool startDrag = false;

    private void Awake()
    {
        // todo: 到时候开了
        Cursor.visible = false;
    }

    protected override void ManagedUpdate()
    {
        base.ManagedUpdate();
        transform.position = GameCursor.MouseWorldPos;

        float angle = 45; // 一开始先转正

        Vector2 vec = (Vector3)CameraUtils.BottomRight - transform.position;
        angle += MathUtils.SignedAngle(Vector2.right, vec) * 0.8f;
        transform.eulerAngles = new Vector3(0, 0, angle);
    }

    // protected override void OnCursorPress()
    // {
    //     base.OnCursorPress();
    //    
    // }

    protected override void OnCursorDrag(Vector2 delta)
    {
        base.OnCursorDrag(delta);
        if (delta != Vector2.zero && !startDrag)
        {
            startDrag = true; // 这种drag是要有偏移才开始drag的

            foreach (var slime in SlimeSpawner.Instance.Slimes)
            {
                if (slime.StateMachine.State == Slime.Slime.StDizzy)
                    continue;
                if (slime.StateMachine != Slime.Slime.StDeath && slime.collider.IsTouching(Collider))
                {
                    slimes.Add(slime);
                    slime.StateMachine.State = Slime.Slime.StPickedup;
                }
            }
        }

        if (startDrag)
            foreach (var slime in slimes)
            {
                slime.transform.position += (Vector3)delta;
            }
    }

    protected override void OnCursorRelease()
    {
        base.OnCursorRelease();
        foreach (var slime in slimes)
        {
            if (slime.StateMachine.State == Slime.Slime.StDizzy)
                continue;
            slime.StateMachine.State = Slime.Slime.StRun;
            if (RandomUtils.NextFloat() < 0.04f)
            {
                slime.intentionAnim.Play("Shy");
                print(123);
            }
            else
                slime.intentionSr.sprite = null;
        }

        slimes.Clear();
        startDrag = false;
    }


    protected override void OnCursorWipe()
    {
        base.OnCursorWipe();
        List<Slime.Slime> slimeToRemove = new();
        foreach (var slime in slimes)
        {
            if (RandomUtils.NextFloat() < 0.1f)
            {
                slime.StateMachine.State = Slime.Slime.StDizzy;
                slimeToRemove.Add(slime);
            }
        }

        foreach (var slime in slimeToRemove)
        {
            slimes.Remove(slime);
        }
    }
}