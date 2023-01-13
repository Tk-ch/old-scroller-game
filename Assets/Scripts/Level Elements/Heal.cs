using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heal : ObjectObstacle
{

    new void OnTriggerEnter2D(Collider2D col) { 
    }
    void OnTriggerStay2D(Collider2D collision) {
        Check(collision);
    }

    private void Check(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;
        if (!game.player.isRolling) return;
        game.player.TakeDamage((int)damage, (int)shiftDamage);
        if (canBeDestroyedByShip) Destroy(gameObject);
    }
}
