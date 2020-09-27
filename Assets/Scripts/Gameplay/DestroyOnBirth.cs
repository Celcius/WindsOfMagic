using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnBirth : MonoBehaviour
{
    [SerializeField]
    private GameTimeVar timeHandler;

    private float birthTime;

    private void Start()
    {
        birthTime = timeHandler.ElapsedTime;
    }
    
    private void Update()
    {
        if(timeHandler.GameSpeed < 0 && timeHandler.ElapsedTime < birthTime)
        {
            Destroy(gameObject);
        }
    }
}
