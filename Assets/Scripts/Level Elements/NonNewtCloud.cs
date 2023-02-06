using Nebuloic;
using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Player will take massive damage if enters non-newtonian cloud at an incorrect speed (or switches the speed to an incorrect one when inside the cloud)
/// </summary>
[Serializable]
public class NonNewtCloudData : FieldObstacleData
{
    public long Gear;
}

public class NonNewtCloud : FieldObstacle
{
    
    [SerializeField] private NonNewtCloudData nonNewtCloudData;

    private void OnValidate() => UpdateData();

    private new void Start()
    {
        UpdateData();
    }

    private void OnEnable()
    {
        OnInit += Init;
    
    }

    private void Init()
    {
        nonNewtCloudData = (NonNewtCloudData) data;
        UpdateData();
    }

    private new void UpdateData()
    {

        data = nonNewtCloudData;
        base.UpdateData(data);

        NonNewtCloudData localData = (NonNewtCloudData)data; 

        if (UIHandler.instance == null) return;

        nonNewtCloudData.Gear = Mathf.Clamp((int)localData.Gear, 0, UIHandler.instance.GearColors.Length-1);

        Color c = UIHandler.instance.GearColorsSelected[(int)localData.Gear];
        c.a = 0.5f;
        GetComponent<SpriteRenderer>().color = c;
        UIHandler.instance.guiHandler.SetWarning(c, 5f);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;
        if (Player.instance.Ship.Engine.CurrentGear > ((NonNewtCloudData)data).Gear) {
            Player.instance.Ship.Engine.CurrentGear -= (int)gearDamage;
            Player.instance.Ship.Armor.HP -= (int)damage;
        }
    }

}
