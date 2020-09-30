using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeController : MonoBehaviour
{
    private GameTime gameTime;

    void Start() 
    {
        gameTime = GameTime.Instance;
    }

    void Update()
    {    
        if(gameTime.GameSpeed != 0)
        {
            gameTime.ElapsedTime = Mathf.Clamp(gameTime.ElapsedTime + gameTime.DeltaTime,
                                               0,
                                               float.MaxValue);
        }
    }
}
