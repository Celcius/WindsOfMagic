using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AmoaebaUtils;

public class Rusher : Chaser
{
    private bool isRushing = false;
    [SerializeField]
    private AnimationCurve rushMove;
    [SerializeField]
    private float rushDistance;
    [SerializeField]
    private float rushDuration;

    [SerializeField]
    private float acceptableRushAngle = 25;
    private float elapsedRush = 0;
    private Vector3 startRushPos;

    private Vector3 rushDir;

    [SerializeField]
    private Transform dirTransform;

    [SerializeField]
    private RotateTowardsPlayer rotateComponent;

    protected override void OnWithinDistance()
    {
        
        if(!isRushing && GetPlayerAngle() <= acceptableRushAngle)
        {
            isRushing = true;
            canMove = false;
            elapsedRush = 0;
            rotateComponent.enabled = false;
            startRushPos = transform.position;
            rushDir = dirTransform.up;
        }
    }

    private float GetPlayerAngle()
    {
        return Vector2.Angle(dirTransform.up, player.Value.position - transform.position);
    }

    protected override void Update()
    {
        if(GameTime.Instance.GameSpeed <= 0 || GameTime.Instance.IsReversing)
        {
            
            body.velocity = Vector3.zero;

            isRushing = elapsedRush > 0 && elapsedRush <= rushDuration;
            if(isRushing)
            {
                elapsedRush += GameTime.Instance.DeltaTime;
            }

            rotateComponent.enabled = !isRushing;
            body.isKinematic = !isRushing;

            canMove = !isRushing;
            dirTransform.up = isRushing? rushDir : dirTransform.up;
            return;
        }
        
        if(!isRushing)
        {
            body.isKinematic = false;
            rotateComponent.enabled = true;
            canMove = true;
            base.Update();
        }

        if(isRushing)
        {
            
            elapsedRush += GameTime.Instance.DeltaTime;
            float displacement = rushMove.Evaluate(Mathf.Clamp01(elapsedRush / rushDuration)) * rushDistance;
            body.isKinematic = true;
            body.MovePosition(startRushPos + dirTransform.up * displacement);

            isRushing = elapsedRush >= 0 && elapsedRush <= rushDuration;
        }
    }

    private void OnDrawGizmos() 
    {
        Gizmos.color = Color.magenta;
        
        int points = 20;
        float angleInc = 360.0f /20.0f;
        for(int i = 0; i < points; i++)
        {
            Vector3 start = GeometryUtils.PointInCircle(playerDetectDistance, i*angleInc);
            Vector3 end = GeometryUtils.PointInCircle(playerDetectDistance, (i+1.0f)*angleInc);
            Gizmos.DrawLine(transform.position + start, transform.position + end);
        }
        Gizmos.DrawLine(transform.position, transform.position + dirTransform.up * rushDistance);

        
        Gizmos.DrawLine(transform.position, transform.position 
                        + Quaternion.Euler(0,0, acceptableRushAngle) * dirTransform.up * rushDistance); 
        Gizmos.DrawLine(transform.position, transform.position 
                        + Quaternion.Euler(0,0, -acceptableRushAngle) * dirTransform.up * rushDistance); 
    }
}
