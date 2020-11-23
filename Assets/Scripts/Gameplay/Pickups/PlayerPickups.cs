using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AmoaebaUtils;

public class PlayerPickups: ApplyOnCollision
{
    [SerializeField]
    protected StatTierList currentStats;

    [HideInInspector]
    public PlayerStatType[] increments;
    
    [HideInInspector]
    public PlayerStatType[] decrements;



    protected override void Apply(Transform Transform)
    {
        if(GameTime.Instance.GameSpeed <= 0)
        {
            return;
        }

        string toPrint = " - PlayerPickup - \n";
        bool shouldPrint = false;

        if(increments != null)
        {
            shouldPrint = true;
            foreach(PlayerStatType type in increments)
            {
                int prev = currentStats.GetTier(type);
                bool isMax = prev == StatTierList.MaxTier;
                currentStats.IncrementTier(type);
                currentStats.IncrementTier(type);
                int end = currentStats.GetTier(type);
                if(isMax)
                
                toPrint += "[+]["+type+"]" + prev + " -> " + end + "\n";
            }
        }

        if(decrements != null)
        {
            foreach(PlayerStatType type in decrements)
            {
                int prev = currentStats.GetTier(type);
                bool isMin = prev == StatTierList.MaxTier;
                currentStats.DecrementTier(type);
                int end = currentStats.GetTier(type);
                toPrint += "[-]["+type+"]" + prev + " -> " + end + "\n";
            }
        }

        if(shouldPrint)
        {
            Debug.Log(toPrint);
        }
    }
}
