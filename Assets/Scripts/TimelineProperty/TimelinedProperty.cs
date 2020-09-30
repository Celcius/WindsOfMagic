using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimelinedProperty<T,V> where T : TimedValue<V>
{
    private TimedValueTimeline<T,V> timeline = new TimedValueTimeline<T,V>();

    public bool HasValue => timeline.HasValues;

    public float Duration => timeline.Count <= 1? 0 : 
                             timeline.DurationBetweenIndexes(0, timeline.Count-1);

    public V Value => timeline.HasValues? timeline.GetLastValue().Value : default(V);

    public int TimePoints => timeline.Count;
    
    public bool HasValueBefore(float timestamp)
    {
        return timeline.HasValues && ((TimedValue<V>)timeline[0]).Timestamp < timestamp;
    }

    public V InterpolatedValueAt(float timestamp, bool clampRightEdge = true)
    {
        if(!HasValue)
        {
            return default(V);
        }
        else if(!HasValueBefore(timestamp) ||  timeline.Count == 1)
        {
            return timeline[0].Value;
        }

        T left, right;

        if(timestamp > timeline[timeline.Count-1].Timestamp)
        {
            if(clampRightEdge)
            {
                return timeline[timeline.Count-1].Value;
            }
            left = timeline[timeline.Count-2];
            right = timeline[timeline.Count-1];
        }
        else
        {
            left = timeline.GetPrevValue(timestamp);
            right = timeline.GetNextValue(timestamp);
        }        

        return left.RelativeLerp(right, timestamp);
    }


    public void UpdateValue(float timestamp, V value)
    {
        if(timeline.Count == 0)
        {
            return;
        }
        
        T  nextValue = timeline.GetNextValue(timestamp);
        nextValue.Timestamp = timestamp;
        nextValue.Value = value;
        SetValue(nextValue);
    }

    public void SetValue(T value)
    {
        timeline.AddTimeEvent(value, true);
    }

    public void RevertToStart()
    {
        timeline.RevertToIndex(0);
    }

    public void SetValues(T[] values, bool clearAll = true)
    {
        if(clearAll)
        {
            timeline.Clear();
        }

        foreach(T val in values)
        {
            SetValue(val);
        }
    }

    public void ClipDurationFromEnd(float duration)
    {
        if(Duration <= duration)
        {
            return;
        }

        int i = timeline.Count-1;
        while(i > 1 && timeline.DurationBetweenIndexes(0, i-1) > duration)
        {
            --i;
        }

        float lastInstant = timeline[0].Timestamp + duration;
        
        T[] values = timeline.GetRange(0, i);
        values[i].Timestamp = lastInstant;
        values[i].Value = InterpolatedValueAt(lastInstant);
        timeline.SetTimeEvents(values);
    }

    public void ClipDurationFromBeginning(float duration)
    {
        if(Duration <= duration)
        {
            return;
        }

        int lastIndex =  timeline.Count-1;

        int i = 0;
        while(i < lastIndex-1 && timeline.DurationBetweenIndexes(i+1, lastIndex) > duration)
        {
            ++i;
        }

        float firstInstant = timeline[lastIndex].Timestamp - duration;
        
        T[] values = timeline.GetRange(i, lastIndex);
        values[0].Timestamp = firstInstant;
        values[0].Value = InterpolatedValueAt(firstInstant);
        timeline.SetTimeEvents(values);
    }
}
