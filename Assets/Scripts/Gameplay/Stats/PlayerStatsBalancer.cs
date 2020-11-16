using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using AmoaebaUtils;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class PlayerStatsBalancer : ScriptableObject
{
    [System.Serializable]
    public struct PlayerStatBalance
    {
        public AnimationCurve statCurve;

        public PlayerStatType type;
    }

    [SerializeField]
    private PlayerStatBalance[] statCurves;
    public PlayerStatBalance[] StatCurves
    {
        get { return statCurves; }
        set { statCurves = value; }
    }

    [SerializeField]
    private StatTierList baseStats;

    [SerializeField]
    private StatTierList currentTiers;

    [SerializeField]
    private PlayerStats minStats;
    [SerializeField]
    private PlayerStats maxStats;

    [SerializeField]
    private PlayerStats currentStats;

    private Dictionary<PlayerStatType, int> computedIndexes;

    private void OnEnable() 
    {
        ComputeIndexes();    

        if(!UnityEngineUtils.IsInPlayModeOrAboutToPlay())
        {
            return;
        }

        currentTiers.OnStatsChangeEvent -= OnStatsChanged;
    }

    private void OnDisable() 
    {
        currentTiers.OnStatsChangeEvent -= OnStatsChanged;
    }
    public void ResetStats()
    {
        currentTiers.OnStatsChangeEvent -= OnStatsChanged;
        ComputeIndexes();
        
        currentTiers.Tiers = baseStats.Tiers;
        UpdateStats();

        currentTiers.OnStatsChangeEvent += OnStatsChanged;
    }

    public void FillPlayerStats(PlayerStats stats, StatTierList.StatTier[] tiers)
    {       
        foreach(StatTierList.StatTier tier in tiers)
        {
            AnimationCurve curve = GetCurve(tier.type);
            stats.SetPlayerStat(tier.type, curve.Evaluate(tier.TierRatio()));
        }
    }

    
    private void OnStatsChanged()
    {
        UpdateStats();
    }

    private void UpdateStats()
    {
        FillPlayerStats(currentStats, currentTiers.Tiers);
        FillPlayerStats(minStats, StatTierList.GetFilledList(StatTierList.MinTier));
        FillPlayerStats(maxStats, StatTierList.GetFilledList(StatTierList.MaxTier));
    }

    private AnimationCurve GetCurve(PlayerStatType type)
    {
        if(computedIndexes == null || !computedIndexes.ContainsKey(type))
        {
            ComputeIndexes();
        }
        
        if(!computedIndexes.ContainsKey(type))
        {
            return null;
        }

        if(statCurves[computedIndexes[type]].type != type)
        {
            ComputeIndexes();
        }

        return statCurves[computedIndexes[type]].statCurve;
    }

    private void ComputeIndexes()
    {
        if(computedIndexes == null)
        {
            computedIndexes = new Dictionary<PlayerStatType, int>();
        }
        computedIndexes.Clear();

        for(int i = 0; i < statCurves.Length; i++)
        {
            computedIndexes.Add(statCurves[i].type, i);
        }
    }
}


#if UNITY_EDITOR

[CustomEditor(typeof(PlayerStatsBalancer))]
public class PlayerStatsBalancerEditor : Editor
{
    private PlayerStatsBalancer balancer;

    private void OnEnable()
    {
        balancer = (PlayerStatsBalancer)target;
    }
    
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if(balancer == null)
        {
            return;
        }

        EditorGUILayout.Space();
                
        if(GUILayout.Button("Regenerate Tiers"))
        {
            Dictionary<PlayerStatType, AnimationCurve> storedTiers = new Dictionary<PlayerStatType, AnimationCurve>();
            if(balancer.StatCurves != null && balancer.StatCurves.Length > 0)
            {
                foreach(PlayerStatsBalancer.PlayerStatBalance stat in balancer.StatCurves)
                {
                    if(storedTiers.ContainsKey(stat.type))
                    {
                        Debug.LogError("Aborting regenerating stat curves due to collision with " + stat.type); 
                        break;
                    }
                    storedTiers.Add(stat.type, stat.statCurve);
                }   
            }

            Array tierTypes = Enum.GetValues(typeof(PlayerStatType));
            PlayerStatsBalancer.PlayerStatBalance[] newStats = new PlayerStatsBalancer.PlayerStatBalance[tierTypes.Length];

            for(int i = 0; i < tierTypes.Length; i++)
            {
                PlayerStatType type = (PlayerStatType)tierTypes.GetValue(i);
                newStats[i].type = type;
                newStats[i].statCurve = storedTiers.ContainsKey(type)? storedTiers[type] : new AnimationCurve();
            }
            
            balancer.StatCurves = newStats;
        }

        if(GUILayout.Button("Update Stats"))
        {
            balancer.ResetStats();
        }
    }
}
#endif