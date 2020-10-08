using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AmoaebaUtils;

public class GameTimeBoundTransform : MonoBehaviour, IGameTimeListener
{    private GameTime gameTime;

    private TimelinedProperty<TimedVector3, Vector3> posTimeline = new TimelinedProperty<TimedVector3, Vector3>();
    private TimelinedProperty<TimedQuaternion, Quaternion> rotationTimeline = new TimelinedProperty<TimedQuaternion, Quaternion>();
    private TimelinedProperty<TimedVector3, Vector3> scaleTimeline = new TimelinedProperty<TimedVector3, Vector3>();
    
    private float disableTime = -1;

    private bool ignoreGameSpeed = false;
    float ignoreStartPoint;
    float currentElapsed;
    float totalElapsed;

    public bool IgnoreGameSpeed
    {
        get { return ignoreGameSpeed; }
        set 
        {
            if(value != ignoreGameSpeed)
            {
                if(value)
                {
                    ignoreStartPoint = GameTime.Instance.ElapsedTime;
                    currentElapsed = 0;
                }
                else 
                {
                    totalElapsed += Mathf.Abs(ignoreStartPoint - GameTime.Instance.ElapsedTime) + currentElapsed;
                }
            }
            ignoreGameSpeed = value;
        }
    }

    private void Start()
    {
        totalElapsed = 0;
        gameTime = GameTime.Instance;
        posTimeline.SetValue(new TimedVector3(gameTime.ElapsedTime, transform.position));
        rotationTimeline.SetValue(new TimedQuaternion(gameTime.ElapsedTime, transform.rotation));
        scaleTimeline.SetValue(new TimedVector3(gameTime.ElapsedTime, transform.localScale));
    }

    void Update()
    {
        if(IgnoreGameSpeed)
        {
            currentElapsed += Time.deltaTime * gameTime.DefaultSpeed;
        }
        
        float gameSpeed = IgnoreGameSpeed? gameTime.DefaultSpeed : gameTime.GameSpeed;
        float timestamp = IgnoreGameSpeed? ignoreStartPoint + currentElapsed : gameTime.ElapsedTime + totalElapsed;
                          
        gameTime.UpdateTimeline<TimedVector3, Vector3>(posTimeline,                      
            new TimedVector3(timestamp, transform.position),
            gameSpeed,
            timestamp);

        gameTime.UpdateTimeline<TimedQuaternion, Quaternion>(rotationTimeline,
            new TimedQuaternion(timestamp, transform.rotation), 
            gameSpeed,
            timestamp);
        
        gameTime.UpdateTimeline<TimedVector3, Vector3>(scaleTimeline,
            new TimedVector3(timestamp, transform.localScale), 
            gameSpeed,
            timestamp);

        transform.position = posTimeline.Value;
        transform.rotation = rotationTimeline.Value;
        transform.localScale = scaleTimeline.Value;
    }

    public void OnTimeElapsed(float elapsed)
    {
        if(!gameObject.activeInHierarchy)
        {
            if(elapsed < disableTime)
            {
                gameObject.SetActive(true);
            } 
            else if(GameTime.Instance.GameSpeed > 0 &&  elapsed > disableTime + GameTime.Instance.MaxDeathTime)
            {
                Destroy(gameObject);
            }
        }
    }

    private void OnEnable() 
    {
        GameTime.Instance.RemoveTimeListener(this);
    }

    private void OnDestroy()
    {
        GameTime.Instance.RemoveTimeListener(this);
    }
    
    private void OnDisable() 
    {
        disableTime = GameTime.Instance.ElapsedTime;
        GameTime.Instance.AddTimeListener(this);
    }

    
}
