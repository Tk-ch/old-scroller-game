using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndPortal : LevelElement
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) {
            game.FinishGame();
        } 
    }
}
