using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AmoaebaUtils;

public class ScoreAwarder : MonoBehaviour
{

    private TimedHealth health;

    [SerializeField]
    private float score;

    [SerializeField]
    private float scoreOffset;

    [SerializeField]
    private TimelineFloatVar roundVar;

    private void Start()
    {
        health = GetComponent<TimedHealth>();
        health.OnDeathEvent += ScoreOnDeath;    
    }

    private void OnDestroy() 
    {
        health.OnDeathEvent -= ScoreOnDeath;  
    }
    private void ScoreOnDeath()
    {
        roundVar.Value += score + (int)TimedBoundRandom.RandomFloat(-scoreOffset,scoreOffset);
    }
}
