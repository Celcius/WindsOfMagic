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

    // Start is called before the first frame update
    void Start()
    {
        playerStats.SetPlayerStats(basePlayerStats);
        projectileStats.SetProjectileStats(baseProjectileStats);
        GameTime.Instance.Start();
    }
}
