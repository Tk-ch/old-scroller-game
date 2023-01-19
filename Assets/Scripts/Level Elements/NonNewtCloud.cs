using System.Collections;
using UnityEngine;



/// <summary>
/// Player will take massive damage if enters non-newtonian cloud at an incorrect speed (or switches the speed to an incorrect one when inside the cloud)
/// </summary>
public class NonNewtCloud : FieldObstacle
{
    private long gear;

    private new void Start()
    {
        Color c = game.UIhandler.GearColorsSelected[(int)gear];
        c.a = 0.5f;
        GetComponent<SpriteRenderer>().color = c;
        game.UIhandler.SetWarning(c, 5f);
        
        base.Start();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;
        if (game.player.EngineComponent.CurrentGear > gear) {
            game.player.EngineComponent.CurrentGear -= (int)gearDamage;
            game.player.ArmorComponent.HP -= (int)damage;
        }
    }

}
