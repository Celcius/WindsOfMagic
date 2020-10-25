using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AmoaebaUtils;

public class HealthPickup : ApplyOnCollision
{
    [SerializeField]
    private float percentageToFill = 0.1f;
    protected override void Apply(Transform Transform)
    {
        TimedHealth health = Transform.GetComponent<TimedHealth>();
        health.SetHealthDelta(health.CurrentMaxHealth * percentageToFill);   
    }
}
