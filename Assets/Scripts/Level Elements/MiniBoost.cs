using System.Collections;
using UnityEngine;

public class MiniBoost : ObjectObstacle
{
    [SerializeField] float accelerationBoost;
    new void OnTriggerEnter2D(Collider2D collision)
    {
        Check(collision);
    }
    new void OnTriggerStay2D(Collider2D collision)
    {
        Check(collision);
    }

    private void Check(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;
        if (!Player.instance.Ship.IsRolling) return;
        Player.instance.Ship.Engine.CurrentAcceleration += accelerationBoost;
        if (canBeDestroyedByHit) Destroy(gameObject);
    }
}
