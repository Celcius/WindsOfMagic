using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TimedValue<V> 
{
    public float Timestamp;
    public V Value;

    public TimedValue(float timestamp, V value)
    {
        this.Timestamp = timestamp;
        this.Value = value;
    }

    public abstract V RelativeLerp(TimedValue<V> otherRef, float time);
    
    public abstract bool SameValue(V value);
}

