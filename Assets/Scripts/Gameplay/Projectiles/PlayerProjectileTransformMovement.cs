using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AmoaebaUtils;

[RequireComponent(typeof(TransformMovement))]
public class PlayerProjectileTransformMovement : PlayerProjectile
{
    TransformMovement movement;

    public override void SetPlayerProjectileStats(float currentSpeed, ProjectileStats stats)
    {
        movement = GetComponent<TransformMovement>();    
        transform.localScale = Vector3.one * stats.Size;
        movement.AxisMultipliers = Vector3.one* (stats.AddedSpeed + currentSpeed);
    }
}
