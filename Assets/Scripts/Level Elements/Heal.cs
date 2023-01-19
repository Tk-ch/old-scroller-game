using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heal : ObjectObstacle
{

    new void OnTriggerEnter2D(Collider2D collision) {
        Check(collision);
    }
    void OnTriggerStay2D(Collider2D collision) {
        Check(collision);
    }

    private void Check(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;
        if (!game.player.ShipComponent.IsRolling) return;
        game.player.EngineComponent.CurrentGear -= (int)gearDamage;
        game.player.ArmorComponent.HP -= (int)damage;
        if (canBeDestroyedByShip) Destroy(gameObject);
    }
}
