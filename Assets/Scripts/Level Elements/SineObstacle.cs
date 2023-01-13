using System.Collections;
using UnityEngine;

public class SineObstacle : ObjectObstacle
{
    private new void FixedUpdate()
    {
        transform.position = new Vector3(Mathf.Sin(game.levelPosition / 5) * 3, transform.position.y);

        base.FixedUpdate();
    }
}
