using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using AmoaebaUtils;

public class TimedHealth : MonoBehaviour, IGameTimeListener
{
    public delegate void OnDeath();
    public event OnDeath OnDeathEvent;

    [SerializeField]
    private float maxHealth = 0.0f;
    public float CurrentMaxHealth 
    {
        get { return maxHealth; }
        set { maxHealth = value; }
    }

    public bool IsAlive => Health > 0;

    [SerializeField]
    private bool disableOnDeath = true;

    private TimelinedProperty<TimedFloat, float> healthTimeline = new TimelinedProperty<TimedFloat, float>();
    public float Health => healthTimeline.Value;

    [SerializeField]
    private FloatVar externalVar = null;

    [SerializeField]
    public bool ShouldRevertTime = true;

    [SerializeField]
    private bool isInvincible = false;
    public bool IsInvincible => isInvincible;

    [SerializeField]
    public SpriteRenderer healthSprite;

    [SerializeField]
    private float iFrame = 0.1f;

    private float healthFrames = 0.05f;

    private float actualIFrame => usePlayerIFrame? currentPlayerStats.iFrameTime : iFrame;

    [SerializeField]
    private bool usePlayerIFrame = false;

    [SerializeField]
    private PlayerStats currentPlayerStats;

    private IEnumerator invincibleAnim;

    private Color spriteColor;

    private void Start()
    {
        SetupHealth(maxHealth, maxHealth);
        GameTime.Instance.AddTimeListener(this);
        spriteColor = healthSprite.color;
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
        
        float delta = currentHealth - this.healthTimeline.Value;
        SetHealthDelta(delta);
    }

    public void SetHealthDelta(float delta)
    {
        if(delta <= 0 && isInvincible)
        {
            return;
        }

        float current = Mathf.Clamp(healthTimeline.Value + delta, 0, maxHealth);
        healthTimeline.SetValue(new TimedFloat(GameTime.Instance.ElapsedTime, current));
        
        if(externalVar != null)
        {
            externalVar.Value = healthTimeline.Value;
        }

        if(current == 0)
        {
            OnDeathEvent?.Invoke();
            if(disableOnDeath)
            {
                gameObject.SetActive(false);
            }
        }
        else if(delta < 0 && actualIFrame > 0 && healthSprite != null && gameObject.activeInHierarchy)
        {
            if(invincibleAnim != null)
            {
                StopCoroutine(invincibleAnim);
            }
            invincibleAnim = InvincibleRoutine();
            StartCoroutine(invincibleAnim);
        }
    }

    private IEnumerator InvincibleRoutine()
    {
        float toElapse = actualIFrame;
        float toElapseSprite = healthFrames;
        isInvincible = true;
        
        Color healthColor = UnityEngineUtils.NegativeColor(spriteColor);
        healthSprite.color = healthColor;

        while(toElapse >= 0)
        {
            yield return new WaitForEndOfFrame();
            toElapse -= Time.deltaTime;
            toElapseSprite -= Time.deltaTime;
            
            if(toElapseSprite <= 0)
            {
                healthSprite.color = (healthSprite.color == healthColor) ? spriteColor : healthColor;
                toElapseSprite = healthFrames;    
            }
        }

        healthSprite.color = spriteColor;
        invincibleAnim = null;
        isInvincible = false;
    }

    public void OnTimeElapsed(float timeElapsed)
    {
        if(ShouldRevertTime && timeElapsed < healthTimeline.LastInstant)
        {
            healthTimeline.ClipDurationFromEnd(timeElapsed, false);

            if(externalVar != null)
            {
                externalVar.Value = healthTimeline.Value;
            }
        }
    }
}
