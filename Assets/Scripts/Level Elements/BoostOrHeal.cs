using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoostOrHeal : ObjectObstacle
{
    [SerializeField] float accelerationBoost;
    [SerializeField] Color boostColor;
    [SerializeField] Color healColor;


    private void Start()
    {
        game.player.ShipComponent.OnRollChanged += UpdateColor;
    }

    private void UpdateColor() {
        GetComponent<SpriteRenderer>().color = game.player.ShipComponent.IsRolling ? boostColor : healColor;
    }

    new void OnTriggerEnter2D(Collider2D collision)
    {
        Check(collision);
    }

    private void Check(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;
        if (game.player.ShipComponent.IsRolling)
        {
            game.player.EngineComponent.CurrentAcceleration += accelerationBoost;
        }
        else {
            game.player.ArmorComponent.Armor.HP -= (int)damage;
        }
        if (canBeDestroyedByHit) {

            Destroy(gameObject); 
        }
    }

    private void OnDestroy()
    {
        game.player.ShipComponent.OnRollChanged -= UpdateColor;
    }
}
