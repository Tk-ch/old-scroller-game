using System.Collections;
using UnityEngine;



/// <summary>
/// Player will take massive damage if enters non-newtonian cloud at an incorrect speed (or switches the speed to an incorrect one when inside the cloud)
/// </summary>
public class NonNewtCloud : FieldObstacle
{
    private long shift;

    private new void Start()
    {
        Color c = game.player.shiftColors[(int)shift] * 2;
        c.a = 0.5f;
        GetComponent<SpriteRenderer>().color = c;
        game.UIhandler.SetWarning(c, 5f);
        
        base.Start();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (game.player.CurrentShift > shift) {
            game.player.TakeDamage((int) damage, (int) shiftDamage);
        }
    }

}
