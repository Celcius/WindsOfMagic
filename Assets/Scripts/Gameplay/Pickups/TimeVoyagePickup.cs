using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AmoaebaUtils;

public class TimeVoyagePickup : ApplyOnCollision
{
    [SerializeField]
    private FloatVar timeVoyageBar;
    
    [SerializeField]
    private float ratioToFill = 1.0f;
    
    protected override void Apply(Transform Transform)
    {
        timeVoyageBar.Value = Mathf.Clamp01(timeVoyageBar.Value + ratioToFill);
    }
}
