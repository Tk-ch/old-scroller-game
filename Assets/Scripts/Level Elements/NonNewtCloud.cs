using Nebuloic;
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
        Color c = UIHandler.instance.GearColorsSelected[(int)gear];
        c.a = 0.5f;
        GetComponent<SpriteRenderer>().color = c;
        UIHandler.instance.guiHandler.SetWarning(c, 5f);
        
        base.Start();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;
        if (Player.instance.Ship.Engine.CurrentGear > gear) {
            Player.instance.Ship.Engine.CurrentGear -= (int)gearDamage;
            Player.instance.Ship.Armor.HP -= (int)damage;
        }
    }

}
