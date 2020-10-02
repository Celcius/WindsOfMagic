using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AmoaebaUtils;

public class GameTime : SingletonScriptableObject<GameTime>
{
    [SerializeField]
    private float elapsedTime = 0;
    
    [SerializeField]
    private float gameSpeed = 1;

    [SerializeField]
    private PlayerStats maxStats;

    [SerializeField]
    private float reverseSpeed = -0.5f;

        
    [SerializeField]
    private PhysicsAnimationCurve playSpeedIn;

    [SerializeField]
    private PhysicsAnimationCurve playSpeedOut;

    [SerializeField]
    private PhysicsAnimationCurve reverseSpeedIn;

    [SerializeField]
    private PhysicsAnimationCurve reverseSpeedOut;


    private IEnumerator timeInRoutine = null;

    private CoroutineRunner runner = null;

    private List<IGameTimeListener> timeListeners = new List<IGameTimeListener>();
    private List<IGameTimeListener> toRemove = new List<IGameTimeListener>();

    public float MaxDeathTime => maxStats.RollbackTime * maxStats.RollbackCount;
    
    private bool isReversing = false;
    public bool IsReversing => isReversing;

    private bool isStopped = false;
    public bool IsStopped => isStopped;

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

    public void Start()
    {
        elapsedTime = 0.0f;
        gameSpeed = 1.0f;
        isReversing = false;
        isStopped = false;
    }
    public void Play()
    {
        isReversing = false;
        isStopped = false;
        ChangeTime(1.0f, reverseSpeedOut, playSpeedIn);
    }

    public void Reverse()
    {
        isReversing = true;
        isStopped = false;
        ChangeTime(reverseSpeed, playSpeedOut, reverseSpeedIn);
    }

    public void Stop()
    {
        isStopped = true;
        isReversing = false;
        gameSpeed = 0;
    }

    private void ChangeTime(float toValue, PhysicsAnimationCurve curveFrom, PhysicsAnimationCurve curveTo)
    {
        PrepareTimeChange();
        timeInRoutine = AnimateSpeedTo(toValue, curveFrom, curveTo);
        runner.StartCoroutine(timeInRoutine);
    }

    private IEnumerator AnimateSpeedTo(float value, PhysicsAnimationCurve curveFrom, PhysicsAnimationCurve curveTo)
    {
        float elapsed = 0.0f;
        float duration = curveFrom.TimeMultiplier;
        float startSpeed = gameSpeed;
        
        if(Mathf.Sign(value) != Mathf.Sign(gameSpeed) && gameSpeed != 0)
        {
            while(elapsed < duration)
            {
                gameSpeed = curveFrom.Evaluate(gameSpeed, duration - elapsed) * startSpeed;
                elapsed += Time.deltaTime;

                yield return new WaitForEndOfFrame();
            }  
            
            elapsed = gameSpeed = 0;
        }

        duration = curveTo.TimeMultiplier;
        
        while(elapsed < duration)
        {
            gameSpeed = curveTo.Evaluate(gameSpeed, elapsed)* value;
            elapsed += Time.deltaTime;

            yield return new WaitForEndOfFrame();
        }

        gameSpeed = value;
    }

    private void PrepareTimeChange()
    {
        if(runner == null)
        {
            runner = CoroutineRunner.Instantiate(this.name);
            DontDestroyOnLoad(runner);
        }
        if(timeInRoutine != null)
        {
            runner.StopCoroutine(timeInRoutine);
        }
    }
}
