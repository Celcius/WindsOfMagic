using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AmoaebaUtils;

public class ApplyToTimelineFloatVarOnCollision : ApplyToVarOnCollision<float, TimelineFloatVar>
{
    [SerializeField]
    private float inc;

    protected override void Apply(Transform col)
    {
        var.Value = var.Value + inc;
    }
}
