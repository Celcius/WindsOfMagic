using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct PickupRepresentation
{
    public PlayerStats playerStats;
    public ProjectileStats projectileStats;
    public Sprite image;
    public string topLabel;
    public string botLabel;
}
