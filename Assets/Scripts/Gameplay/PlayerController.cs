using System.Collections; 
using System.Collections.Generic;
using UnityEngine;
using AmoaebaUtils;

[RequireComponent(typeof(Rigidbody2D))]
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
    private float deadzone = 0.05f;

    private float shotElapsed = 0.0f;

    private Vector2 lastMovement = Vector2.right;

    [SerializeField]
    private RollbackTimer rollbackTimer;

    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        timeHandler = GameTime.Instance;
        shotElapsed = 0.0f;
    }

    void Update()
    {
        if(input.IsReversing() && !GameTime.Instance.IsReversing && rollbackTimer.FilledRollbacks >= 1.0f)
        {
            GameTime.Instance.Reverse();
        }
        else if((GameTime.Instance.IsReversing || GameTime.Instance.IsStopped) && !input.IsReversing())
        {
            rollbackTimer.Value = rollbackTimer.FilledRollbacks;
            GameTime.Instance.Play();
        }
        else if(GameTime.Instance.IsReversing && rollbackTimer.Value <= 0)
        {
            GameTime.Instance.Stop();
        }
        else if(GameTime.Instance.GameSpeed < 0 && input.IsReversing())
        {
            rollbackTimer.Value += GameTime.Instance.DeltaTime;
        }
        else if(timeHandler.GameSpeed > 0)
        {
            Vector2 moveDir = input.GetMoveAxis();
            Vector3 speed = (Vector3)moveDir * playerStats.MoveSpeed;
            Vector3 nextPos = transform.position + speed * timeHandler.DeltaTime;

            shotElapsed -= timeHandler.DeltaTime;
            if(shotElapsed <= 0 && input.IsShooting() && moveDir.magnitude > deadzone)
            {
                Shoot(speed.magnitude);
                shotElapsed = playerStats.FireRate;
            }

            body.MovePosition(nextPos);

            moveDir = moveDir.normalized;
            if(!Mathf.Approximately(moveDir.x, 0) || !Mathf.Approximately(moveDir.y,0))
            {
                lastMovement = new Vector2(moveDir.x, moveDir.y);
            }

            rollbackTimer.Value += GameTime.Instance.DeltaTime * playerStats.RollbackRecoverySpeed;
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
}

