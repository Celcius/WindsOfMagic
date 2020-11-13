using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AmoaebaUtils;

public class WaveSpawner : MonoBehaviour, IGameTimeListener
{
    private TimelinedProperty<TimedFloat, float> currentIndex = new TimelinedProperty<TimedFloat, float>();
    private TimelinedProperty<TimedBool, bool> instantiatedWave = new TimelinedProperty<TimedBool, bool>(); 
    

    public delegate void OnWillSpawnWave();
    public OnWillSpawnWave OnWillSpawnWaveEvent;

    [SerializeField]
    private EnemyWaves waves;

    [SerializeField]
    private WallScriptVar wallVar;


    [SerializeField]

    private TransformArrVar aliveEnemies;

#if UNITY_EDITOR
    [SerializeField]

    private bool canSpawn = true;
#else
    private bool canSpawn = true;
#endif

    public int CurrentWave => (int)currentIndex.Value;

    [SerializeField]
    private AnimationCurve minWavePickups;
    
    [SerializeField]
    private AnimationCurve maxWavePickups;

    [SerializeField]
    private int wavesToMaxPickups = 5;

    [SerializeField]
    private Vector2 pickupOffset;

    [SerializeField]
    private RoundPickup roundPickupPrefab;

    [SerializeField]
    private TransformArrVar currentPickups;

    [SerializeField]
    private RoundPickup healthPickup;

    [SerializeField]
    private PickupRepresentationArr normalModePickups;
    
    [SerializeField]
    private PickupRepresentationArr assistModePickups;

    [SerializeField]
    private BoolVar isAssistMode;

    [SerializeField]
    private PlayTestOptions playTest;

    float lastAngle = 0;

    public void Start()
    {
        currentIndex.Clear();
        instantiatedWave.Clear();
        SetCurrentIndexValue(0);
        SetIsInstantiatedValue(false);
        aliveEnemies.OnChange -= OnEnemyChange;
        aliveEnemies.OnChange += OnEnemyChange;
        currentPickups.OnChange -= OnPickupChange;
        currentPickups.OnChange += OnPickupChange;
        GameTime.Instance.AddTimeListener(this);
    }

    private void OnDestroy() 
    {
        aliveEnemies.OnChange -= OnEnemyChange;
        currentPickups.OnChange -= OnPickupChange;
        GameTime.Instance.RemoveTimeListener(this);
    }

    public void OnPickupChange(Transform[] oldVal, Transform[] newVal)
    {
        if(instantiatedWave.Value && oldVal.Length > 0 && newVal.Length < oldVal.Length)
        {
            foreach(Transform pickup in newVal)
            {
                if(pickup != null && pickup.gameObject.activeInHierarchy)
                {
                    pickup.gameObject.SetActive(false);
                }
            }

            SetIsInstantiatedValue(false);
        }
    }

    public void OnEnemyChange(Transform[] oldVal, Transform[] newVal)
    {
        if(oldVal.Length > 0 && newVal.Length == 0)
        {
            SetIsInstantiatedValue(false);
        }
    }

    public void OnTimeElapsed(float timeElapsed)
    {
        if(!canSpawn)
        {
            return;
        }
        if(GameTime.Instance.IsReversing)
        {
            currentIndex.InterpolatedValueAt(GameTime.Instance.ElapsedTime);
            instantiatedWave.InterpolatedValueAt(GameTime.Instance.ElapsedTime);
        }
        else if(!GameTime.Instance.IsStopped && !instantiatedWave.Value)
        {
            if(currentIndex.Value % 2 == 0)
            {
                InstantiateWave();
            }
            else
            {
                InstantiatePickups();
            }
        }
    }

    private void InstantiateWave()
    {
        SetCurrentIndexValue((int)currentIndex.Value+1);

        OnWillSpawnWaveEvent?.Invoke();

        int index = TimedBoundRandom.RandomInt(0, waves.Waves.Length);
        Transform prefab = waves.Waves[index].transform;

        for(int i = 0; i < prefab.childCount; i++)
        {
            Transform childPrefab = prefab.GetChild(i);
            Instantiate(childPrefab, childPrefab.position, childPrefab.rotation);
        }

        SetIsInstantiatedValue(true);
    }

