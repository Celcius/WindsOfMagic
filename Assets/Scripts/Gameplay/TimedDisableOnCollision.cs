using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AmoaebaUtils;

public class TimedDisableOnCollision : DisableOnCollision
{
    protected override void Apply(Transform transform)
    {
        if(GameTime.Instance.GameSpeed <= 0)
        {
            return;
        }
        base.Apply(transform);
    } 
}
