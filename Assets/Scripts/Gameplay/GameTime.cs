using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTime : SingletonScriptableObject<GameTime>
{
    [SerializeField]
    private float elapsedTime = 0;
    
    [SerializeField]
    private float gameSpeed = 1;

    private List<IGameTimeListener> timeListeners = new List<IGameTimeListener>();
    private List<IGameTimeListener> toRemove = new List<IGameTimeListener>();
    
    public float ElapsedTime
    {
        get { return elapsedTime; }
        set 
        { 
            elapsedTime = value; 
            foreach(IGameTimeListener listener in toRemove)
            {
                timeListeners.Remove(listener);
            }
            toRemove.Clear();
            foreach(IGameTimeListener listener in timeListeners)
            {
                listener.OnTimeElapsed(elapsedTime);
            }
        }
    } 

    public float GameSpeed
    {
        get { return gameSpeed; }
        set { gameSpeed = value; }
    } 

    public float DeltaTime => (Time.deltaTime * GameSpeed);

    public void UpdateTimeline<T,V>(TimelinedProperty<T, V> timeline, T currentValue) 
        where T : TimedValue<V>
    {
        float gameSpeed = this.GameSpeed;
        float time  = this.ElapsedTime;

        if(gameSpeed > 0)
        {
            timeline.SetValue(currentValue);
        } 
        else if(gameSpeed < 0)
        {
            if(timeline.HasValueBefore(time))
            {
                timeline.UpdateValue(time, timeline.InterpolatedValueAt(time));
            }
            else if(timeline.TimePoints > 1)
            {
                timeline.RevertToStart();
            }
        }
    }

    public void AddTimeListener(IGameTimeListener listener)
    {
        if(timeListeners.Contains(listener))
        {
            return;
        }
        timeListeners.Add(listener);
        toRemove.Remove(listener);
    }

    public void RemoveTimeListener(IGameTimeListener listener)
    {
        if(timeListeners.Contains(listener))
        {
            toRemove.Add(listener);
        }
    }
}
