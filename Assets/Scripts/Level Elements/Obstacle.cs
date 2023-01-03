using UnityEngine;

// A basic obstacle class yep
// Has damage but no way of doing it

public class Obstacle : LevelElement
{
    [SerializeField] protected long damage = 0;
    [SerializeField] protected long shiftDamage = 0;
}
