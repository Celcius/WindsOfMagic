using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    
    private Bounds gameBounds;
    public Bounds GameBounds => gameBounds; 

    void Awake()
    {
        BoxCollider2D[] colliders = GetComponents<BoxCollider2D>();
        Vector2 startPos = new Vector2(float.MaxValue, float.MaxValue);
        Vector2 size = new Vector2(float.MinValue, float.MinValue);

        foreach(BoxCollider2D collider in colliders)
        {
            startPos.x = Mathf.Min(startPos.x, collider.bounds.min.x);
            startPos.y = Mathf.Min(startPos.y, collider.bounds.min.y);

            size.x = Mathf.Max(size.x, collider.bounds.size.x);
            size.y = Mathf.Max(size.y, collider.bounds.size.y);
        }
        gameBounds = new Bounds(startPos + size/2.0f, size);
    }

    public bool IsOutOfBounds(Vector2 position)
    {
        return !gameBounds.Contains(position);
    }
}
