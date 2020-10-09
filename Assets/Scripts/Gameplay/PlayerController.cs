using System.Collections; 
using System.Collections.Generic;
using UnityEngine;
using AmoaebaUtils;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(GameTimeBoundTransform))]
[RequireComponent(typeof(TimedHealth))]
public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private InputHandler input;

    private Rigidbody2D body;
    private GameTime timeHandler;

    [SerializeField]
    private PlayerStats playerStats;

    [SerializeField]
    private ProjectileStats projectileStats;

    [SerializeField]
    private PlayerProjectile bullet;

    [SerializeField]
    private string timeVoyageMask;
    private string defaultMask;


    [SerializeField]
    private float deadzone = 0.05f;

    private float shotElapsed = 0.0f;

    private Vector2 lastMovement = Vector2.right;

    [SerializeField]
    private RollbackTimer rollbackTimer;

    [SerializeField]
    private FloatVar timeVoyageRatio;
    private bool isTimeVoyaging = false;

    GameTimeBoundTransform timeBoundTransform;

    GameTime gameTime;

    TimedHealth health;

    void Start()
    {
        gameTime = GameTime.Instance;
        timeBoundTransform = GetComponent<GameTimeBoundTransform>();
        health = GetComponent<TimedHealth>();
        isTimeVoyaging = false;
        timeBoundTransform.IgnoreGameSpeed = false;
        timeVoyageRatio.Value = 0;
        body = GetComponent<Rigidbody2D>();
        timeHandler = gameTime;
        shotElapsed = 0.0f;
        defaultMask = LayerMask.LayerToName(gameObject.layer);

        playerStats.OnChangeEvent += OnStatsChanged;
        OnStatsChanged();
    }

    private void OnStatsChanged()
    {
        if(health.CurrentMaxHealth != playerStats.Health)
        {
            float delta = playerStats.Health - health.CurrentMaxHealth;
            delta = delta < 0 ? 0 : delta;
            health.SetupHealth(health.Health + delta, playerStats.Health, false);
        }
    }

    private void OnDestroy() {
        playerStats.OnChangeEvent -= OnStatsChanged;
    }

    void FixedUpdate()
    {
        if(timeBoundTransform.IgnoreGameSpeed || (!gameTime.IsReversing && timeHandler.GameSpeed > 0 && !gameTime.IsStopped))
        {
            float delta =  (isTimeVoyaging || timeBoundTransform.IgnoreGameSpeed)? Time.deltaTime : gameTime.DeltaTime;
            Vector2 moveDir = input.GetMoveAxis();
            Vector3 speed = (Vector3)moveDir * playerStats.MoveSpeed;
            Vector3 nextPos = transform.position + speed * delta;

            if(!isTimeVoyaging)
            {
                shotElapsed -= delta;
                if(shotElapsed <= 0 && input.IsShooting() && moveDir.magnitude > deadzone)
                {
                    Shoot(speed.magnitude);
                    shotElapsed = playerStats.FireRate;
                }                  
            }

           body.MovePosition(nextPos);
           transform.right = transform.right + (transform.position-nextPos).normalized* GameTime.Instance.DeltaTime;

            moveDir = moveDir.normalized;
            if(!Mathf.Approximately(moveDir.x, 0) || !Mathf.Approximately(moveDir.y,0))
            {
                lastMovement = new Vector2(moveDir.x, moveDir.y);
            }

            if(gameTime.GameSpeed >= gameTime.DefaultSpeed)
            {
                rollbackTimer.Value += delta * playerStats.RollbackRecoverySpeed;
            }
        }
    }

    void Update()
    {
        
        if(timeBoundTransform.IgnoreGameSpeed && !isTimeVoyaging && gameTime.GameSpeed >= gameTime.DefaultSpeed)
        {
            timeBoundTransform.IgnoreGameSpeed = false;
        }

        if(input.IsReversing() && !gameTime.IsReversing && rollbackTimer.FilledRollbacks >= 1.0f)
        {
            if(!isTimeVoyaging && timeVoyageRatio.Value >= 1.0f)
            {
                StartTimeVoyage();
            }
            gameTime.Reverse();
        }
        else if((gameTime.IsReversing || gameTime.IsStopped) && !input.IsReversing())
        {
            if(isTimeVoyaging)
            {
                StopTimeVoyage();
            }
            rollbackTimer.Value = rollbackTimer.FilledRollbacks;
            gameTime.Play();
        }
        else if(gameTime.IsReversing && rollbackTimer.Value <= 0)
        {
            if(isTimeVoyaging)
            {
                StopTimeVoyage();
                rollbackTimer.Value = 0;
                gameTime.Play();
            }
            else
            {
                gameTime.Stop();
            }
        }
        else if(gameTime.GameSpeed < 0 && input.IsReversing())
        {
            rollbackTimer.Value += gameTime.DeltaTime;
        }
    }

    private void Shoot(float speedMagnitude)
    {
        if(bullet == null)
        {
            return;
        }
        float angle = Vector2.Angle(Vector2.right, lastMovement.normalized);
        angle = lastMovement.y < 0 ? - angle : angle;
        PlayerProjectile projectile = Instantiate(bullet,
                                                  transform.position,
                                                  Quaternion.Euler(0,0,angle));
        projectile.SetPlayerProjectileStats(speedMagnitude, projectileStats);                                                  
    }

    private void StartTimeVoyage()
    {
        gameObject.layer  =LayerMask.NameToLayer(timeVoyageMask);
        timeBoundTransform.IgnoreGameSpeed = isTimeVoyaging = true;     
        health.ShouldRevertTime = false;          
    }

    private void StopTimeVoyage()
    {
        isTimeVoyaging = false;
        timeVoyageRatio.Value = 0; 
        gameObject.layer = LayerMask.NameToLayer(defaultMask);
        health.ShouldRevertTime = true;
    }
}

