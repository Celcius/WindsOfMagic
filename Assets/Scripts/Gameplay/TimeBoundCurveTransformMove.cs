using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AmoaebaUtils;

[RequireComponent(typeof(TransformMovementTimeBoundAdapter))]
public class TimeBoundCurveTransformMove : CurveTransformMove
{
    private GameTime timeHandler;

    private void Start() 
    {
        timeHandler = GameTime.Instance;    
    }
    
    public override float GetDeltaTime()
    {
        return timeHandler.DeltaTime;
    }
}
