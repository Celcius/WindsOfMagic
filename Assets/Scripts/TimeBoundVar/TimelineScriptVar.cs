using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AmoaebaUtils;

public abstract class TimelineScriptVar<T, V> : ScriptVar<V>
    where T : TimedValue<V> 
{
    private TimelinedProperty<T, V> history = new TimelinedProperty<T, V>();
    
    public override V Value
    {
        get { return history.Value; }
        set 
        { 
            if(CanUpdateTimeline() && !HasValue(value))
            {
                V oldVal = this.Value;

                float timeInstant = GetElapsedTime();
                if(timeInstant < history.LastInstant)
                {
                    history.ClipDurationFromEnd(timeInstant, false);
                }
                else
                {
                    history.SetValue(GetStamp(value, timeInstant));
                }
                
                InvokeChangeEvent(oldVal, value);
            }
        }
    }

    public virtual float GetElapsedTime()
    {
        return Time.time;
    }

    public virtual bool CanUpdateTimeline()
    {
        return true;
    }

    public void Reset()
    {
        history.Clear();
    }

    public abstract bool HasValue(V val);

    public abstract T GetStamp(V val, float time);
}
