using System.Collections; 
using System.Collections.Generic;
using UnityEngine;
using AmoaebaUtils;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private InputHandler input;
    private Rigidbody2D body;
    private GameTime timeHandler;

    [SerializeField]
    private PlayerStats playerStats;

    private Vector2 speed = Vector2.zero;

    [SerializeField]
    private float deadzone = 0.05f;

    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        timeHandler = GameTime.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        if(timeHandler.GameSpeed > 0)
        {
            body.MovePosition(transform.position + (Vector3)input.GetMoveAxis() * Time.deltaTime * playerStats.MoveSpeed);
        }
    }

    private void Shoot()
    {
        Debug.Log("Pew");
    }
}

