using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class TimedHealth : MonoBehaviour, IGameTimeListener
{
    [SerializeField]
    private float maxHealth = 0.0f;
    public float CurrentMaxHealth 
    {
        get { return maxHealth; }
        set { maxHealth = value; }
    }

    [SerializeField]
    private bool disableOnDeath = true;

    private TimelinedProperty<TimedFloat, float> healthTimeline = new TimelinedProperty<TimedFloat, float>();
    public float Health => healthTimeline.Value;

    [SerializeField]
    private FloatVar externalVar = null;

    [SerializeField]
    public bool ShouldRevertTime = true;

    private void Start()
    {
        SetupHealth(maxHealth, maxHealth);
        GameTime.Instance.AddTimeListener(this);
    }

    private void OnDestroy()
    {
        GameTime.Instance.RemoveTimeListener(this);
    }

    public void SetupHealth(float currentHealth, float maxHealth, bool clearPast = true)
    {
        Assert.IsFalse(maxHealth == 0.0f, $"Creating {this} with empty Health");
        float inc = Mathf.Max(0, maxHealth - this.maxHealth);
        this.maxHealth = maxHealth;
        currentHealth = Mathf.Clamp(currentHealth + inc, 0, maxHealth);
        if(clearPast)
        {
            healthTimeline.Clear();    
        }
        
        SetHealthDelta(currentHealth - this.healthTimeline.Value);
    }

    public void SetHealthDelta(float delta)
    {
        float current = Mathf.Clamp(healthTimeline.Value + delta, 0, maxHealth);
        healthTimeline.SetValue(new TimedFloat(GameTime.Instance.ElapsedTime, current));
        
        if(externalVar != null)
        {
            externalVar.Value = healthTimeline.Value;
        }

        if(current == 0 && disableOnDeath)
        {
            gameObject.SetActive(false);
        }
    }

    public void OnTimeElapsed(float timeElapsed)
    {
        if(ShouldRevertTime && timeElapsed <= healthTimeline.LastInstant)
        {
            healthTimeline.ClipDurationFromEnd(healthTimeline.LastInstant - timeElapsed);

            if(externalVar != null)
            {
                externalVar.Value = healthTimeline.Value;
            }
        }
    }


}
