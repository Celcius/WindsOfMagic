using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AmoaebaUtils;

[RequireComponent(typeof(RoundPickup))]
public class InstantiatePickupTextOnCollision : InstantiateOnCollision
{
    protected override void Apply(Transform col)
    {
        base.Apply(col);
        if(instantiated != null)
        {
            PickupText text = instantiated.GetComponent<PickupText>();
            RoundPickup pickup = GetComponent<RoundPickup>();
            if(pickup != null)
            {
                text.SetLabels(pickup.topLabel, pickup.botLabel);
            }
            
        }
    }
}
