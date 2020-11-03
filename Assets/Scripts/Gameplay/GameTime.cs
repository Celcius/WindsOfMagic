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

    
    [SerializeField]
    private BoolVar isPaused;

    [SerializeField]
    private BoolVar IsPlayingNormal;

    private float defaultSpeed = 1.0f;
    public float DefaultSpeed => defaultSpeed;

    public delegate void OnSpeedChange();
    public event OnSpeedChange OnSpeedChangeEvent;



    private IEnumerator timeInRoutine = null;

    private CoroutineRunner runner = null;

    private List<IGameTimeListener> timeListeners = new List<IGameTimeListener>();
    private List<IGameTimeListener> toRemove = new List<IGameTimeListener>();

    public float MaxDeathTime => maxStats.RollbackTime * maxStats.RollbackCount+0.5f;
    
    private bool isReversing = false;
    public bool IsReversing => isReversing;

    private bool isStopped = false;
    public bool IsStopped => isStopped;

    public bool IsPaused => isPaused.Value;
    private float storedPauseSpeed = 0;

    private float speedModifier = 1.0f;
    public float SpeedModifier
    {
        get { return speedModifier; }
        set { speedModifier = value; }
    }

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
        get { return gameSpeed * speedModifier; }
        set 
        { 
            gameSpeed = value;  
            OnSpeedChangeEvent?.Invoke(); 
        }
    } 



    public float DeltaTime => (Time.deltaTime * GameSpeed);

    public void UpdateTimeline<T,V>(TimelinedProperty<T, V> timeline, T currentValue)
        where T : TimedValue<V>
    {
        UpdateTimeline<T,V>(timeline, currentValue, this.gameSpeed, this.ElapsedTime);
    }

    public void UpdateTimeline<T,V>(TimelinedProperty<T, V> timeline, T currentValue, float gameSpeed, float time) 
        where T : TimedValue<V>
    {
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
        gameSpeed = defaultSpeed;
        isReversing = false;
        isStopped = false;
        IsPlayingNormal.Value = true;

        isPaused.OnChange += PauseChange;
        PauseChange(false, isPaused.Value);
    }

    private void OnDisable() 
    {
        isPaused.OnChange -= PauseChange;
    }

    private void PauseChange(bool oldVal, bool newVal)
    {
        if(oldVal == newVal)
        {
            return;
        }

        if(newVal)
        {
            storedPauseSpeed = gameSpeed;
            GameSpeed = 0;
        }
        else
        {
            GameSpeed = storedPauseSpeed;
        }
    }

    public void Play()
    {
        isReversing = false;
        isStopped = false;
        IsPlayingNormal.Value = true;
        ChangeTime(defaultSpeed, reverseSpeedOut, playSpeedIn);
    }

    public void Reverse()
    {
        isReversing = true;
        isStopped = false;
        IsPlayingNormal.Value = false;
        ChangeTime(reverseSpeed, playSpeedOut, reverseSpeedIn);
    }

    public void Stop()
    {
        if(runner != null && timeInRoutine != null)
        {
            runner.StopCoroutine(timeInRoutine);            
        }

        IsPlayingNormal.Value = false;
        isStopped = true;
        isReversing = false;
        gameSpeed = 0;
        OnSpeedChangeEvent?.Invoke(); 
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
                
                OnSpeedChangeEvent?.Invoke(); 

                yield return new WaitForEndOfFrame();
            }  
            
            elapsed = gameSpeed = 0;
        }

        duration = curveTo.TimeMultiplier;
        
        while(elapsed < duration)
        {
            gameSpeed = curveTo.Evaluate(gameSpeed, elapsed)* value;
            elapsed += Time.deltaTime;
            OnSpeedChangeEvent?.Invoke(); 
            yield return new WaitForEndOfFrame();
        }

        gameSpeed = value;
        OnSpeedChangeEvent?.Invoke(); 
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
