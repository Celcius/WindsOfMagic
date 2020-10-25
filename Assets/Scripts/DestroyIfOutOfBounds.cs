using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyIfOutOfBounds : MonoBehaviour
{
    [SerializeField]
    private WallScriptVar wall;

    // Start is called before the first frame update
    void Start()
    {
        if(wall.Value.IsOutOfBounds((Vector2)transform.position))
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(wall.Value.IsOutOfBounds((Vector2)transform.position))
        {
            Destroy(gameObject);
        }
    }


}
