using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AmoaebaUtils;

public class WeirdShooter : BulletWaveShooter
{
    [SerializeField]
    private Sprite[] shootAnimation;

    [SerializeField]
    private SpriteRenderer spriteRenderer;

    [SerializeField]
    private RotateTowardsPlayer rotateComponent;

    [SerializeField]
    private Chaser chaserComponent;

    private bool isShooting = false;
    private bool isAnimatingShoot = false;
    private float shootingStart;

    private float animateShootElapsed = 0;

    [SerializeField]
    private float shootAnimationTime = 3.0f;

    [SerializeField]
    private GameObject normalCollider;

    [SerializeField]
    private GameObject shootingCollider;

    protected override void Start() 
    {
        spriteRenderer.sprite = shootAnimation[0];
        UpdateColliders(false);
        base.Start();
    }


    protected override void Update()
    {   
        if(isAnimatingShoot)
        {
            AnimateShoot();
        }
        else if(!isShooting)
        {
            bool isPlayerClose = playerMinDistance == float.MaxValue 
                || Vector2.Distance(player.Value.position, transform.position) <= playerMinDistance;

            if(isPlayerClose)
            {
                rotateComponent.UpdateRotation();
                shootingStart = GameTime.Instance.ElapsedTime;
                rotateComponent.enabled = false;
                chaserComponent.enabled =  false;
                chaserComponent.Stop();
                isAnimatingShoot = true;
                animateShootElapsed =  shootingStart + shootAnimationTime - GameTime.Instance.ElapsedTime;
            }
        }
        else if(isShooting && !HasFinishedShooting)
        {
            ShootUpdate(true);
        }
        else
        {
            ResetShot();
            isShooting = false;
            rotateComponent.enabled = true;
            chaserComponent.enabled =  true;
            UpdateColliders(false);
            spriteRenderer.sprite = shootAnimation[0];
        }
    }

    private void AnimateShoot()
    {
        animateShootElapsed -= GameTime.Instance.DeltaTime;
        if(animateShootElapsed > 0 && animateShootElapsed <= shootAnimationTime)
        {
            int index = (int)((1.0f - Mathf.Clamp01(animateShootElapsed / shootAnimationTime)) * (shootAnimation.Length-1));
            bool shootCollider = index+1 >= (shootAnimation.Length)/2.0f;
            UpdateColliders(shootCollider);
            
            spriteRenderer.sprite = shootAnimation[index];
        }
        else if(animateShootElapsed <= 0)
        {
            spriteRenderer.sprite = shootAnimation[shootAnimation.Length-1];
            isAnimatingShoot = false;
            isShooting = true;
        }
        else
        {
            spriteRenderer.sprite = shootAnimation[0];
            isAnimatingShoot = false;
            isShooting = false;
            rotateComponent.enabled = true;
            chaserComponent.enabled =  true;
            UpdateColliders(false);
        }
    }

    private void UpdateColliders(bool isShooting)
    {
        shootingCollider.SetActive(isShooting);
        normalCollider.SetActive(!isShooting);
    }
}
