﻿using System.Collections;
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
        transform.right = Quaternion.Euler(0,0, angleRot) * dir.normalized;
    }
}
