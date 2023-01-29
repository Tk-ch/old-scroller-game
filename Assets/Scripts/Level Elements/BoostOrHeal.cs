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
        game.player.Ship.OnRollChanged += UpdateColor;
    }

    private void UpdateColor() {
        GetComponent<SpriteRenderer>().color = game.player.Ship.IsRolling ? boostColor : healColor;
    }

    new void OnTriggerEnter2D(Collider2D collision)
    {
        Check(collision);
    }

    private void Check(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;
        if (game.player.Ship.IsRolling)
        {
            game.player.Ship.Logic.Engine.CurrentAcceleration += accelerationBoost;
        }
        else {
            game.player.Ship.Logic.Armor.HP -= (int)damage;
        }
        if (canBeDestroyedByHit) {

            Destroy(gameObject); 
        }
    }

    private void OnDestroy()
    {
        game.player.Ship.OnRollChanged -= UpdateColor;
    }
}
