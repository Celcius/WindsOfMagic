using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollbackTimer : FloatVar
{
    [SerializeField]
    private PlayerStats currentStats;

    public int FilledRollbacks => (int)(value / currentStats.RollbackTime);
    public float UnfilledRatio => (value - (int)value) / currentStats.RollbackTime;

    public void AddPercentage(float percentage)
    {
        Value += currentStats.RollbackTime * percentage;
    }

    public void SetPercentage(float percentage)
    {
        Value = currentStats.RollbackTime * percentage;
    }

    public override float Value
    {
        get { return value; }
        set 
        { 
            float oldVal = this.value;
            this.value = Mathf.Clamp(value, 0, currentStats.RollbackTime * currentStats.RollbackCount);
            InvokeChangeEvent(oldVal, value);
        }
    }
}
