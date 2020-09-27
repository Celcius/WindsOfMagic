using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AmoaebaUtils;

public class TransformMovementTimeBoundAdapter : TimeBoundAdapter<TransformMovement>
{
    protected override void UpdateTime(TransformMovement component, float gameTime)
    {
        component.SetElapsedTime(gameTime);
    }
}