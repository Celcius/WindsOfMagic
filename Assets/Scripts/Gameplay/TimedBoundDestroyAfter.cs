using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AmoaebaUtils;

public class TimedBoundDestroyAfter : FadeDestroyAfter
{
    protected override float GetDeltaTime()
    {
        return GameTime.Instance.DeltaTime;
    }

    protected override void DestroySelf()
    {
        gameObject.SetActive(false);
    }
    
}
