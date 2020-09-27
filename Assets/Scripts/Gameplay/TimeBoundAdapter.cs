using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TimeBoundAdapter<T> : MonoBehaviour where T : MonoBehaviour
{
    protected T[] components;

    [SerializeField]
    private GameTimeVar gameTime;

    private float birthTime = 0;
    private bool enabledComponents = true;

    private void Start()
    {
        components = GetComponents<T>();
        birthTime = gameTime.ElapsedTime;
    }

    private void Update() 
    {
        if(gameTime.GameSpeed > 0 && !enabledComponents)
        {
            foreach(T component in components)
            {
                UpdateTime(component, gameTime.ElapsedTime - birthTime);
                component.enabled = true;
            }
            
            enabledComponents = true;
        }
        else if(gameTime.GameSpeed <= 0 && enabledComponents)
        {
            foreach(T component in components)
            {
                component.enabled = false;
            }

            enabledComponents = false;
        }
    }

    protected abstract void UpdateTime(T component, float gameTime);
}
