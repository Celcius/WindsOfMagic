using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;

public class TimelinePropertyTestSuite
{
    float elapsedTime;

    [SetUp]
    public void Setup()
    {
        elapsedTime = 0.0f;
    }

    [TearDown]
    public void Teardown()
    {
        elapsedTime = 0.0f;
    }

    [UnityTest]
    public IEnumerator TestInit()
    {
        TimelineFloat timeFloat = new TimelineFloat();

        Assert.False(timeFloat.HasValue, "New TimelineProperty with values");
        Assert.False(timeFloat.HasValueBefore(0), "New TimelineProperty with values");
        Assert.False(timeFloat.HasValueBefore(-5), "New TimelineProperty with values");
        Assert.False(timeFloat.HasValueBefore(100), "New TimelineProperty with values");
        Assert.True(timeFloat.Value == default(float), "New TimelineProperty with unexpected Value");
        Assert.True(timeFloat.TimePoints == 0, "New TimelineProperty with unexpected Value");
        Assert.True(timeFloat.Duration == 0.0f, "TimelineProperty with unexpected Duration");
        

        yield break;
    }

    [UnityTest]
    public IEnumerator TestSetValue()
    {
        TimelineFloat timeFloat = new TimelineFloat();

        TimedFloat testVal1 = new TimedFloat(1,1);
        TimedFloat testVal2 = new TimedFloat(2,5);
        TimedFloat testVal3 = new TimedFloat(3, 10);
        TimedFloat testVal4 = new TimedFloat(1.5f ,5);
        
        timeFloat.SetValue(testVal1);
        Assert.True(timeFloat.Duration == 0.0f, "TimelineProperty with unexpected Duration");

        timeFloat.SetValue(testVal2);
        timeFloat.SetValue(testVal3);

        Assert.True(timeFloat.HasValue, "TimelineProperty without values");
        Assert.True(timeFloat.Value == testVal3.Value, "TimelineProperty with unexpected Value");
        Assert.True(timeFloat.TimePoints == 3, "TimelineProperty with unexpected Value");
        Assert.False(timeFloat.HasValueBefore(0), "Unexpected HasValueBefore");
        Assert.False(timeFloat.HasValueBefore(-5), "Unexpected HasValueBefore");
        Assert.True(timeFloat.HasValueBefore(1.5f), "Unexpected HasValueBefore");
        Assert.True(timeFloat.HasValueBefore(100), "Unexpected HasValueBefore");

        timeFloat.SetValue(testVal4);

        Assert.True(timeFloat.HasValue, "TimelineProperty without values");
        Assert.True(timeFloat.Value == testVal4.Value, "TimelineProperty with unexpected Value");
        Assert.True(timeFloat.TimePoints == 2, "TimelineProperty with unexpected Value");
        Assert.False(timeFloat.HasValueBefore(0),  "Unexpected HasValueBefore");
        Assert.False(timeFloat.HasValueBefore(-5),  "Unexpected HasValueBefore");
        Assert.True(timeFloat.HasValueBefore(1.5f),  "Unexpected HasValueBefore");
        Assert.True(timeFloat.HasValueBefore(100),  "Unexpected HasValueBefore");

        yield break;
    }

