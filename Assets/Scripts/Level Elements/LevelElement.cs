using Nebuloic;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;


/// <summary>
/// A base class for all the elements of the levels
/// </summary>
/// 
public abstract class LevelElementData { }

public abstract class LevelElement : MonoBehaviour
{

    protected LevelElementData data;
    public LevelElementData Data => data;
    
    [SerializeField] protected Game game; // �������� �������� ���� ��������������� ������
    [SerializeField] float destroyCoordinate = -10;

    
    // Most level elements move downward with the speed of the player, so this method does this
    protected void FixedUpdate()
    {
        transform.Translate(new Vector2(0, -Player.instance.Ship.Engine.CurrentSpeed * Time.fixedDeltaTime), Space.World);

        if (transform.position.y < destroyCoordinate) Destroy(gameObject);
    }

    
    public void Init(LevelElementData data) {
        this.data = data;
    }
}
