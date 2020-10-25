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

    private Rigidbody2D body;
    private void Start()
    {
        body = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if(GameTime.Instance.GameSpeed <= 0 || GameTime.Instance.IsReversing || player.Value  == null)
        {
            body.velocity = Vector3.zero;
            return;
        }

        dir =  Vector2.Lerp(dir, (Vector2)(player.Value.position - transform.position).normalized, steerSpeed / moveSpeed);
        
        Vector3 newPos = transform.position + (Vector3)dir.normalized * moveSpeed * GameTime.Instance.DeltaTime;
        body.velocity = (Vector3)dir.normalized * moveSpeed;
        //body.AddForce(dir.normalized * moveSpeed, ForceMode2D.Impulse);
        //body.MovePosition(newPos);
        //transform.position = newPos;
        //Debug.DrawLine(transform.position, transform.position+(Vector3)dir.normalized, Color.red, 10.0f);
        
    }
}
