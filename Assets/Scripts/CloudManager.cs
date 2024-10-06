using System;
using System.Collections;
using System.Collections.Generic;
using Lucky.Extensions;
using Lucky.Framework;
using Lucky.Utilities;
using UnityEngine;

public class CloudManager : ManagedBehaviour
{
    public Cloud cloudPrefab;
    public float Duration;
    public float angle;

    private void Awake()
    {
        this.CreateFuncTimer(
            () =>
            {
                Cloud cloud = Instantiate(cloudPrefab, transform.position, Quaternion.identity);
                cloud.Dir = MathUtils.AngleToVector(angle, 1);
            }, () => Duration, isStartImmediate:true
        );
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawRay(transform.position, MathUtils.AngleToVector(angle, 10));
    }
}