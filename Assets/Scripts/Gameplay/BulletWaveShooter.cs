using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AmoaebaUtils;

public class BulletWaveShooter : MonoBehaviour
{
    private struct BulletInstanceDefinition
    {
        public Vector3 position;
        public Quaternion rotation;
    }

    [SerializeField]
    private BulletSpawnDefinition[] waves;


    [SerializeField]
    private bool repeat = true;

    [SerializeField]
    private float spawnRadiusOffset = 0;

    [SerializeField]
    private Vector2 spawnOffset = Vector2.zero;

    [SerializeField]
    private bool ignoreRotation = false;

    private GameTime timeHandler;
    private float elapsedTime = 0.0f;

    private int currentWaveIndex = 0;

    [SerializeField]
    private float playerMinDistance = float.MaxValue;

    [SerializeField]
    private TransformVar player;

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

        bool isPlayerClose = playerMinDistance == float.MaxValue 
                            || Vector2.Distance(player.Value.position, transform.position) <= playerMinDistance;

        if(elapsedTime >= waves[currentWaveIndex].timeToSpawn && isPlayerClose)
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
        BulletInstanceDefinition[] waveDefinitions = GetWaveDefinitions(wave);

        List<Transform> newProjectiles = new List<Transform>();
        foreach(BulletInstanceDefinition def in waveDefinitions)
        {
            Transform newP = Transform.Instantiate(wave.projectile, def.position, def.rotation);
            newP.gameObject.SetActive(false);
            newProjectiles.Add(newP);
            ApplyDamageOnCollision damageOnCollision = newP.GetComponent<ApplyDamageOnCollision>();
            if(damageOnCollision != null)
            {
              damageOnCollision.Damage = wave.damage;              
            }
        }

        foreach(Transform t in newProjectiles)
        {
            t.gameObject.SetActive(true);
        }
    }

    private BulletInstanceDefinition[] GetWaveDefinitions(BulletWave wave)
    {

        BulletInstanceDefinition[] ret = new BulletInstanceDefinition[wave.amount];
        float arcStart = wave.rotation;
        float arcEnd = wave.rotation + wave.range;

        for(int i = 0; i < wave.amount; i++)
        {
            float offset =  (float)i / (float)wave.amount;
            float angle = GeometryUtils.AngleInArc(arcStart, arcEnd, offset);
            angle += ignoreRotation? 0 : transform.rotation.eulerAngles.z;
            ret[i].position = GeometryUtils.PointInCircle(spawnRadiusOffset, angle);
            ret[i].position += transform.position + (Vector3)spawnOffset;

            ret[i].rotation =  Quaternion.Euler(0,0,angle);
        }
        return ret;
    }

    private void OnDrawGizmos() 
    {
        float radius = spawnRadiusOffset > 0 ? spawnRadiusOffset : 0.05f;
        Gizmos.DrawWireSphere(transform.position + (Vector3)spawnOffset, radius);    

        Color stored = Gizmos.color;

        foreach(BulletSpawnDefinition waveDef in waves)
        {
            BulletWave wave = waveDef.wave;
            Gizmos.color = wave.debugColor;
            BulletInstanceDefinition[] waveDefinitions = GetWaveDefinitions(wave);
            foreach(BulletInstanceDefinition def in waveDefinitions)
            {
                Gizmos.DrawLine(def.position, def.position + 
                                             (ignoreRotation? Vector3.right : 
                                              def.rotation * Vector3.right));
            }
        }    

        Gizmos.color = stored;
    }
}
