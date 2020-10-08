using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AmoaebaUtils;

public class BulletWaveShooter : MonoBehaviour
{

    [SerializeField]
    private BulletSpawnDefinition[] waves;


    [SerializeField]
    private bool repeat = true;

    [SerializeField]
    private float spawnOffset = 0;

    private GameTime timeHandler;
    private float elapsedTime = 0.0f;

    private int currentWaveIndex = 0;

    private void Start() 
    {
        timeHandler = GameTime.Instance;
    }

    private void Update()
    {   
        if(waves.Length == 0 || (!repeat && currentWaveIndex >= waves.Length))
        {
            return;
        }

        elapsedTime =  Mathf.Clamp(elapsedTime + timeHandler.DeltaTime, 
                                   0, 
                                   waves[waves.Length-1].timeToSpawn);

        if(timeHandler.GameSpeed  <= 0)
        {
            while(currentWaveIndex > 0 && elapsedTime >= waves[currentWaveIndex].timeToSpawn)
            {
                --currentWaveIndex;
            }
            return;
        }

        if(elapsedTime >= waves[currentWaveIndex].timeToSpawn)
        {
            BulletSpawnDefinition definition = waves[currentWaveIndex];
            SpawnWave(definition.wave);

            ++currentWaveIndex;
            if(repeat && currentWaveIndex >= waves.Length)
            {
                elapsedTime = 0;
                currentWaveIndex = 0;
            }
        }
    }

    private void SpawnWave(BulletWave wave)
    {
        float arcStart = wave.rotation;
        float arcEnd = wave.rotation + wave.range;
        for(int i = 0; i < wave.amount; i++)
        {
            float offset =  (float)i / (float)wave.amount;
            float angle = GeometryUtils.AngleInArc(arcStart, arcEnd, offset);
            Vector3 pos = GeometryUtils.PointInCircle(spawnOffset,angle);
        
            Transform newP = Transform.Instantiate(wave.projectile, transform.position + pos, Quaternion.Euler(0,0,angle));
            ApplyDamageOnCollision damageOnCollision = newP.GetComponent<ApplyDamageOnCollision>();
            if(damageOnCollision != null)
            {
              damageOnCollision.Damage = wave.damage;              
            }
        }
    }
}
