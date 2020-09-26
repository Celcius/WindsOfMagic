using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedFloat : TimedValue<float> 
{
    public TimedFloat(float timestamp, float value) : base(timestamp, value) {}

    public override float RelativeLerp(TimedValue<float> otherRef, float time)
    {
        float ratio = (otherRef.Value - this.Value) / (otherRef.Timestamp - this.Timestamp);

        return ratio * (time - this.Timestamp) + this.Value;
    }

    public override bool SameValue(float value)
    {
        return this.Value == value;
    }
}

public class TimedBool : TimedValue<bool> 
{
    public TimedBool(float timestamp, bool value) : base(timestamp, value) {}

    public override bool RelativeLerp(TimedValue<bool> otherRef, float time)
    {
        TimedValue<bool> left = otherRef.Timestamp < this.Timestamp ? otherRef : this;
        TimedValue<bool> right = left == this? otherRef : this;

        if(time < left.Timestamp)
        {
            return left.Value;
        } 
        else if(time > right.Timestamp)
        {
            return right.Value;
        }

        // return closest value to time
        return Mathf.Abs(time - left.Timestamp) > Mathf.Abs(right.Timestamp - time)? 
            left.Value : 
            right.Value;
    }

    public override bool SameValue(bool value)
    {
        return this.Value == value;
    }
}

public class TimedQuaternion : TimedValue<Quaternion> 
{
    public TimedQuaternion(float timestamp, Quaternion value) : base(timestamp, value) {}

    public override Quaternion RelativeLerp(TimedValue<Quaternion> otherRef, float time)
    {
        Vector3 thisEuler = this.Value.eulerAngles;
        Vector3 otherEuler = otherRef.Value.eulerAngles;
        
        Vector3 lerpedVec = TimedVector3.Lerp(thisEuler, 
                                              this.Timestamp, 
                                              otherEuler, 
                                              otherRef.Timestamp, 
                                              time);

        return Quaternion.Euler(lerpedVec);
    }

    public override bool SameValue(Quaternion value)
    {
        return this.Value.eulerAngles == value.eulerAngles;
    }
}

public class TimedVector2: TimedValue<Vector2> 
{
    public TimedVector2(float timestamp, Vector2 value) : base(timestamp, value) {}
    public override Vector2 RelativeLerp(TimedValue<Vector2> otherRef, float time)
    {       
        Vector3 lerpedVec = TimedVector3.Lerp(this.Value, 
                                              this.Timestamp, 
                                              otherRef.Value, 
                                              otherRef.Timestamp, 
                                              time);

        return (Vector2)lerpedVec;
    }

    public override bool SameValue(Vector2 value)
    {
        return this.Value == value;
    }
}

public class TimedVector3: TimedValue<Vector3> 
{
    public TimedVector3(float timestamp, Vector3 value) : base(timestamp, value) {}

    public override Vector3 RelativeLerp(TimedValue<Vector3>  otherRef, float time)
    {
        Vector3 lerpedVec = TimedVector3.Lerp(this.Value, 
                                              this.Timestamp, 
                                              otherRef.Value, 
                                              otherRef.Timestamp, 
                                              time);

        return lerpedVec;
    }

    public static Vector3 Lerp(Vector3 vecA, 
                               float timeA, 
                               Vector3 vecB,
                               float timeB, 
                               float lerpTime)
    {
        Vector3 ratio = (vecB - vecA) / (timeB - timeA);

        return ratio * (lerpTime - timeA) + vecA;
    }

    public override bool SameValue(Vector3 value)
    {
        return this.Value == value;
    }
}

public struct TimedTransformStruct
{
    public Vector3 position;
    public Vector3 rotationEuler;
    public Vector3 scale;

    public TimedTransformStruct(Vector3 position, Vector3 rotationEuler, Vector3 scale)
    {
        this.position = position;
        this.rotationEuler = rotationEuler;
        this.scale = scale;
    }
}
