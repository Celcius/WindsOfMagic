using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AmoaebaUtils;

public class WaveSpawner : MonoBehaviour, IGameTimeListener
{
    [SerializeField]
    private EnemyWaves waves;

    private TimelinedProperty<TimedFloat, float> currentIndex = new TimelinedProperty<TimedFloat, float>();
    private TimelinedProperty<TimedBool, bool> instantiatedWave = new TimelinedProperty<TimedBool, bool>(); 
    
    [SerializeField]

    private TransformArrVar aliveEnemies;

    
    public void Start()
    {
        currentIndex.Clear();
        instantiatedWave.Clear();
        SetCurrentIndexValue(0);
        SetIsInstantiatedValue(false);
        aliveEnemies.OnChange -= OnEnemyChange;
        aliveEnemies.OnChange += OnEnemyChange;
        GameTime.Instance.AddTimeListener(this);
    }

    private void OnDestroy() 
    {
        aliveEnemies.OnChange -= OnEnemyChange;
        GameTime.Instance.RemoveTimeListener(this);
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
        if(GameTime.Instance.IsReversing)
        {
            currentIndex.InterpolatedValueAt(GameTime.Instance.ElapsedTime);
            instantiatedWave.InterpolatedValueAt(GameTime.Instance.ElapsedTime);
        }
        else if(!GameTime.Instance.IsStopped && !instantiatedWave.Value)
        {
            InstantiateWave();
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

    private void SetCurrentIndexValue(float val)
    {
        currentIndex.SetValue(new TimedFloat(GameTime.Instance.ElapsedTime, val));       
    }

    private void SetIsInstantiatedValue(bool val)
    {
        instantiatedWave.SetValue(new TimedBool(GameTime.Instance.ElapsedTime, val));
    }
}
