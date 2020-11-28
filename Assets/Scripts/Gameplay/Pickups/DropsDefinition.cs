using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropsDefinition : ScriptableObject
{
    [System.Serializable]
    public struct DropDef
    {
        public Transform pickup;
        public float dropPercentage;
    }

    [SerializeField]
    private DropDef[] drops;

    private float[] dropChance;

    private float totalWeight = 0;

    [SerializeField]
    private int firstDropIndex = 0;

    [System.NonSerialized]
    private bool hasComputed = false;

    
    [System.NonSerialized]
    private static bool hasSpawned = false;

    private void OnEnable() 
    {
        hasSpawned = false;
    }

    private void Compute() 
    {
        if(hasComputed)
        {
            return;
        }

        float weight = 0;
        dropChance = new float[drops.Length];

        for(int i = 0; i < drops.Length; i++)
        {
            weight += drops[i].dropPercentage;
            dropChance[i] = weight;
        }

        totalWeight = weight;
        hasComputed = true;
    }

    public Transform GetNextRandomDrop()
    {
        Compute();
        
        if(!hasSpawned)
        {
            hasSpawned = true;  
            return drops[firstDropIndex].pickup;
        }

        float roll = TimedBoundRandom.RandomFloat(0, 1);
        for(int i = 0; i < drops.Length; i++)
        {
            if(roll <= dropChance[i])
            {
                return drops[i].pickup;
            }
        }
        return null;
    }
}
