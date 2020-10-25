using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AmoaebaUtils;

public class WaveSpawner : MonoBehaviour, IGameTimeListener
{
    private TimelinedProperty<TimedFloat, float> currentIndex = new TimelinedProperty<TimedFloat, float>();
    private TimelinedProperty<TimedBool, bool> instantiatedWave = new TimelinedProperty<TimedBool, bool>(); 
    
    [SerializeField]
    private EnemyWaves waves;


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
    private RoundPickup roundPickupPrefab;

    [SerializeField]
    private TransformArrVar currentPickups;

    [SerializeField]
    private PickupRepresentationArr normalModePickups;
    
    [SerializeField]
    private PickupRepresentationArr assistModePickups;

    [SerializeField]
    private BoolVar isAssistMode;

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
        toCreate = Mathf.Max(1, toCreate);

        for(int i = 0; i < toCreate; i++)
        {
            RoundPickup pickup = Instantiate(roundPickupPrefab);
            PopulatePickup(pickup);
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
    }
}
