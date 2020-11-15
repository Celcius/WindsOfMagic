using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AmoaebaUtils;

public class ScoreAwarder : MonoBehaviour
{

    private TimedHealth health;

    [SerializeField]
    private float score;
    public float Score => score;

    [SerializeField]
    private float scoreOffsetPercent;

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
        roundVar.Value += score + (int)TimedBoundRandom.RandomFloat(-scoreOffsetPercent*score,scoreOffsetPercent*score);
    }
}
