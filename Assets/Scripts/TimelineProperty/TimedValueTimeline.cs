using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AmoaebaUtils;

public class TimedValueTimeline<T,V>
       where T : TimedValue<V>
{

    private IndexableStack<TimedValue<V>> timelineStack = new IndexableStack<TimedValue<V>>();

    public bool HasValues => timelineStack.Count > 0;

    public int Count => timelineStack.Count;

    public TimedValue<V> this[int index]
    {
        get => timelineStack[index];
    }

    public void SetTimeEvents(TimedValue<V>[] values)
    {
        timelineStack.Clear();
        timelineStack.AddRange(values);
    }

    public void AddTimeEvent(T timeEvent, bool overrideFuture)
    {
        if(timelineStack.Count > 0 && !overrideFuture && timelineStack.Peek().Timestamp > timeEvent.Timestamp)
        {
           return;
        }

        RemoveAfter(timeEvent.Timestamp);
       
        if(timelineStack.Count <= 1)
        {
            timelineStack.Enqueue(timeEvent);
            return;
        }
        
        // Memory optimization, interpolates
        TimedValue<V> prev = timelineStack[timelineStack.Count-2];
        TimedValue<V> last = timelineStack[timelineStack.Count-1];
        V expectedVal = prev.RelativeLerp(last, timeEvent.Timestamp);

        if(timeEvent.SameValue(expectedVal))
        {
            timelineStack[timelineStack.Count-1] = timeEvent;
        }
        else
        {
            timelineStack.Enqueue(timeEvent);
        }
    }

    public bool RemoveAfter(float timestamp)
    {
        bool didRemove = false;
        while(timelineStack.Count > 0 && timelineStack.Peek().Timestamp >= timestamp)
        {
            timelineStack.Dequeue();
            didRemove = false;
        }

        return didRemove;
    }

    public void RevertToIndex(int index)
    {
        while(timelineStack.Count > index+1)
        {
            timelineStack.Dequeue();
        }
    }
    
    public void Clear()
    {
        timelineStack.Clear();
    }

    public TimedValue<V> GetPrevValue(float timestamp, bool removeAfter = false)
    {
        if(removeAfter)
        {
            RemoveAfter(timestamp);
            return ((TimedValue<V>)timelineStack[timelineStack.Count-1]);
        }

        for(int i = timelineStack.Count -1; i >= 0 ; i--)
        {
            TimedValue<V> timedValue = timelineStack[i];
            if(timedValue.Timestamp < timestamp)
            {
                return timedValue;
            }
        }

        return null;
    }

    public TimedValue<V> GetNextValue(float timestamp)
    {
        if(timelineStack.Count == 0)
        {
            return null;
        }

        TimedValue<V> lastVal = timelineStack[timelineStack.Count-1];
        if(lastVal.Timestamp <= timestamp || timelineStack.Count == 0)
        {
            return lastVal;
        }

        for(int i = timelineStack.Count-2; i >= 0 ; i--)
        {
            TimedValue<V> timedVal = timelineStack[i];
            if(timedVal.Timestamp < timestamp)
            {
                return timelineStack[i+1];
            }
        }

        return null;
    }

    public TimedValue<V> GetLastValue()
    {
        return HasValues? timelineStack[timelineStack.Count-1] : null;
    }

    public float DurationBetweenIndexes(int index1, int index2)
    {
        return Mathf.Abs(timelineStack[index1].Timestamp - timelineStack[index2].Timestamp);
    }

    public TimedValue<V>[] GetRange(int start, int end)
    {
        return timelineStack.GetRange(start, end-start+1).ToArray();
    }
}