       [UnityTest]
    public IEnumerator SetValues()
    {
        TimelineFloat timeFloat = new TimelineFloat();
        TimedFloat[] testVals = {new TimedFloat(1,5), 
                                 new TimedFloat(2,5),
                                 new TimedFloat(3.25f, 25)};

        TimedFloat[] testVals2 = {new TimedFloat(5,5), 
            new TimedFloat(6,10)};

        TimedFloat[] testVals3 = {new TimedFloat(5.5f,5), 
            new TimedFloat(7,30)};

        timeFloat.SetValues(testVals);

        Assert.True(timeFloat.HasValue, "TimelineProperty without values");
        Assert.True(timeFloat.Value == testVals[2].Value, "TimelineProperty with unexpected Value");
        Assert.True(timeFloat.TimePoints == 3, "TimelineProperty with unexpected Value");
        Assert.False(timeFloat.HasValueBefore(0),  "Unexpected HasValueBefore");
        Assert.False(timeFloat.HasValueBefore(-5),  "Unexpected HasValueBefore");
        Assert.True(timeFloat.HasValueBefore(1.5f),  "Unexpected HasValueBefore");
        Assert.True(timeFloat.HasValueBefore(100),  "Unexpected HasValueBefore");

        timeFloat.SetValues(testVals2, true);

        Assert.True(timeFloat.HasValue, "TimelineProperty without values");
        Assert.True(timeFloat.Value == testVals2[1].Value, "TimelineProperty with unexpected Value");
        Assert.True(timeFloat.TimePoints == 2, "TimelineProperty with unexpected Value");
        Assert.False(timeFloat.HasValueBefore(0),  "Unexpected HasValueBefore");
        Assert.False(timeFloat.HasValueBefore(-5),  "Unexpected HasValueBefore");
        Assert.False(timeFloat.HasValueBefore(4.7f),  "Unexpected HasValueBefore");
        Assert.True(timeFloat.HasValueBefore(5.5f),  "Unexpected HasValueBefore");
        Assert.True(timeFloat.HasValueBefore(100),  "Unexpected HasValueBefore");

        timeFloat.SetValues(testVals3, false);

        Assert.True(timeFloat.HasValue, "TimelineProperty without values");
        Assert.True(timeFloat.Value == testVals3[1].Value, "TimelineProperty with unexpected Value");
        Assert.True(timeFloat.TimePoints == 3, "TimelineProperty with unexpected Value");
        Assert.False(timeFloat.HasValueBefore(0),  "Unexpected HasValueBefore");
        Assert.False(timeFloat.HasValueBefore(-5),  "Unexpected HasValueBefore");
        Assert.False(timeFloat.HasValueBefore(4.7f),  "Unexpected HasValueBefore");
        Assert.True(timeFloat.HasValueBefore(5.5f),  "Unexpected HasValueBefore");
        Assert.True(timeFloat.HasValueBefore(100),  "Unexpected HasValueBefore");

        yield break;
    }

    [UnityTest]
    public IEnumerator TestHasBefore()
    {
        TimelineFloat timeFloat = new TimelineFloat();

        TimedFloat testVal1 = new TimedFloat(1,1);
        TimedFloat testVal2 = new TimedFloat(2,5);
        TimedFloat testVal3 = new TimedFloat(3, 10);
        TimedFloat testVal4 = new TimedFloat(1.5f ,5);
        
        timeFloat.SetValue(testVal1);
        timeFloat.SetValue(testVal2);
        timeFloat.SetValue(testVal3);

        Assert.True(timeFloat.HasValue, "TimelineProperty without values");
        Assert.True(timeFloat.Value == testVal3.Value, "TimelineProperty with unexpected Value");
        Assert.True(timeFloat.TimePoints == 3, "TimelineProperty with unexpected Value");

        timeFloat.SetValue(testVal4);

        Assert.True(timeFloat.HasValue, "TimelineProperty without values");
        Assert.True(timeFloat.Value == testVal4.Value, "TimelineProperty with unexpected Value");
        Assert.True(timeFloat.TimePoints == 2, "TimelineProperty with unexpected Value");

        yield break;
    }

    [UnityTest]
    public IEnumerator TestAddInterpolation()
    {
        TimelineFloat timeFloat = new TimelineFloat();

        TimedFloat testVal1 = new TimedFloat(1,1);
        TimedFloat testVal2 = new TimedFloat(2,2);
        TimedFloat testVal3 = new TimedFloat(3,3);
        TimedFloat testVal4 = new TimedFloat(4 ,5);
        
        timeFloat.SetValue(testVal1);
        timeFloat.SetValue(testVal2);
        timeFloat.SetValue(testVal3);

        Assert.True(timeFloat.HasValue, "TimelineProperty without values");
        Assert.True(timeFloat.Value == testVal3.Value, "TimelineProperty with unexpected Value");
        Assert.True(timeFloat.TimePoints == 2, "TimelineProperty with unexpected Value");

        timeFloat.SetValue(testVal4);

        Assert.True(timeFloat.HasValue, "TimelineProperty without values");
        Assert.True(timeFloat.Value == testVal4.Value, "TimelineProperty with unexpected Value");
        Assert.True(timeFloat.TimePoints == 3, "TimelineProperty with unexpected Value");

        yield break;
    }

