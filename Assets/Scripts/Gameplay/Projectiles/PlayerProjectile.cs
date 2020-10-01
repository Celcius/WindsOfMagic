using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerProjectile : MonoBehaviour
{
    public abstract void SetPlayerProjectileStats(float currentSpeed, ProjectileStats stats);
}
