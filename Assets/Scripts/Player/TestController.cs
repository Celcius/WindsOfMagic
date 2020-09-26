using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AmoaebaUtils;

public class TestController : MonoBehaviour
{
    private TimelinedProperty<TimedVector3, Vector3> posTimeline = new TimelinedProperty<TimedVector3, Vector3>();

    [SerializeField]
    private FloatVar gameSpeed;

    [SerializeField]
    private FloatVar gameTime;

    // Start is called before the first frame update
    void Start()
    {
        posTimeline.SetValue(new TimedVector3(gameTime.Value, transform.position));
    }

    // Update is called once per frame
    void Update()
    {
        if(gameSpeed.Value != 0)
        {
            gameTime.Value += Time.deltaTime * gameSpeed.Value;
        }
        
        if(gameSpeed.Value > 0)
        {
            posTimeline.SetValue(new TimedVector3(gameTime.Value, transform.position));
        } 
        else if(gameSpeed.Value < 0)
        {
            if(posTimeline.HasValueBefore(gameTime.Value))
            {
                transform.position = posTimeline.InterpolatedValueAt(gameTime.Value);
            }
            else
            {
                if(posTimeline.TimePoints > 1)
                {
                    posTimeline.RevertToStart();
                }
                
                transform.position = posTimeline.Value;
            }
        }
        
        Debug.Log(posTimeline.TimePoints);
    }
}
