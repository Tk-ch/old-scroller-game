using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heal : ObjectObstacle
{

    new void OnTriggerEnter2D(Collider2D collision) {
        Check(collision);
    }
    new void OnTriggerStay2D(Collider2D collision) {
        Check(collision);
    }

    private void Check(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;
        if (!game.Ship.IsRolling) return;
        game.Ship.Engine.CurrentGear -= (int)gearDamage;
        game.Ship.Armor.HP -= (int)damage;
        if (canBeDestroyedByShip) Destroy(gameObject);
    }
}
