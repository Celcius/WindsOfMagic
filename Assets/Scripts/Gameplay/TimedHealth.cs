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

    private TimelinedProperty<TimedBool, bool> isInvincible = new TimelinedProperty<TimedBool, bool>();
    public bool IsInvincible => isInvincible.Value;

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

    private float lastInvincibleStart = 0;

    private bool hasStarted = false;

    private void Awake() 
    {
        spriteColor = healthSprite.color;
    }
    private void Start()
    {
        isInvincible.SetValue(new TimedBool(GameTime.Instance.ElapsedTime, false));
        SetupHealth(maxHealth, maxHealth);
        GameTime.Instance.AddTimeListener(this);
        hasStarted = true;
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
        if(delta <= 0 && IsInvincible)
        {
            return;
        }

        if(!IsInvincible && healthSprite.color != spriteColor)
        {
            healthSprite.color = spriteColor;
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
            lastInvincibleStart = GameTime.Instance.ElapsedTime;
            invincibleAnim = InvincibleRoutine();
            StartCoroutine(invincibleAnim);
        }
    }

    private void OnEnable() 
    {
        if(usePlayerIFrame || !hasStarted)
        {
            return;
        }

        healthSprite.color = spriteColor;
        invincibleAnim = InvincibleRoutine();
        StartCoroutine(invincibleAnim); 
    }

    private IEnumerator InvincibleRoutine()
    {
        while(GameTime.Instance.GameSpeed <= 0)
        {
            yield return new WaitForEndOfFrame();
        }

        if(GameTime.Instance.ElapsedTime < lastInvincibleStart || GameTime.Instance.ElapsedTime >= lastInvincibleStart + actualIFrame)
        {
            healthSprite.color = spriteColor;
            StopCoroutine(invincibleAnim);
            invincibleAnim = null;
        
            isInvincible.SetValue(new TimedBool(GameTime.Instance.ElapsedTime, false));
            yield break;
        }

        float toElapse = actualIFrame;
        float toElapseSprite = healthFrames;
        isInvincible.SetValue(new TimedBool(GameTime.Instance.ElapsedTime, true));
        
        Color healthColor = UnityEngineUtils.NegativeColor(spriteColor);
        healthSprite.color = healthColor;

        while(toElapse >= 0 && toElapse <= actualIFrame)
        {
            yield return new WaitForEndOfFrame();
            toElapse -= GameTime.Instance.DeltaTime;
            toElapseSprite -= GameTime.Instance.DeltaTime;
            
            if(toElapseSprite <= 0)
            {
                healthSprite.color = (healthSprite.color == healthColor) ? spriteColor : healthColor;
                toElapseSprite = healthFrames;
            }
            else if(toElapseSprite > healthFrames)
            {
                healthSprite.color = (healthSprite.color == healthColor) ? spriteColor : healthColor;
                toElapseSprite = toElapseSprite % healthFrames;
            }
        }

        healthSprite.color = spriteColor;
        StopCoroutine(invincibleAnim);
        invincibleAnim = null;
        
        isInvincible.SetValue(new TimedBool(GameTime.Instance.ElapsedTime, false));
    }

    public void OnTimeElapsed(float timeElapsed)
    {
        if(ShouldRevertTime && timeElapsed < healthTimeline.LastInstant)
        {
            healthTimeline.ClipDurationFromEnd(healthTimeline.LastInstant - timeElapsed, false);

            if(externalVar != null)
            {
                externalVar.Value = healthTimeline.Value;
            }
        }
    }
}
