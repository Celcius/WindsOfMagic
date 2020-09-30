using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : ScriptableObject
{
    public Vector2 GetMoveAxis()
    {
        return new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
    }

    public bool IsShooting()
    {
        return true;
    }

    public bool IsReversing()
    {
        return false;
    }
}
