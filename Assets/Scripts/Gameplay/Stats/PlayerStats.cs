using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class PlayerStats : ScriptableObject
{
    public delegate void OnChange();
    public event OnChange OnChangeEvent;

    public float Acceleration => GetStat(PlayerStatType.Acceleration);
    public float MoveSpeed => GetStat(PlayerStatType.MoveSpeed);
    public float Health => GetStat(PlayerStatType.Health);
    public float RollbackTime => GetStat(PlayerStatType.RollbackTime);
    public float RollbackCount => GetStat(PlayerStatType.RollbackCount);
    public float RollbackRecoverySpeed => GetStat(PlayerStatType.RollbackRecoverySpeed);
    public float iFrameTime => GetStat(PlayerStatType.iFrameTime);
    public float FireRate => GetStat(PlayerStatType.FireRate);
    public float FireAmount => GetStat(PlayerStatType.FireAmount);

    private Dictionary<PlayerStatType, float> stats;

    public float GetStat(PlayerStatType type)
    {
        if(stats == null || !stats.ContainsKey(type))
        {
            return 0;
        }
        return stats[type];
    }

    public void ClearStats()
    {
        if(stats == null)
        {
            stats = new Dictionary<PlayerStatType, float>();
        }
        stats.Clear();
    }

    public void SetPlayerStat(PlayerStatType type, float val)
    {
        if(stats == null)
        {
            stats = new Dictionary<PlayerStatType, float>();
        }
        stats[type] = val;
        
        OnChangeEvent?.Invoke();
    }


    public void SetPlayerStats(PlayerStats otherStats)
    {
        this.stats = new Dictionary<PlayerStatType, float>(otherStats.stats);
        
        OnChangeEvent?.Invoke();
    }

    public void SetPlayerStats(float acceleration, 
                               float moveSpeed,
                               float health, 
                               float rollbackTime,
                               float rollbackCount,
                               float rollbackRecoverySpeed,
                               float iFrameTime,
                               float fireRate,
                               float fireAmount, 
                               float projectileSpeed,
                               float projectileSize,
                               float projectileDamage)
    {
        stats[PlayerStatType.Acceleration] = acceleration;
        stats[PlayerStatType.MoveSpeed] = moveSpeed;
        stats[PlayerStatType.Health] = health;
        stats[PlayerStatType.RollbackTime] = rollbackTime;
        stats[PlayerStatType.RollbackCount] = rollbackCount;
        stats[PlayerStatType.RollbackRecoverySpeed] = rollbackRecoverySpeed;
        stats[PlayerStatType.iFrameTime] = iFrameTime;
        stats[PlayerStatType.FireRate] = fireRate;
        stats[PlayerStatType.FireAmount] = fireAmount;
        stats[PlayerStatType.ProjectileSpeed] = projectileSpeed;
        stats[PlayerStatType.ProjectileSize] = projectileSize;
        stats[PlayerStatType.ProjectileDamage] = projectileDamage;

        OnChangeEvent?.Invoke();
    }


    public void AddPlayerStats(PlayerStats offset, PlayerStats min, PlayerStats max)
    {
        foreach (PlayerStatType type in Enum.GetValues(typeof(PlayerStatType)))
        {
            this.SetPlayerStat(type,
                         Mathf.Clamp(offset.GetStat(type), 
                                     min.GetStat(type), 
                                     max.GetStat(type)));
        }

        OnChangeEvent?.Invoke();
    }
}



#if UNITY_EDITOR

[CustomEditor(typeof(PlayerStats))]
public class PlayerStatsEditor : Editor
{
    private PlayerStats playerStats;

    private void OnEnable()
    {
        playerStats = (PlayerStats)target;
    }
    
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if(playerStats == null)
        {
            return;
        }

        EditorGUILayout.Space();
                
        if(GUILayout.Button("Print Current Stats"))
        {
            Array tierTypes = Enum.GetValues(typeof(PlayerStatType));

            string output = "------- " + playerStats.name + " -------\n";
            foreach(PlayerStatType stat in tierTypes)
            {
                output += "[" + stat + "]  " + playerStats.GetStat(stat) + "\n"; 
            }
            Debug.Log(output);
        }
    }
}
#endif