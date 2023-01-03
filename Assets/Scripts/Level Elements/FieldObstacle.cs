using System.Collections;
using UnityEngine;

// A class for obstacles that take up space of all screen and have some length
// For example, nebulas, non-newtonic clouds, etc
public class FieldObstacle : Obstacle
{
    protected float length;

    protected void Start()
    {
        transform.localScale = new Vector3(transform.localScale.x, length, 1);
    }

}
