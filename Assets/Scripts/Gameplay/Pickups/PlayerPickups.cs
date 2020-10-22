using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AmoaebaUtils;

public class PlayerPickups: ApplyOnCollision
{
    [SerializeField]
    protected PlayerStats currentStats;

    [SerializeField]
    public PlayerStats offsetToApply;

    [SerializeField]
    protected PlayerStats minCurrentStats;

    [SerializeField]
    protected PlayerStats maxCurrentStats;

    protected override void Apply(Transform Transform)
    {
        if(offsetToApply == null || GameTime.Instance.GameSpeed <= 0)
        {
            return;
        }
        currentStats.AddPlayerStats(offsetToApply, minCurrentStats, maxCurrentStats);
    }
}
