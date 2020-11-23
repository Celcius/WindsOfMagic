using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AmoaebaUtils;

[RequireComponent(typeof(RoundPickup))]
public class InstantiatePickupTextOnCollision : InstantiateOnCollision
{

    [SerializeField]
    private StatTierList statTierList;

    protected override void Apply(Transform col)
    {
        base.Apply(col);
        if(instantiated != null)
        {
            PickupText text = instantiated.GetComponent<PickupText>();
            RoundPickup pickup = GetComponent<RoundPickup>();
            if(pickup != null)
            {
                bool isMax = pickup.PlayerStats.increments.Length > 0 && IsMaxStat(pickup.PlayerStats.increments[0]);
                bool isMin = pickup.PlayerStats.decrements.Length > 0 && IsMinStat(pickup.PlayerStats.decrements[0]);
            
                string top = (isMax? "Max " : "+ " ) + pickup.topLabel;
                string bot = (isMin? "Min " : "- " ) + pickup.botLabel;
                text.SetLabels(top, bot);
            }   
        }
    }

    private bool IsMaxStat(PlayerStatType stat)
    {
        return statTierList.GetTier(stat) >= StatTierList.MaxTier;
    }

    private bool IsMinStat(PlayerStatType stat)
    {
        return statTierList.GetTier(stat) <= StatTierList.MinTier;
    }
}
