using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AmoaebaUtils;

public class TimeRewindPickup : ApplyOnCollision
{
    [SerializeField]
    private FloatVar rollbackTimer;

    [SerializeField]
    private float ratioToFill = 1.0f;
    protected override void Apply(Transform Transform)
    {
        rollbackTimer.Value = rollbackTimer.Value + ratioToFill;
    }
}
