using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct PickupRepresentation
{
    public PlayerStatType[] increments;
    public PlayerStatType[] decrements;
    public Sprite image;
    public string topLabel;
    public string botLabel;
}
