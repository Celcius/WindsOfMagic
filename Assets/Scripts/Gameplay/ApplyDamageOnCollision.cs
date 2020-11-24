using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AmoaebaUtils;

public class ApplyDamageOnCollision : ApplyOnCollision
{
    [SerializeField]
    private float damage = 0.0f;

    [SerializeField]
    private bool applyOnOther =  true;

    [SerializeField]
    protected Transform applyTransform;

    public Transform TargetTransform => (applyTransform != null? applyTransform : transform);

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

        Transform target = (applyOnOther? other : TargetTransform);
        
        TimedHealth health = target.GetComponent<TimedHealth>();
        if(health == null)
        {
            ApplyDamageOnCollision collision = target.GetComponent<ApplyDamageOnCollision>();
            if(collision == null)
            {
                return;
            }
            health = collision.TargetTransform.GetComponent<TimedHealth>();
        }

        if(health != null)
        {
            health.SetHealthDelta(-damage);
        }
    }
}