   [UnityTest]
    public IEnumerator TestInterpolatedValue()
    {
        TimelineFloat timeFloat = new TimelineFloat();
        TimedFloat testVal1 = new TimedFloat(1,5);
        TimedFloat testVal2 = new TimedFloat(2,5);
        TimedFloat testVal3 = new TimedFloat(3, 25);
        TimedFloat testVal4 = new TimedFloat(1.5f ,5);
    
        Assert.True(timeFloat.InterpolatedValueAt(1.0f) == default(float), "TimelineProperty with unexpected Value");
        Assert.True(timeFloat.InterpolatedValueAt(0.0f) == default(float), "TimelineProperty with unexpected Value");

        timeFloat.SetValue(testVal1);

        Assert.True(timeFloat.InterpolatedValueAt(0.5f) == testVal1.Value, "TimelineProperty with unexpected Value");
        Assert.True(timeFloat.InterpolatedValueAt(2.0f) == testVal1.Value, "TimelineProperty with unexpected Value");
        Assert.True(timeFloat.InterpolatedValueAt(3.0f) == testVal1.Value, "TimelineProperty with unexpected Value");

        timeFloat.SetValue(testVal4);

        Assert.True(timeFloat.InterpolatedValueAt(0.5f) == testVal4.Value, "TimelineProperty with unexpected Value");
        Assert.True(timeFloat.InterpolatedValueAt(2.0f) == testVal4.Value, "TimelineProperty with unexpected Value");
        Assert.True(timeFloat.InterpolatedValueAt(3.0f) == testVal4.Value, "TimelineProperty with unexpected Value");
        
        timeFloat.SetValue(testVal1);
        timeFloat.SetValue(testVal2);
        timeFloat.SetValue(testVal3);

        float interpolate0 = testVal4.Value;
        float interpolate05 = testVal4.Value;
        float interpolate175 = QuickInterpolation(testVal4, testVal2, 1.75f); 
        float interpolate2 = testVal2.Value;
        float interpolate25 = QuickInterpolation(testVal3, testVal2, 2.5f);
        float interpolate3 = testVal3.Value;
        float interpolate4v1 = testVal3.Value;
        float interpolate4v2 = QuickInterpolation(testVal2, testVal3, 4.0f);
        float interpolateNeg2 = testVal4.Value;

        
        Assert.True(Mathf.Approximately(timeFloat.InterpolatedValueAt(0.0f), interpolate0), "TimelineProperty with unexpected Value");
        Assert.True(Mathf.Approximately(timeFloat.InterpolatedValueAt(0.5f), interpolate05), "TimelineProperty with unexpected Value");
        Assert.True(Mathf.Approximately(timeFloat.InterpolatedValueAt(1.75f), interpolate175), "TimelineProperty with unexpected Value");
        Assert.True(Mathf.Approximately(timeFloat.InterpolatedValueAt(2.0f), interpolate2), "TimelineProperty with unexpected Value");
        Assert.True(Mathf.Approximately(timeFloat.InterpolatedValueAt(2.5f), interpolate25), "TimelineProperty with unexpected Value");
        Assert.True(Mathf.Approximately(timeFloat.InterpolatedValueAt(3.0f), interpolate3), "TimelineProperty with unexpected Value");
        Assert.True(Mathf.Approximately(timeFloat.InterpolatedValueAt(4.0f), interpolate4v1), "TimelineProperty with unexpected Value");
        Assert.True(Mathf.Approximately(timeFloat.InterpolatedValueAt(4.0f, false), interpolate4v2), "TimelineProperty with unexpected Value");
        Assert.True(Mathf.Approximately(timeFloat.InterpolatedValueAt(-2.0f), interpolateNeg2), "TimelineProperty with unexpected Value");

        yield break;
    }


