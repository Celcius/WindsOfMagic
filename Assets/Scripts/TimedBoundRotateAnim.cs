using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AmoaebaUtils;

public class TimedBoundRotateAnim : RotateAnim
{
    [SerializeField]
    private bool randomDir = false;

    private float dir = 1.0f;

    protected override void OnChange(float evaluatedVal)
    {
        base.OnChange(dir* evaluatedVal);
    }

    
    void Start()
    {
        dir = Mathf.Sign(TimedBoundRandom.RandomFloat(float.MinValue, float.MaxValue));
        base.Start();
    }
}
