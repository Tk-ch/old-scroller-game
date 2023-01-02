using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System;

public class Game : MonoBehaviour
{
    [SerializeField]
    Player player;

    [SerializeField]
    GameObject background;
    [SerializeField]
    string levelName;

    [SerializeField]
    Transform obstacleParent;

    float levelPosition;

    Level level;


    void InstantiateElement(LevelElementInfo el) {
        GameObject prefab = Instantiate(Resources.Load("Prefabs/" + el.PrefabName, typeof(GameObject)), obstacleParent, true) as GameObject;
        prefab.transform.position = new Vector2(el.X, obstacleParent.position.y);
        el.Properties.Add("player", player);
        prefab.GetComponent<LevelElement>().Init(el.Properties);
    }

    void Start()
    {
        

        var levelString = (TextAsset) Resources.Load("Levels/" + levelName);
        level = JsonConvert.DeserializeObject<Level>(levelString.text, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto });

        

    }

    private void FixedUpdate()
    {
        levelPosition += player.CurrentSpeed * Time.fixedDeltaTime;

        while (level.Elements.Count > 0 && level.Elements[0].Y <= levelPosition) {
            InstantiateElement(level.Elements[0]);
            level.Elements.RemoveAt(0);
        }
    }

    private void Update()
    {
        background.GetComponent<Renderer>().material.SetFloat("_Y", levelPosition / Screen.height * 10);
    }

}
