using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AmoaebaUtils;

[RequireComponent(typeof(TransformMovement))]
public class RoundPickup : MonoBehaviour
{
    [SerializeField]
    private int roundsToMaxSpeed = 10;
    
    [SerializeField]
    private PhysicsAnimationCurve speedCurve;

    [SerializeField]
    private WaveSpawnerVar waveSpawner;

    private PlayerPickups playerStats;

    private ProjectilePickups projectileStats;

    private SpriteRenderer image;

    private TransformMovement movement;

    public string topLabel;
    public string botLabel;


    Vector2 dir;

    private void Awake() 
    {
        this.image = GetComponent<SpriteRenderer>();
        projectileStats = GetComponent<ProjectilePickups>();
        playerStats = GetComponent<PlayerPickups>();
    }
    public void SetupPickup(Vector2 dir)
    {
        movement = GetComponent<TransformMovement>();

        float speedRatio = (float) (Mathf.Clamp(waveSpawner.Value.CurrentWave,0,roundsToMaxSpeed) / roundsToMaxSpeed);
        float speed = speedCurve.Evaluate(0, speedRatio);

        movement.SetAxisMultiplier(dir * speed);
    }

    public void SetPlayerStats(PlayerStats stats, Sprite representation)
    {
        this.playerStats.offsetToApply = stats;
        this.image.sprite = representation;
    }

    public void SetProjectileStats(ProjectileStats stats,  Sprite representation)
    {
        this.projectileStats.offsetToApply = stats;
        this.image.sprite = representation;
    }
}