    [UnityTest]
    public IEnumerator TestClipBeginning()
    {
        TimelineFloat timeFloat = new TimelineFloat();
        yield return TestInit();
        timeFloat.ClipDurationFromBeginning(1.0f);
        yield return TestInit();

        TimedFloat[] testVals = {new TimedFloat(1,5), 
                                 new TimedFloat(2,5),
                                 new TimedFloat(3.25f, 25)};

        timeFloat.SetValues(testVals);

        Assert.True(timeFloat.Duration == 2.25f, "TimelineProperty with unexpected Duration");
        Assert.True(timeFloat.TimePoints == 3, "TimelineProperty with unexpected points");
        Assert.True(timeFloat.Value == testVals[2].Value, "TimelineProperty with unexpected Value");

        timeFloat.ClipDurationFromBeginning(3.0f);

        Assert.True(timeFloat.Duration == 2.25f, "TimelineProperty with unexpected Duration");
        Assert.True(timeFloat.TimePoints == 3, "TimelineProperty with unexpected points");
        Assert.True(timeFloat.Value == testVals[2].Value, "TimelineProperty with unexpected Value");

        timeFloat.ClipDurationFromBeginning(2.25f);

        Assert.True(timeFloat.Duration == 2.25f, "TimelineProperty with unexpected Duration");
        Assert.True(timeFloat.TimePoints == 3, "TimelineProperty with unexpected points");
        Assert.True(timeFloat.Value == testVals[2].Value, "TimelineProperty with unexpected Value");

        timeFloat.ClipDurationFromBeginning(1.5f);

        Assert.True(timeFloat.Duration == 1.5f, "TimelineProperty with unexpected Duration");
        Assert.True(timeFloat.TimePoints == 3, "TimelineProperty with unexpected Value");
        Assert.True(timeFloat.Value == testVals[2].Value, "TimelineProperty with unexpected Value");

        timeFloat.ClipDurationFromBeginning(0.5f);

        Assert.True(timeFloat.Duration == 0.5f, "TimelineProperty with unexpected Duration");
        Assert.True(timeFloat.TimePoints == 2, "TimelineProperty with unexpected Value");
        Assert.True(timeFloat.Value == testVals[2].Value, "TimelineProperty with unexpected Value");
    }

    

    [UnityTest]
    public IEnumerator TestClipEnd()
    {
        TimelineFloat timeFloat = new TimelineFloat();
        yield return TestInit();
        timeFloat.ClipDurationFromEnd(1.0f);
        yield return TestInit();

        TimedFloat[] testVals = {new TimedFloat(1,5), 
                                 new TimedFloat(2,5),
                                 new TimedFloat(3.25f, 25)};

        timeFloat.SetValues(testVals);

        Assert.True(timeFloat.Duration == 2.25f, "TimelineProperty with unexpected Duration");
        Assert.True(timeFloat.TimePoints == 3, "TimelineProperty with unexpected points");
        Assert.True(timeFloat.Value == testVals[2].Value, "TimelineProperty with unexpected Value");

        timeFloat.ClipDurationFromEnd(3.0f);

        Assert.True(timeFloat.Duration == 2.25f, "TimelineProperty with unexpected Duration");
        Assert.True(timeFloat.TimePoints == 3, "TimelineProperty with unexpected points");
        Assert.True(timeFloat.Value == testVals[2].Value, "TimelineProperty with unexpected Value");

        timeFloat.ClipDurationFromEnd(2.25f);

        Assert.True(timeFloat.Duration == 2.25f, "TimelineProperty with unexpected Duration");
        Assert.True(timeFloat.TimePoints == 3, "TimelineProperty with unexpected points");
        Assert.True(timeFloat.Value == testVals[2].Value, "TimelineProperty with unexpected Value");

        timeFloat.ClipDurationFromEnd(1.5f);

        float expectedValue = QuickInterpolation(testVals[1], testVals[2], 2.5f);
        Assert.True(timeFloat.Duration == 1.5f, "TimelineProperty with unexpected Duration");
        Assert.True(timeFloat.TimePoints == 3, "TimelineProperty with unexpected Value");
        Assert.True(timeFloat.Value == expectedValue, "TimelineProperty with unexpected Value");

        
        timeFloat.ClipDurationFromEnd(0.5f);

        expectedValue = QuickInterpolation(testVals[0], testVals[1], 1.5f);
        Assert.True(timeFloat.Duration == 0.5f, "TimelineProperty with unexpected Duration");
        Assert.True(timeFloat.TimePoints == 2, "TimelineProperty with unexpected Value");
        Assert.True(timeFloat.Value == expectedValue, "TimelineProperty with unexpected Value");
    }

    private float QuickInterpolation(TimedFloat a, TimedFloat b, float Yc)
    { 
        float Xa = a.Value;
        float Ya = a.Timestamp;
        float Xb = b.Value;
        float Yb = b.Timestamp;
        return ((Xb - Xa) / (Yb - Ya)) * (Yc - Ya) + Xa;
    }
}
