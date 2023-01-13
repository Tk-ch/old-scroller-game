using System.Collections;
using UnityEngine;


public class GravityWell : LevelElement
{

    float force;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;
        game.player.transform.Translate(new Vector3(force * Time.deltaTime, 0,0));
    }

}
