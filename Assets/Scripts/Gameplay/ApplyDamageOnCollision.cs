using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AmoaebaUtils;

public class ApplyDamageOnCollision : ApplyOnCollision
{
    [SerializeField]
    private float damage = 0.0f;

    public float Damage 
    {
        get { return damage; }
        set { damage = value; }
    }

    protected override void Apply(Transform other)
    {
        if(GameTime.Instance.GameSpeed <= 0)
        {
            return;
        }
        TimedHealth health = other.GetComponent<TimedHealth>();
        if(health == null)
        {
            return;
        }

        health.SetHealthDelta(-damage);
    }
}
