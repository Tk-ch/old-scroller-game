using Nebuloic;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;


/// <summary>
/// A base class for all the elements of the levels
/// </summary>
public abstract class LevelElementData { }

[ExecuteInEditMode]
public abstract class LevelElement : MonoBehaviour
{

    protected LevelElementData data;
    public LevelElementData Data => data;
    
    [SerializeField] protected LevelManager game; // б≥льш≥сть елемент≥в р≥вню потребуватимуть гравц€
    [SerializeField] float destroyCoordinate = -10;

    protected event Action OnInit;
    
    // Most level elements move downward with the speed of the player, so this method does this
    protected void FixedUpdate()
    {
        if (LevelManager.instance.IsPaused) return;

        transform.Translate(new Vector2(0, -Player.instance.Ship.Engine.CurrentSpeed * Time.fixedDeltaTime), Space.World);

        if (transform.position.y < destroyCoordinate) Destroy(gameObject);
    }

    
    public void Init(LevelElementData data) {
        this.data = data;
        OnInit?.Invoke();
    }

}
