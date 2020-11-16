using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileStats : ScriptableObject
{
    public delegate void OnChange();
    public event OnChange OnChangeEvent;

    public float AddedSpeed = 3;
    public float Size = 0.5f;
    public float Damage = 1;

    public void AddProjectileStats(ProjectileStats offset, ProjectileStats min, ProjectileStats max)
    {
        this.AddedSpeed = Mathf.Clamp(this.AddedSpeed+offset.AddedSpeed, min.AddedSpeed, max.AddedSpeed);
        this.Size = Mathf.Clamp(this.Size+offset.Size, min.Size, max.Size);
        this.Damage = Mathf.Clamp(this.Damage+offset.Damage, min.Damage, max.Damage);
        
        OnChangeEvent?.Invoke();
    }

    public void SetProjectileStats(ProjectileStats other)
    {
        SetProjectileStats(other.AddedSpeed,
                           other.Size,
                           other.Damage);
    }

    public void SetProjectileStats(PlayerStats other)
    {
        SetProjectileStats(other.GetStat(PlayerStatType.ProjectileSpeed),
                           other.GetStat(PlayerStatType.ProjectileSize),
                           other.GetStat(PlayerStatType.ProjectileDamage));
    }


    public void SetProjectileStats(float addedSpeed, float size, float damage)
    {
        this.AddedSpeed = addedSpeed;
        this.Size = size;
        this.Damage = damage;
        
        OnChangeEvent?.Invoke();
    }
}
