using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// A class for obstacles that are relatively small objects on screen
// Such as meteors, asteroids, comets, etc.
public class ObjectObstacle : Obstacle
{
    [SerializeField] protected float rotationSpeed = 0;
    [SerializeField] protected float relativeSpeed = 0;
    [SerializeField] protected float obstacleHP = 1;
    [SerializeField] protected bool canBeDestroyedByHit = true;
    [SerializeField] protected bool canBeDestroyedByShip = true;
    public float ObstacleHP
    { 
        get => obstacleHP; 
        set 
        {
            if (value <= 0 && canBeDestroyedByHit) Destroy(gameObject);
            obstacleHP = value;
        } 
    }

    private new void FixedUpdate()
    {
        transform.Rotate(new Vector3(0,0,rotationSpeed * Time.fixedDeltaTime));
        transform.Translate(new Vector3(0, relativeSpeed * Time.fixedDeltaTime, 0), Space.World);
        base.FixedUpdate();
    }

    protected void OnTriggerEnter2D(Collider2D collision) => Check(collision);
    protected void OnTriggerStay2D(Collider2D collision)
    {
        Check(collision);
    }

    void Check(Collider2D collision) {

        if (!collision.CompareTag("Player") || !game.Ship.Armor.IsVulnerable) return;
        game.Ship.Engine.CurrentGear -= (int)gearDamage;
        game.Ship.Armor.HP -= (int)damage;
        if (canBeDestroyedByShip) Destroy(gameObject);
    }
}
