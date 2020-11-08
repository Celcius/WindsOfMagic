using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnBirth : MonoBehaviour
{
    private GameTime gameTime;

    private float birthTime;

    private void Start()
    {
        gameTime = GameTime.Instance;
        birthTime = gameTime.ElapsedTime;
    }
    
    private void Update()
    {
        if(gameTime.GameSpeed < 0 && gameTime.ElapsedTime < birthTime)
        {
            
            Destroy(gameObject);
        }
    }
}
