using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : ScriptableObject
{
    public delegate void OnChange();
    public event OnChange OnChangeEvent;

    private float acceleration = 200;
    [SerializeField]
    private float moveSpeed = 3;
    [SerializeField]
    private float health = 3;
    
    [SerializeField]
    private float rollbackTime = 3.0f;
    [SerializeField]
    private float rollbackCount = 3;

    [SerializeField]
    private float rollbackRecoverySpeed = 0.333f;
    [SerializeField]
    private float iframeTime = 0.3f;
    [SerializeField]
    private float fireRate = 1;
    
    [SerializeField]
    private float fireAmount = 1;

    public float Acceleration => acceleration;
    public float MoveSpeed => moveSpeed;
    public float Health => health;
    public float RollbackTime => rollbackTime;
    public float RollbackCount => rollbackCount;
    public float RollbackRecoverySpeed => rollbackRecoverySpeed;
    public float iFrameTime => iframeTime;
    public float FireRate => fireRate;
    public float FireAmount => fireAmount;

    public void SetPlayerStats(PlayerStats otherStats)
    {
        SetPlayerStats(otherStats.acceleration,
                       otherStats.MoveSpeed,
                       otherStats.Health,
                       otherStats.RollbackTime,
                       otherStats.RollbackCount,
                       otherStats.RollbackRecoverySpeed,
                       otherStats.iFrameTime,otherStats.FireRate,
                       otherStats.FireAmount);
    }

    public void SetPlayerStats(float acceleration, 
                               float moveSpeed,
                               float health, 
                               float rollbackTime,
                               float rollbackCount,
                               float rollbackRecoverySpeed,
                               float iFrameTime,
                               float fireRate,
                               float  fireAmount)
    {
        this.acceleration = acceleration;
        this.moveSpeed = moveSpeed;
        this.health = health;
        this.rollbackTime = rollbackTime;
        this.rollbackCount = rollbackCount;
        this.rollbackRecoverySpeed = rollbackRecoverySpeed;
        this.iframeTime = iFrameTime;
        this.fireRate = fireRate;
        this.fireAmount = fireAmount;

        OnChangeEvent?.Invoke();
    }


    public void AddPlayerStats(PlayerStats offset, PlayerStats min, PlayerStats max)
    {
        this.moveSpeed = Mathf.Clamp(this.MoveSpeed+offset.MoveSpeed, min.MoveSpeed, max.MoveSpeed);
        this.health = Mathf.Clamp(this.Health+offset.Health, min.Health, max.Health);
        this.rollbackTime =  Mathf.Clamp(this.RollbackTime+offset.RollbackTime, min.RollbackTime, max.RollbackTime);
        this.rollbackCount = Mathf.Clamp(this.RollbackCount+offset.RollbackCount, min.RollbackCount, max.RollbackCount);
        this.rollbackRecoverySpeed = Mathf.Clamp(this.RollbackRecoverySpeed+offset.RollbackRecoverySpeed, min.RollbackRecoverySpeed, max.RollbackRecoverySpeed);
        this.iframeTime = Mathf.Clamp(this.iFrameTime+offset.iFrameTime, min.iFrameTime, max.iFrameTime);
        this.fireRate = Mathf.Clamp(this.FireRate+offset.FireRate, min.FireRate, max.FireRate);
        this.fireAmount = Mathf.Clamp(this.FireAmount+offset.FireAmount, min.FireAmount, max.FireAmount);

        OnChangeEvent?.Invoke();
    }
}
