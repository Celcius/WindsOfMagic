using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField]
    private PlayerStats playerStats;

    [SerializeField]
    private PlayerStats basePlayerStats;

    [SerializeField]
    private GameTime gameTime;

    // Start is called before the first frame update
    void Start()
    {
        playerStats.SetPlayerStats(basePlayerStats);
        gameTime.ElapsedTime = 0.0f;
        gameTime.GameSpeed = 1.0f;
    }
}
