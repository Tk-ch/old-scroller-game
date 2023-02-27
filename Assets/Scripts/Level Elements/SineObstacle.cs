using Nebuloic;
using System;
using System.Collections;
using UnityEngine;




public class SineObstacle : ObjectObstacle
{
    [Serializable]
    public class SineObstacleData : LevelElementData
    {
        public float horizontalSpeed = 3f;
    }

    [SerializeField] protected new SineObstacleData data;

    private new void FixedUpdate()
    {
        if (LevelManager.instance.IsPaused) return;
        transform.position = new Vector3(Mathf.Sin(game.levelPosition / 5) * data.horizontalSpeed, transform.position.y);

        base.FixedUpdate();
    }
}
