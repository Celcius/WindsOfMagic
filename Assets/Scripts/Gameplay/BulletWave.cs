using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletWave : ScriptableObject
{
    public Transform projectile;
    public float amount = 1;

    [Range(0, 360)]
    public float range = 360;

    public float rotation = 0;
}
