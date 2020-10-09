using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AmoaebaUtils;
public class TimedBoundRandom : SingletonScriptableObject<TimedBoundRandom>, IGameTimeListener
{
    private class TimedRandom : TimedValue<Random.State> 
    {
        public TimedRandom(float timestamp, Random.State value) : base(timestamp, value) {}

        public override Random.State RelativeLerp(TimedValue<Random.State> otherRef, float time)
        {
            if(time > otherRef.Timestamp)
            {
                return otherRef.Value;
            }
            else
            {
                return this.Value;
            }
        }

        public override bool SameValue(Random.State value)
        {
            return this.Value.Equals(value);
        }
    }


    private int seed;
    private TimelinedProperty<TimedRandom, Random.State> timeline = new TimelinedProperty<TimedRandom, Random.State>();    


    private void OnEnable()
    {
        seed = Random.Range(0, int.MaxValue);
        timeline.Clear();
        Random.InitState(seed);
        timeline.SetValue(GetCurrentState());
        GameTime.Instance.AddTimeListener(this);
    }

    private void OnDisable()
    {
        GameTime.Instance.RemoveTimeListener(this);
    }

    private TimedRandom GetCurrentState()
    {
        return new TimedRandom(GameTime.Instance.ElapsedTime, Random.state);
    }

    public static float RandomFloat(float min, float max)
    {
        return TimedBoundRandom.Instance.RangeFloat(min,max);
    }

    public static int RandomInt(int min, int max)
    {
        return TimedBoundRandom.Instance.RangeInt(min,max);
    }

    public float RangeFloat(float min, float max)
    {
        float val = Random.Range(min,max);
        timeline.SetValue(GetCurrentState());

        return val;
    }

    public int RangeInt(int min, int max)
    {
        int val = Random.Range(min,max);
        timeline.SetValue(GetCurrentState());

        return val;
    }
    
    public void OnTimeElapsed(float timeElapsed)
    {
        if(GameTime.Instance.IsReversing)
        {
            timeline.InterpolatedValueAt(GameTime.Instance.ElapsedTime);
        }
    }
}
