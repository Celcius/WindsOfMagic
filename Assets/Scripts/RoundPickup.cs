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

    private SpriteRenderer image;

    private TransformMovement movement;

    public string topLabel;
    public string botLabel;


    Vector2 dir;

    private void Awake() 
    {
        this.image = GetComponent<SpriteRenderer>();
        playerStats = GetComponent<PlayerPickups>();
    }
    public void SetupPickup(Vector2 dir, float speed)
    {
        movement = GetComponent<TransformMovement>();

        movement.SetAxisMultiplier(dir * speed);
    }

    public void SetPlayerStats(PlayerStatType[] increments, PlayerStatType[] decrements, Sprite representation)
    {
        this.playerStats.increments = increments;
        this.playerStats.decrements = decrements;
        this.image.sprite = representation;
    }
}
