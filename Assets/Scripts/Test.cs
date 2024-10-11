using System;
using System.Collections;
using System.Collections.Generic;
using Lucky.Framework;
using UnityEngine;
using Animator = Lucky.Framework.Animation.Animator;

public class Test : ManagedBehaviour
{
    private void Awake()
    {
        Animator anim = new Animator("/");
        Add(anim);
        anim.Add("idle", "butterfly", 1);
        // anim.Play("idle");
    }
}