    private void InstantiatePickups()
    {
        SetCurrentIndexValue((int)currentIndex.Value+1);

        float ratio = Mathf.Clamp01((float) CurrentWave / wavesToMaxPickups);
        int toCreate = TimedBoundRandom.RandomInt((int)minWavePickups.Evaluate(ratio),
                                                  (int)maxWavePickups.Evaluate(ratio)+1);
        toCreate = Mathf.Max(1, toCreate) + 1;

        float angle = TimedBoundRandom.RandomFloat(0,360);
        lastAngle = angle;
        float offsetInc = pickupOffset.y * 2.0f / (toCreate-1);
    
        for(int i = 0; i < toCreate; i++)
        {
            Vector2 anchorPos = Vector2.right *(wallVar.Value.Radius + pickupOffset.x) 
                                + Vector2.up * (-pickupOffset.y + offsetInc * i);
            anchorPos = MathUtils.Rotate(anchorPos, angle);

            Vector2 dir = (MathUtils.Rotate(Vector2.right, (angle + 180) % 360) - MathUtils.Rotate(Vector2.right, angle)).normalized;

            bool isHealthPickup = (i == 1);

            RoundPickup pickup = null;
            if(isHealthPickup)
            {
                if(playTest.spawnHealthOpposite)
                {
                    anchorPos += anchorPos.magnitude * dir * 2;
                    dir = -dir;
                }
                pickup = Instantiate(healthPickup, (Vector3) anchorPos, Quaternion.identity);
            }
            else
            {
                pickup = Instantiate(roundPickupPrefab, (Vector3) anchorPos , Quaternion.identity);
                PopulatePickup(pickup);
            }
            
            pickup.SetupPickup(dir);
        }

        SetIsInstantiatedValue(true);
    }

    private void SetCurrentIndexValue(float val)
    {
        currentIndex.SetValue(new TimedFloat(GameTime.Instance.ElapsedTime, val));       
    }

    private void SetIsInstantiatedValue(bool val)
    {
        instantiatedWave.SetValue(new TimedBool(GameTime.Instance.ElapsedTime, val));
    }

    private void PopulatePickup(RoundPickup pickup)
    {
        PickupRepresentationArr possiblePickups = isAssistMode.Value? assistModePickups : normalModePickups;

        int chosenIndex = TimedBoundRandom.RandomInt(0, possiblePickups.Pickups.Length);
        if(chosenIndex >= possiblePickups.Pickups.Length)
        {
            return;
        }

        PickupRepresentation representation = possiblePickups.Pickups[chosenIndex];
        if(representation.playerStats != null)
        {
            pickup.SetPlayerStats(representation.playerStats, 
                                  representation.image);
        }
        
        if(representation.projectileStats != null)
        {
            pickup.SetProjectileStats(representation.projectileStats,
                                      representation.image);
        }
        pickup.topLabel = representation.topLabel;
        pickup.botLabel = representation.botLabel;
    }

    private void OnDrawGizmos() 
    {
        if(wallVar != null && wallVar.Value != null)
        {
            Gizmos.color = Color.green;
            Vector2 mid = MathUtils.Rotate(Vector2.right *(wallVar.Value.Radius + pickupOffset.x), lastAngle);
            Vector2 bot = MathUtils.Rotate(Vector2.right *(wallVar.Value.Radius + pickupOffset.x) - Vector2.up * pickupOffset.y, lastAngle);
            Vector2 top = MathUtils.Rotate(Vector2.right *(wallVar.Value.Radius + pickupOffset.x) + Vector2.up * pickupOffset.y, lastAngle);
            
            Gizmos.DrawLine(Vector3.zero, mid);
            Gizmos.DrawLine(bot, top);
        }
    }
}
