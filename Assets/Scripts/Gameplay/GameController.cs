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
    private ProjectileStats projectileStats;

    [SerializeField]
    private ProjectileStats baseProjectileStats;

    [SerializeField]
    private GameTime gameTime;

    [SerializeField]
    private InputHandler inputHandler;

    [SerializeField]
    private PauseMenu pauseMenu;

    [SerializeField]
    private PlayTestOptions playTestOptions;
    
    void Start()
    {
        playTestOptions.ResetOptions();
        playerStats.SetPlayerStats(basePlayerStats);
        projectileStats.SetProjectileStats(baseProjectileStats);
        GameTime.Instance.Start();
    }

    
    void Update()
    {    
        if(inputHandler.IsPauseDown())
        {
            pauseMenu.SwapPause();
        }
    }
}
