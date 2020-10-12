using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AmoaebaUtils;

public class TimeController : MonoBehaviour
{
    private GameTime gameTime;
    void Start() 
    {
        gameTime = GameTime.Instance;
    }

    void Update()
    {    
        if(gameTime.GameSpeed != 0 && !gameTime.IsPaused)
        {
            gameTime.ElapsedTime = Mathf.Clamp(gameTime.ElapsedTime + gameTime.DeltaTime,
                                               0,
                                               float.MaxValue);
        }
    }
}
