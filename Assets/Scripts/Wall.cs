using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AmoaebaUtils;

[RequireComponent(typeof(EdgeCollider2D))]
[ExecuteInEditMode]
public class Wall : MonoBehaviour
{
    
    private Bounds gameBounds;
    public Bounds GameBounds => gameBounds; 

    public float radius;
    
    [Range(2, 200)]
    public int points = 50;

    private EdgeCollider2D edgeCollider2D;
    [SerializeField]
    private LineRenderer[] representations;
    private int currentColliderPoints = -1;
    private float currentColliderRadius = -1;

    [SerializeField]
    private ColorScheme currentColors;

    [SerializeField]
    private ColorType colorType;

    private void Awake()
    {
        UpdateCollider();
     /*   BoxCollider2D[] colliders = GetComponents<BoxCollider2D>();
        Vector2 startPos = new Vector2(float.MaxValue, float.MaxValue);
        Vector2 size = new Vector2(float.MinValue, float.MinValue);

        foreach(BoxCollider2D collider in colliders)
        {
            startPos.x = Mathf.Min(startPos.x, collider.bounds.min.x);
            startPos.y = Mathf.Min(startPos.y, collider.bounds.min.y);

            size.x = Mathf.Max(size.x, collider.bounds.size.x);
            size.y = Mathf.Max(size.y, collider.bounds.size.y);
        }
        gameBounds = new Bounds(startPos + size/2.0f, size);*/

        
    }

    private void UpdateCollider()
    {
        if(representations[0].sharedMaterial != null)
        {
            Color color = currentColors.GetColor(colorType);
            representations[0].sharedMaterial.color = color;
        }
        
        if(currentColliderPoints == points && currentColliderRadius == radius)
        {
            return;
        }

        if(edgeCollider2D == null)
        {
            edgeCollider2D = GetComponent<EdgeCollider2D>();
        }

        Vector2[] pointsVec = new Vector2[points+3];
    
        foreach(LineRenderer representation in representations)
        {
            representation.positionCount = pointsVec.Length;
        }

        for(int i = 0; i <= points; i++)
        {
            float angle = (360 /points * i) % 360;
            pointsVec[i] = GeometryUtils.PointInCircle(radius, angle);

            foreach(LineRenderer representation in representations)
            {
                representation.SetPosition(i, (Vector3)pointsVec[i]);
            }
        }
        pointsVec[points+1] = pointsVec[0];
        pointsVec[points+2] = pointsVec[1];
        foreach(LineRenderer representation in representations)
        {
            representation.SetPosition(points+1, pointsVec[0]);
            representation.SetPosition(points+2, pointsVec[1]);
        }
    
        
        edgeCollider2D.points = pointsVec;
        currentColliderRadius = radius;
        currentColliderPoints = points;
    }

#if UNITY_EDITOR
private void OnValidate() 
{
    UpdateCollider();    
}
#endif

    public bool IsOutOfBounds(Vector2 position)
    {
        return (position - (Vector2)transform.position).magnitude >= radius;
    }

    public void OnDrawGizmos() {
        Gizmos.DrawWireCube(gameBounds.center, gameBounds.size);
    }
}
