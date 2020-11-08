using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

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
        Assert.IsFalse(health== null, "No timed health  assigned to " + this.name);
        health.OnDeathEvent -= OnDeathEvent;    
    }

    protected abstract void OnDeathEvent();
}
