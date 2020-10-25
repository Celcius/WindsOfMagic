using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TimedHealth))]
public class DropPickupsOnDisable : MonoBehaviour
{

    [SerializeField]
    private DropsDefinitionVar definition;

    [SerializeField]
    private FloatVar playerHealth;

    private TimedHealth health;

    private void Start()
    {
        health = GetComponent<TimedHealth>();
        health.OnDeathEvent += OnDeath;
    }

    private void OnDestroy() 
    {
        health.OnDeathEvent -= OnDeath;    
    }

    private void OnDeath() 
    {
        if(definition == null || definition.Value == null || playerHealth.Value <= 0)
        {
            return;
        }

        Transform toInstantiate = definition.Value.GetNextRandomDrop();
        if(toInstantiate != null)
        {
            Instantiate(toInstantiate, transform.position, toInstantiate.rotation);
        }
    }
}
