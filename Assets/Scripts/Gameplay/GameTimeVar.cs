using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTimeVar : ScriptVar<GameTime>
{
    public float ElapsedTime
    {
        get { return Value.ElapsedTime; }
        set 
        { 
            GameTime newTime = this.Value;
            newTime.ElapsedTime = value; 
            this.Value = newTime;
        }
    }

    public float GameSpeed
    {
        get { return Value.GameSpeed; }
        set
        { 
            GameTime newTime = this.Value;
            newTime.GameSpeed = value; 
            this.Value = newTime;
        }
    }

    public void UpdateTimeline<T,V>(TimelinedProperty<T, V> timeline, T currentValue) 
        where T : TimedValue<V>
    {
        float gameSpeed = this.Value.GameSpeed;
        float time  = this.Value.ElapsedTime;

        if(gameSpeed > 0)
        {
            timeline.SetValue(currentValue);
        } 
        else if(gameSpeed < 0)
        {
            if(timeline.HasValueBefore(time))
            {
                timeline.SetValue(time, timeline.InterpolatedValueAt(time));
            }
            else if(timeline.TimePoints > 1)
            {
                timeline.RevertToStart();
            }
        }
    }
}
