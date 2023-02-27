using System;
using System.Collections;
using UnityEngine;
using static GravityWell;


[Serializable]
public class GravityWellData : LevelElementData
{
    public float force;
}

public class GravityWell : LevelElement
{

    [SerializeField] protected GravityWellData gravityWellData;
    private void OnEnable()
    {
        OnInit += Init;

    }
    private void OnValidate()
    {
        data = gravityWellData;
        Init();
    }

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        gravityWellData = (GravityWellData)data;
        if (((GravityWellData)data).force < 0)
        {
            GetComponent<SpriteRenderer>().flipY = false;
        } else
        {
            GetComponent<SpriteRenderer>().flipY = true;

        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;
        Player.instance.ShipBehaviour.transform.Translate(new Vector3(((GravityWellData)data).force * Time.deltaTime, 0,0));
    }

}
