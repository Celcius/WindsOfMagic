using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AmoaebaUtils;

public class RotateTowardsPlayer : MonoBehaviour
{
    [SerializeField]
    private TransformVar player;

    [SerializeField]
    private float angleRot = 90.0f;

    [SerializeField]
    private bool destroyComponentOnStart = false;

    [SerializeField]
    private float rotateSpeed = 0;

    private void Start()
    {
        UpdateRotation();
        if(destroyComponentOnStart)
        {
            Destroy(this);
        }
    }

    private void Update()
    {
       UpdateRotation();
    }

    private void UpdateRotation()
    {
        Vector2 dir = player.Value.position - transform.position;
        if(rotateSpeed <= 0)
        {
            transform.right = Quaternion.Euler(0,0, angleRot) * dir.normalized;
        }
        else
        {
            transform.right += Quaternion.Euler(0,0, angleRot) * dir.normalized * GameTime.Instance.DeltaTime * rotateSpeed;
        }
    }
}
