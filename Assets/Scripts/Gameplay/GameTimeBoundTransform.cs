using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AmoaebaUtils;

public class GameTimeBoundTransform : MonoBehaviour
{
    [SerializeField]
    private GameTimeVar gameTime;

    private TimelinedProperty<TimedVector3, Vector3> posTimeline = new TimelinedProperty<TimedVector3, Vector3>();
    private TimelinedProperty<TimedQuaternion, Quaternion> rotationTimeline = new TimelinedProperty<TimedQuaternion, Quaternion>();
    private TimelinedProperty<TimedVector3, Vector3> scaleTimeline = new TimelinedProperty<TimedVector3, Vector3>();
    
    private void Start()
    {
        posTimeline.SetValue(new TimedVector3(gameTime.ElapsedTime, transform.position));
        rotationTimeline.SetValue(new TimedQuaternion(gameTime.ElapsedTime, transform.rotation));
        scaleTimeline.SetValue(new TimedVector3(gameTime.ElapsedTime, transform.localScale));
    }

    void Update()
    {
        gameTime.UpdateTimeline<TimedVector3, Vector3>(posTimeline, 
            new TimedVector3(gameTime.ElapsedTime, transform.position));

        gameTime.UpdateTimeline<TimedQuaternion, Quaternion>(rotationTimeline,
            new TimedQuaternion(gameTime.ElapsedTime, transform.rotation));

        
        gameTime.UpdateTimeline<TimedVector3, Vector3>(scaleTimeline,
            new TimedVector3(gameTime.ElapsedTime, transform.localScale));

        transform.position = posTimeline.Value;
        transform.rotation = rotationTimeline.Value;
        transform.localScale = scaleTimeline.Value;
    }

    
}
