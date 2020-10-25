using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AmoaebaUtils;

[RequireComponent(typeof(TransformMovement))]
public class RoundPickup : MonoBehaviour
{
    [SerializeField]
    private float startPosOffset = 1.5f;

    [SerializeField]
    private int roundsToMaxSpeed = 10;
    
    [SerializeField]
    private PhysicsAnimationCurve speedCurve;

    [SerializeField]
    private WaveSpawnerVar waveSpawner;

    [SerializeField]
    private WallScriptVar wall;

    private PlayerPickups playerStats;

    private ProjectilePickups projectileStats;

    private SpriteRenderer image;

    private TransformMovement movement;

    Vector2 dir;

    private void Awake() 
    {
        this.image = GetComponent<SpriteRenderer>();
        projectileStats = GetComponent<ProjectilePickups>();
        playerStats = GetComponent<PlayerPickups>();
    }
    void Start()
    {

        movement = GetComponent<TransformMovement>();

        int posType = TimedBoundRandom.RandomInt(0,4);
        
        Vector2 offset = Vector2.zero;
        switch(posType)
        {
            case 0:
                dir = Vector2.down;
                offset = Vector2.right;
                break;
            case 1:
                dir = Vector2.left;
                offset = Vector2.up;
                break;
            case 2:
                dir = Vector2.up;
                offset = Vector2.right;
                break;
            case 3:
                dir = Vector2.right;
                offset = Vector2.up;
                break;
        }

        Vector2 size = wall.Value.GameBounds.size;
        // Assume center is 0,0
        Vector2 startPos = Vector2.Scale(-dir, size/2.0f) * startPosOffset;
        transform.position = startPos + Vector2.Scale(offset, size) * TimedBoundRandom.RandomFloat(-0.4f, 0.4f);

        
        float speedRatio = (float) (Mathf.Clamp(waveSpawner.Value.CurrentWave,0,roundsToMaxSpeed) / roundsToMaxSpeed);
        float speed = speedCurve.Evaluate(0, speedRatio);

        movement.SetAxisMultiplier(dir * speed);
        Destroy(this);
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
