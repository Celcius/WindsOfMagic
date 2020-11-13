using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AmoaebaUtils;

public class TimelineFloatVar : TimelineScriptVar<TimedFloat, float>
{

    public override float GetElapsedTime()
    {
        return GameTime.Instance.ElapsedTime;
    }

    public override bool CanUpdateTimeline()
    {
        return GameTime.Instance.IsRunning;
    }

    public override bool HasValue(float val)
    {
        return Mathf.Approximately(Value, val);
    }

    public override TimedFloat GetStamp(float val, float time)
    {
        return new TimedFloat(time, val);
    }
}
