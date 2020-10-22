using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AmoaebaUtils;

[System.Serializable]
public class PickupRepresentationArr : ScriptableObject
{
    [SerializeField]
    private PickupRepresentation[] pickups;
    public PickupRepresentation[] Pickups => pickups;
}
