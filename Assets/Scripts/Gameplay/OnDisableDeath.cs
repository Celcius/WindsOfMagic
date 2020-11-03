using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TimedHealth))]
public abstract class OnDisableDeath : MonoBehaviour
{
    protected TimedHealth health;

    protected virtual void Start()
    {
        health = GetComponent<TimedHealth>();
        health.OnDeathEvent += OnDeathEvent;
    }

    protected virtual void OnDestroy() 
    {
        health.OnDeathEvent -= OnDeathEvent;    
    }

    protected abstract void OnDeathEvent();
}
