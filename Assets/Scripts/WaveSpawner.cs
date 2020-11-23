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
    private float instantiateDistance;


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
    private int wavePickupAmounts = 2;

    [SerializeField]
    private AnimationCurve pickupSpeed;

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

    [SerializeField]
    private DifficultyCalculator difficultySpawner;

    [SerializeField]
    private FloatVar currentWaveVar;

    float lastAngle = 0;

    public void Start()
    {
        Reset();
        aliveEnemies.OnChange -= OnEnemyChange;
        aliveEnemies.OnChange += OnEnemyChange;
        currentPickups.OnChange -= OnPickupChange;
        currentPickups.OnChange += OnPickupChange;
        GameTime.Instance.AddTimeListener(this);
    }

    public void Reset()
    {
        currentIndex.Clear();
        instantiatedWave.Clear();
        SetCurrentIndexValue(0);
        SetIsInstantiatedValue(false);
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
        
        if(GameTime.Instance.GameSpeed > 0 && !GameTime.Instance.IsReversing)
        {
            if(currentIndex.LastInstant > timeElapsed)
            {
                currentIndex.ClipDurationFromEnd(timeElapsed - currentIndex.FirstInstant, false);
            }

            if(instantiatedWave.LastInstant > timeElapsed)
            {
                instantiatedWave.ClipDurationFromEnd(timeElapsed - instantiatedWave.FirstInstant, false);
            }

            if(!instantiatedWave.Value)
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
    }

    private void InstantiateWave()
    {
        SetCurrentIndexValue((int)currentIndex.Value+1);
        currentWaveVar.Value = Mathf.CeilToInt(currentIndex.Value/2.0f);
        OnWillSpawnWaveEvent?.Invoke();
        
        if(playTest.difficultySpawn)
        {
            InstantiateByDifficulty();
        }
        else
        {
            InstantiateFromWaveList();
        }
        

        SetIsInstantiatedValue(true);
    }

    private void InstantiateFromWaveList()
    {
        int index = TimedBoundRandom.RandomInt(0, waves.Waves.Length);
        Transform prefab = waves.Waves[index].transform;

        for(int i = 0; i < prefab.childCount; i++)
        {
            Transform childPrefab = prefab.GetChild(i);
            Instantiate(childPrefab, childPrefab.position, childPrefab.rotation);
        }
    }

    private void InstantiateByDifficulty()
    {
        List<ScoreAwarder> wave = difficultySpawner.GetNewDifficultyTierWave((int)currentWaveVar.Value);
        foreach(ScoreAwarder awarder in wave)
        {
            InstantiateOnDistance(awarder.transform);
        }
    }

    private void InstantiateOnDistance(Transform t)
    {
        Vector3 pos = GeometryUtils.PointInCircle(instantiateDistance, TimedBoundRandom.RandomFloat(0,360));
        Instantiate(t, pos, Quaternion.Euler(0,0, TimedBoundRandom.RandomFloat(0,360)));
    }

    private void InstantiatePickups()
    {
        SetCurrentIndexValue((int)currentIndex.Value+1);

        int toCreate = wavePickupAmounts + 1;

        float angle = TimedBoundRandom.RandomFloat(0,360);
        lastAngle = angle;
        float offsetInc = pickupOffset.y * 2.0f / (toCreate-1);
        PickupRepresentationArr allPickups = isAssistMode.Value? assistModePickups : normalModePickups;
        List<PickupRepresentation> possiblePickups = new List<PickupRepresentation>();
        possiblePickups.AddRange(allPickups.Pickups);

        for(int i = 0; i < toCreate; i++)
        {
            Vector2 anchorPos = Vector2.right *(wallVar.Value.Radius + pickupOffset.x) 
                                + Vector2.up * (-pickupOffset.y + offsetInc * i);
            anchorPos = MathUtils.Rotate(anchorPos, angle);

            Vector2 dir = (MathUtils.Rotate(Vector2.right, (angle + 180) % 360) - MathUtils.Rotate(Vector2.right, angle)).normalized;

            Keyframe lastKey = pickupSpeed.keys[pickupSpeed.keys.Length-1];
            float speed = pickupSpeed.Evaluate(Mathf.Clamp(CurrentWave, 0, lastKey.time));

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
                possiblePickups = PopulatePickup(pickup, possiblePickups);
            }
            
            pickup.SetupPickup(dir, speed);
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

    private List<PickupRepresentation> PopulatePickup(RoundPickup pickup, List<PickupRepresentation> pickups)
    {
        if(pickups == null || pickups.Count <= 0)
        {
            return pickups;
        }

        int chosenIndex = TimedBoundRandom.RandomInt(0, pickups.Count);

        PickupRepresentation representation = pickups[chosenIndex];
        pickup.SetPlayerStats(representation.increments,
                              representation.decrements, 
                              representation.image);
        pickup.topLabel = representation.topLabel;
        pickup.botLabel = representation.botLabel;

        pickups.RemoveAt(chosenIndex);
        return pickups;
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

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(Vector3.zero, instantiateDistance);
    }
}
