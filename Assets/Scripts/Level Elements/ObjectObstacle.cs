using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectObstacle : Obstacle
{
    float scale = 1;
    float rotationSpeed = 0;
    float relativeSpeed = 0;

    private void Start()
    {
        transform.localScale *= scale;
    }

    private new void FixedUpdate()
    {
        transform.Rotate(new Vector3(0,0,rotationSpeed * Time.fixedDeltaTime));
        transform.Translate(new Vector3(0, relativeSpeed * Time.fixedDeltaTime, 0), Space.World);
        base.FixedUpdate();
    }

    protected void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;
        player.HP -= (int)damage;
        player.CurrentShift -= (int)shiftDamage;

    }
}
