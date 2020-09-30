using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AmoaebaUtils;

public class ProjectilePickups : ApplyOnCollision
{
    [SerializeField]
    protected ProjectileStats currentStats;

    [SerializeField]
    protected ProjectileStats offsetToApply;

    [SerializeField]
    protected ProjectileStats minCurrentStats;

    [SerializeField]
    protected ProjectileStats maxCurrentStats;

    protected override void Apply(Transform Transform)
    {
        if(offsetToApply == null || GameTime.Instance.GameSpeed <= 0)
        {
            return;
        }
        currentStats.AddProjectileStats(offsetToApply, minCurrentStats, maxCurrentStats);
    }
}
