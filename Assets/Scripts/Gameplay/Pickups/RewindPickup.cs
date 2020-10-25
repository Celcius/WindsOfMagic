using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AmoaebaUtils;

public class RewindPickup : ApplyOnCollision
{
    [SerializeField]
    private RollbackTimer timer;

    [SerializeField]
    private float percentageToFill = 0.5f;
    protected override void Apply(Transform Transform)
    {
        timer.AddPercentage(percentageToFill);  
    }

}
