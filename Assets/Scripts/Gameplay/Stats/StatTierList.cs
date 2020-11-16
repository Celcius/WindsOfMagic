using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class StatTierList : ScriptableObject
{
    public delegate void OnStatsChange();

    public event OnStatsChange OnStatsChangeEvent;

    public static int MinTier = 1;
    public static int MaxTier = 5;

    [System.Serializable]
    public struct StatTier
    {
        public PlayerStatType type;
        public int tier;

        public float TierRatio()
        {
            return (float)(StatTierList.ClampTier(tier) - StatTierList.MinTier)/(float)(StatTierList.MaxTier - StatTierList.MinTier);
        }
    }

    [SerializeField]
    private StatTier[] tiers;

    public StatTier[] Tiers 
    {
        get { return tiers; }
        set 
        { 
            tiers = new StatTier[value.Length];
            for(int i = 0; i < value.Length; i++)
            {
                tiers[i].tier = value[i].tier;
                tiers[i].type = value[i].type;
            }
            ComputeIndexes();
            OnStatsChangeEvent?.Invoke();
        }
    }

    private Dictionary<PlayerStatType, int> indexesHelper = null;

    private void ComputeIndexes()
    {
        if(indexesHelper == null)
        {
            indexesHelper = new Dictionary<PlayerStatType, int>();
        }
        indexesHelper.Clear();
        
        for(int i = 0;i < tiers.Length; i++)
        {
            indexesHelper[tiers[i].type] = i;
        }
    }

    public void IncrementTier(PlayerStatType type)
    {
        int nextTier = GetTier(type) + 1;
        SetTier(type, nextTier);
    }

    public void DecrementTier(PlayerStatType type)
    {
        int nextTier = GetTier(type) - 1;
        SetTier(type, nextTier);
    }

    public int GetTier(PlayerStatType type)
    {
        if(indexesHelper == null || !indexesHelper.ContainsKey(type))
        {
            ComputeIndexes();
        }

        int index = indexesHelper[type];
        if(tiers[index].type != type)
        {
            ComputeIndexes();
            index = indexesHelper[type];
        }

        return tiers[index].tier;
    }

    public void SetTier(PlayerStatType type, int tier)
    {
        tier = ClampTier(tier);

        if(indexesHelper == null || !indexesHelper.ContainsKey(type))
        {
            ComputeIndexes();
        }

        int index = indexesHelper[type];
        tiers[index].tier = tier;

        OnStatsChangeEvent?.Invoke();
    }

    public static StatTier[] GetFilledList(int tier)
    {
        tier = ClampTier(tier);

        Array tierTypes = Enum.GetValues(typeof(PlayerStatType));
        StatTier[] newTiers = new StatTierList.StatTier[tierTypes.Length];

        for(int i = 0; i < tierTypes.Length; i++)
        {
            PlayerStatType type = (PlayerStatType)tierTypes.GetValue(i);
            newTiers[i].type = type;
            newTiers[i].tier = tier;
        }
        return newTiers;
    }

    public static int ClampTier(int tier)
    {
        return Mathf.Clamp(tier, MinTier, MaxTier);
    }
}


#if UNITY_EDITOR

[CustomEditor(typeof(StatTierList))]
public class StatTierListEditor : Editor
{
    private StatTierList statTier;

    private void OnEnable()
    {
        statTier = (StatTierList)target;
    }
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if(statTier == null)
        {
            return;
        }

        EditorGUILayout.Space();
                
        if(GUILayout.Button("Regenerate Tiers"))
        {
            Dictionary<PlayerStatType, int> storedTiers = new Dictionary<PlayerStatType, int>();
            if(statTier.Tiers != null && statTier.Tiers.Length > 0)
            {
                foreach(StatTierList.StatTier tier in statTier.Tiers)
                {
                    if(storedTiers.ContainsKey(tier.type))
                    {
                        Debug.LogError("Aborting regenerating stat tiers due to collision with " + tier.type); 
                        break;
                    }
                    storedTiers.Add(tier.type, tier.tier);
                }   
            }

            Array tierTypes = Enum.GetValues(typeof(PlayerStatType));
            StatTierList.StatTier[] newTiers = new StatTierList.StatTier[tierTypes.Length];

            for(int i = 0; i < tierTypes.Length; i++)
            {
                PlayerStatType type = (PlayerStatType)tierTypes.GetValue(i);
                newTiers[i].type = type;
                newTiers[i].tier = storedTiers.ContainsKey(type)? storedTiers[type] : 0;
            }
            
            statTier.Tiers = newTiers;
        }
    }
}
#endif