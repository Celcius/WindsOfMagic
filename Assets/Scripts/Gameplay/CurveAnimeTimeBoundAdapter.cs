using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AmoaebaUtils;

public class CurveAnimeTimeBoundAdapter : TimeBoundAdapter<CurveAnim>
{
    protected override void UpdateTime(CurveAnim component, float gameTime)
    {
        component.SetElapsedTime(gameTime);
    }
}
