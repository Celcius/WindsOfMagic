using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeController : MonoBehaviour
{
    [SerializeField]
    private GameTimeVar gameTime;

    // Update is called once per frame
    void Update()
    {    
        if(gameTime.GameSpeed != 0)
        {
            gameTime.ElapsedTime = Mathf.Clamp(gameTime.ElapsedTime + Time.deltaTime * gameTime.GameSpeed,
                                               0,
                                               float.MaxValue);
        }
    }
}
