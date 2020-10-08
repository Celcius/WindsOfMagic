using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AmoaebaUtils;

public class DisableAndRespawn : DisableOnCollision
{
    private static CoroutineRunner runner;
    [SerializeField]
    private float timeToRespawn = 0.0f;
    
    [SerializeField]
    private bool spawnOnInitialPos = true;

    private Vector3 spawnPos;

    private IEnumerator spawnRoutine = null;
    
    private void Start() 
    {
        spawnPos = transform.position;
        if(runner == null)
        {
            runner = CoroutineRunner.Instantiate("DisableAndRespawnHelper");
        }
    }


    protected override void Apply(Transform other)
    {
        spawnPos = spawnOnInitialPos? spawnPos : transform.position;
        
        if(spawnRoutine != null)
        {
            runner.StopCoroutine(spawnRoutine);
        }

        base.Apply(other);

        if(timeToRespawn == 0)
        {
            Respawn();
        }
        else
        {
           spawnRoutine = toSpawnRoutine();
           runner.StartCoroutine(spawnRoutine);
        }
   
    } 
    
    private IEnumerator toSpawnRoutine()
    {
        yield return new WaitForSeconds(timeToRespawn);
        Respawn();
    }

    private void Respawn()
    {
        transform.position = spawnPos;
        gameObject.SetActive(true);
    }
    
}
