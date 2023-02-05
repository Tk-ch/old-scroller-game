using System;
using System.Collections;
using UnityEngine;


public class GravityWell : LevelElement
{

    [Serializable]
    public class GravityWellData : LevelElementData {
        public float force;
    }

    [SerializeField] protected new GravityWellData data;

    private void Start()
    {
        if (data.force < 0) {
            transform.Rotate(180,0,0);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;
        Player.instance.ShipBehaviour.transform.Translate(new Vector3(data.force * Time.deltaTime, 0,0));
    }

}
