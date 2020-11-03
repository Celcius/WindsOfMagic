using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AmoaebaUtils;

[RequireComponent(typeof(TimedHealth))]
public class DropPickupsOnDisable : SpawnOnDeath
{

    [SerializeField]
    private DropsDefinitionVar definition;

    [SerializeField]
    private FloatVar playerHealth;

    [SerializeField]
    private AudioClip sound;

    protected override void OnDeathEvent() 
    {
        Transform explosion = InstantiateEntity();
        AnimateFramesDestroyInstantiate animate = explosion.GetComponent<AnimateFramesDestroyInstantiate>();

        
        if(definition == null || definition.Value == null || playerHealth.Value <= 0)
        {
            return;
        }

        Transform toInstantiate = definition.Value.GetNextRandomDrop();
        if(animate != null)
        {
            animate.ToInstantiate = toInstantiate;
            animate.scale = scale;
        }
        else
        {
            Transform instance = Instantiate(toInstantiate, transform.position, toInstantiate.rotation);
            instance.localScale = Vector3.Scale(instance.localScale, scale);
        }
    }
}
