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

    [SerializeField]
    private float wallRadius;
    public float Radius => wallRadius;
    
    [Range(2, 200)]
    [SerializeField]
    private int wallPoints = 50;

    [SerializeField]
    private float pickupColliderRadius;

    [SerializeField]
    private int pickupColliderPoints;
    [SerializeField]
    private EdgeCollider2D edgeCollider2D;
    
    [SerializeField]
    private EdgeCollider2D pickupEdgeCollider2D;

    [SerializeField]
    private LineRenderer[] representations;

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

        UpdateColliderPoints(edgeCollider2D, wallPoints, wallRadius, true);
        UpdateColliderPoints(pickupEdgeCollider2D, pickupColliderPoints, pickupColliderRadius, false);
    }

    private void UpdateColliderPoints(EdgeCollider2D collider, int points, float radius, bool updateRepresentation)
    {
        Vector2[] pointsVec = new Vector2[points+3];
    
        if(updateRepresentation)
        {
            foreach(LineRenderer representation in representations)
            {
                representation.positionCount = pointsVec.Length;
            }
        }

        for(int i = 0; i <= points; i++)
        {
            float angle = (360 /points * i) % 360;
            pointsVec[i] = GeometryUtils.PointInCircle(radius, angle);

            if(updateRepresentation)
            {
                foreach(LineRenderer representation in representations)
                {
                    representation.SetPosition(i, (Vector3)pointsVec[i]);
                }
            }
        }

        pointsVec[points+1] = pointsVec[0];
        pointsVec[points+2] = pointsVec[1];

        if(updateRepresentation)
        {
            foreach(LineRenderer representation in representations)
            {
                representation.SetPosition(points+1, pointsVec[0]);
                representation.SetPosition(points+2, pointsVec[1]);
            }
        }
        
        collider.points = pointsVec;
    }

#if UNITY_EDITOR
private void OnValidate() 
{
    UpdateCollider();    
}
#endif

    public bool IsOutOfBounds(Vector2 position)
    {
        return (position - (Vector2)transform.position).magnitude >= wallRadius;
    }

    public void OnDrawGizmos() {
        Gizmos.DrawWireCube(gameBounds.center, gameBounds.size);
    }
}
