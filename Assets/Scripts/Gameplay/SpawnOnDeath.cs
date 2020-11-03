using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnOnDeath : OnDisableDeath
{
    [SerializeField]
    private Transform toSpawnOnDeath;
    
    [SerializeField]
    protected bool inheritRotation = true;

    [SerializeField]
    protected Vector3 scale = Vector3.one;

    protected override void OnDeathEvent() 
    {
       InstantiateEntity();
    }

    protected Transform InstantiateEntity()
    {
        if(toSpawnOnDeath == null)
        {
            return null;
        }

        Quaternion rotation = inheritRotation? transform.rotation : toSpawnOnDeath.rotation;
        Transform instance =  Instantiate(toSpawnOnDeath, transform.position, rotation);
        instance.localScale = scale;
        return instance;
    }
}
