using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AmoaebaUtils;

public class PlayerStatsTimeHandler : ScriptableObject, IGameTimeListener
{
    [SerializeField]
    private PlayerStats currentPlayerStats;

    [SerializeField]
    private ProjectileStats currentProjectileStats;

    private enum StatsIndex
    {
        Start = -1,
        Acceleration = 0,
        MoveSpeed,
        Health,
        RollbackTime,
        RollbackCount,
        RollbackRecoverySpeed,
        iFrameTime,
        FireRate,
        FireAmount,
        ProjectileAddedSpeed,
        ProjectileSize,
        ProjectileDamage,
        End,
    }

    private TimelinedProperty<TimedFloat, float>[] playerStats;

    private void OnEnable() 
    {
        if(!UnityEngineUtils.IsInPlayModeOrAboutToPlay())
        {
            return;
        }
       
        GameTime.Instance.AddTimeListener(this);
        currentPlayerStats.OnChangeEvent += OnStatsChange;
        currentProjectileStats.OnChangeEvent += OnStatsChange;
        
        playerStats = new TimelinedProperty<TimedFloat, float>[(int)StatsIndex.End];
        for(int i = 0; i < playerStats.Length; i++)
        {
            playerStats[i] = new TimelinedProperty<TimedFloat, float>();
        }
    }

    private void OnDisable() 
    {
        if(!UnityEngineUtils.IsInPlayModeOrAboutToPlay())
        {
            return;
        }
        
        currentPlayerStats.OnChangeEvent -= OnStatsChange;
        currentProjectileStats.OnChangeEvent -= OnStatsChange;
        GameTime.Instance.RemoveTimeListener(this);
    }

    private void OnStatsChange()
    {
        float elapsedTime = GameTime.Instance.ElapsedTime;
        float[] currentStatsArray = CurrentStatsArray();
        for(int i = ((int)StatsIndex.Start+1); i < (int)StatsIndex.End; i++)
        {
            TimedFloat newVal = new TimedFloat(elapsedTime, currentStatsArray[i]);
            playerStats[i].SetValue(newVal);
        }
    }

    public void OnTimeElapsed(float timeElapsed)
    {
        if(GameTime.Instance.GameSpeed >= 0)
        {
            return;
        }

        currentPlayerStats.SetPlayerStats(playerStats[(int)StatsIndex.Acceleration].InterpolatedValueAt(timeElapsed),
                                          playerStats[(int)StatsIndex.MoveSpeed].InterpolatedValueAt(timeElapsed),
                                          playerStats[(int)StatsIndex.Health].InterpolatedValueAt(timeElapsed),
                                          playerStats[(int)StatsIndex.RollbackTime].InterpolatedValueAt(timeElapsed),
                                          playerStats[(int)StatsIndex.RollbackCount].InterpolatedValueAt(timeElapsed),
                                          playerStats[(int)StatsIndex.RollbackRecoverySpeed].InterpolatedValueAt(timeElapsed),
                                          playerStats[(int)StatsIndex.iFrameTime].InterpolatedValueAt(timeElapsed),
                                          playerStats[(int)StatsIndex.FireRate].InterpolatedValueAt(timeElapsed),
                                          playerStats[(int)StatsIndex.FireAmount].InterpolatedValueAt(timeElapsed),
                                          playerStats[(int)StatsIndex.ProjectileAddedSpeed].InterpolatedValueAt(timeElapsed),
                                          playerStats[(int)StatsIndex.ProjectileSize].InterpolatedValueAt(timeElapsed),
                                          playerStats[(int)StatsIndex.ProjectileDamage].InterpolatedValueAt(timeElapsed));
    }

    private float[] CurrentStatsArray()
    {
        float[] stats = new float[playerStats.Length];
        stats[(int)StatsIndex.Acceleration] = currentPlayerStats.Acceleration;
        stats[(int)StatsIndex.MoveSpeed] = currentPlayerStats.MoveSpeed;
        stats[(int)StatsIndex.Health] = currentPlayerStats.Health;
        stats[(int)StatsIndex.RollbackTime] = currentPlayerStats.RollbackTime;
        stats[(int)StatsIndex.RollbackCount] = currentPlayerStats.RollbackCount;
        stats[(int)StatsIndex.RollbackRecoverySpeed] = currentPlayerStats.RollbackRecoverySpeed;
        stats[(int)StatsIndex.iFrameTime] = currentPlayerStats.iFrameTime;
        stats[(int)StatsIndex.FireRate] = currentPlayerStats.FireRate;
        stats[(int)StatsIndex.FireAmount] = currentPlayerStats.FireAmount;
        stats[(int)StatsIndex.ProjectileAddedSpeed] = currentProjectileStats.AddedSpeed;
        stats[(int)StatsIndex.ProjectileSize] = currentProjectileStats.Size;
        stats[(int)StatsIndex.ProjectileDamage] = currentProjectileStats.Damage;
        return stats;
    }
}
