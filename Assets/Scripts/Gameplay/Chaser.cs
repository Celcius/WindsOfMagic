using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AmoaebaUtils;

public class Chaser : MonoBehaviour
{
    [SerializeField]
    private TransformVar player;

    [SerializeField]
    private float moveSpeed;


    [SerializeField, Range(0,1.0f)]
    private float steerSpeed;

    private Vector2 dir = Vector2.zero;

    void Update()
    {
        if(GameTime.Instance.GameSpeed <= 0)
        {
            return;
        }

        dir =  Vector2.Lerp(dir, (Vector2)(player.Value.position - transform.position).normalized, steerSpeed / moveSpeed);
        transform.position += (Vector3)dir.normalized * moveSpeed * GameTime.Instance.DeltaTime;

        Debug.DrawLine(transform.position, transform.position+(Vector3)dir.normalized, Color.red, 10.0f);
        
    }
}
