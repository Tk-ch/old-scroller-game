using System;
using System.Collections;
using UnityEngine;

// A class for obstacles that take up space of all screen and have some length
// For example, nebulas, non-newtonic clouds, etc
[Serializable]
public class FieldObstacleData : LevelElementData
{
    public float Length = 1f;
}

public abstract class FieldObstacle : Obstacle
{
    private FieldObstacleData fieldObstacleData;

    private void OnValidate() => UpdateData();

    protected void Start() => UpdateData();

    protected void UpdateData() {
        if (data == null && fieldObstacleData != null) data = fieldObstacleData; 
        transform.localScale = new Vector3(transform.localScale.x, ((FieldObstacleData) data).Length, 1);
    }

}
