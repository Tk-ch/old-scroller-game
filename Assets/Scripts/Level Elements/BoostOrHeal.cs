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
        if (Player.instance != null)
            Player.instance.Ship.OnRollChanged += UpdateColor;
    }

    private void UpdateColor(bool isRolling) {
        GetComponent<SpriteRenderer>().color = isRolling ? boostColor : healColor;
    }

    new void OnTriggerEnter2D(Collider2D collision)
    {
        Check(collision);
    }

    private void Check(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;
        if (Player.instance.Ship.IsRolling)
        {
            Player.instance.Ship.Engine.CurrentAcceleration += accelerationBoost;
        }
        else {
            Player.instance.Ship.Armor.HP -= (int)damage;
        }
        if (canBeDestroyedByHit) {

            Destroy(gameObject); 
        }
    }

    private void OnDestroy()
    {
        if (Player.instance != null)
            Player.instance.Ship.OnRollChanged -= UpdateColor;
    }
}
