using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct BulletWave
{
    public Transform projectile;
    public int amount;

    public float range;

    public float rotation;
    public float damage; 

    public Color debugColor;
}
