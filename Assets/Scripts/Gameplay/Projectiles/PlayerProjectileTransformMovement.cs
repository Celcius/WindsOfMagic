using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AmoaebaUtils;

[RequireComponent(typeof(TransformMovement))]
[RequireComponent(typeof(ApplyDamageOnCollision))]
public class PlayerProjectileTransformMovement : PlayerProjectile
{
    TransformMovement movement;
    ApplyDamageOnCollision damageOnCollision;

    public override void SetPlayerProjectileStats(float currentSpeed, ProjectileStats stats)
    {
        movement = GetComponent<TransformMovement>();    
        damageOnCollision = GetComponent<ApplyDamageOnCollision>();
        
        transform.localScale = Vector3.one * stats.Size;
        movement.AxisMultipliers = Vector3.one* (stats.AddedSpeed + currentSpeed);
        damageOnCollision.Damage = stats.Damage;
    }
}
