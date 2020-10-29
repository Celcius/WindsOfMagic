using System.Collections; 
using System.Collections.Generic;
using UnityEngine;
using AmoaebaUtils;
using System;
using UnityEngine.SceneManagement;

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

    [SerializeField]
    private BoolVar IsAssistModeOn;

    [SerializeField]
    private TransformArrVar enemies;

    [SerializeField]
    private PlayTestOptions playtest;

    [SerializeField]
    private WallScriptVar wall;

    private bool isTimeVoyaging = false;
    private bool IsTimeVoyaging
    {
        get { return isTimeVoyaging; }
        set { isTimeVoyaging = value || playtest.alwaysTimeVoyage; }
    }

    GameTimeBoundTransform timeBoundTransform;

    GameTime gameTime;

    TimedHealth health;

    void Start()
    {
        gameTime = GameTime.Instance;
        timeBoundTransform = GetComponent<GameTimeBoundTransform>();
        health = GetComponent<TimedHealth>();
        health.OnDeathEvent += OnDeath;
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

    private void OnDeath()
    {
        if(rollbackTimer.FilledRollbacks >= 1)
        {
            GameTime.Instance.Stop();
        }
        else
        {
            SceneManager.LoadScene(0,LoadSceneMode.Single);
        }
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
        if(gameTime.IsPaused)
        {
            return;
        }

        if(timeBoundTransform.IgnoreGameSpeed || (!gameTime.IsReversing && timeHandler.GameSpeed > 0 && !gameTime.IsStopped))
        {
            float delta =  (IsTimeVoyaging || timeBoundTransform.IgnoreGameSpeed)? Time.deltaTime : gameTime.DeltaTime;
            Vector2 moveDir = input.GetMoveAxis();
            Vector3 speed = (Vector3)moveDir * playerStats.MoveSpeed;
            Vector3 nextPos = transform.position + speed * delta;

            if(!IsTimeVoyaging)
            {
                shotElapsed -= delta;
                if(shotElapsed <= 0 && (input.IsShooting() || IsAssistModeOn.Value))
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

            if(gameTime.GameSpeed >= gameTime.DefaultSpeed && playtest.useTimeRecovery)
            {
                rollbackTimer.Value += delta * playerStats.RollbackRecoverySpeed;
            }
        }
    }

    void Update()
    {
        if(gameTime.IsPaused)
        {
            return;
        }

        if(timeBoundTransform.IgnoreGameSpeed && !IsTimeVoyaging && gameTime.GameSpeed >= gameTime.DefaultSpeed)
        {
            timeBoundTransform.IgnoreGameSpeed = false;
        }

        if(input.IsReversing() && !gameTime.IsReversing && rollbackTimer.FilledRollbacks >= 1.0f)
        {
            if(!IsTimeVoyaging && timeVoyageRatio.Value >= 1.0f)
            {
                StartTimeVoyage();
            }
            gameTime.Reverse();
        }
        else if((gameTime.IsReversing || gameTime.IsStopped) && !input.IsReversing() && health.IsAlive)
        {
            if(IsTimeVoyaging)
            {
                StopTimeVoyage();
            }
            rollbackTimer.Value = rollbackTimer.FilledRollbacks;
            
            gameTime.Play();
        }
        else if(gameTime.IsReversing && rollbackTimer.Value <= 0)
        {
            if(IsTimeVoyaging)
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
        Vector2 shootDir = IsAssistModeOn.Value? GetClosestEnemyDir() : input.GetShootAxis();
        if(bullet == null || Mathf.Approximately(shootDir.magnitude,0))
        {
            return;
        }
        //float angle = Vector2.Angle(Vector2.right, lastMovement.normalized);
        //angle = lastMovement.y < 0 ? - angle : angle;
        float angle = Vector2.SignedAngle(Vector2.right, shootDir.normalized);
        PlayerProjectile projectile = Instantiate(bullet,
                                                  transform.position,
                                                  Quaternion.Euler(0,0,angle));
        projectile.SetPlayerProjectileStats(speedMagnitude, projectileStats);                                                  
    }

    private void StartTimeVoyage()
    {
        gameObject.layer  =LayerMask.NameToLayer(timeVoyageMask);
        timeBoundTransform.IgnoreGameSpeed = IsTimeVoyaging = true;     
        health.ShouldRevertTime = false;          
    }

    private void StopTimeVoyage()
    {
        IsTimeVoyaging = false;
        timeVoyageRatio.Value = 0; 
        gameObject.layer = LayerMask.NameToLayer(defaultMask);
        health.ShouldRevertTime = true;
    }

    private Vector2 GetClosestEnemyDir()
    {
        if(enemies.Count() <= 0)
        {
            return Vector2.zero;
        }

        float magnitude = float.MaxValue;
        Vector2 dir = Vector2.zero;
        foreach(Transform enemy in enemies.Value)
        {
            if(wall.Value.IsOutOfBounds(enemy.position))
            {
                continue;
            }

            Vector2 tempDist = (enemy.position - transform.position);
            float tempMagnitude = tempDist.magnitude;
            if(tempMagnitude < magnitude)
            {
                magnitude = tempMagnitude;
                dir = tempDist.normalized;
            }
        }
        return dir;
    }
}

