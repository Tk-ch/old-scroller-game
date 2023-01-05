using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System;

/// <summary>
/// The main game class, attached to the camera
/// Is used to connect player with the other classes, and also instantiates level prefabs
/// </summary>
public class Game : MonoBehaviour
{
    // Player field 
    public Player player;

    // The background of the scene
    [SerializeField] GameObject background;

    // Filename of the level to load
    [SerializeField] string levelName;

    // Transform that is used when instantiating prefabs
    [SerializeField] Transform obstacleParent;

    public UIHandler UIhandler;

    // Current level position
    float levelPosition;
    
    // Loaded level
    Level level;


    /// <summary>
    /// Instantiates a level element and initializes it
    /// </summary>
    /// <param name="el">The element to instantiate</param>
    void InstantiateElement(LevelElementInfo el) {
        GameObject prefab = Instantiate(Resources.Load("Prefabs/" + el.PrefabName, typeof(GameObject)), obstacleParent, true) as GameObject;
        float yOffset = el.Properties.TryGetValue("length", out object v) ? Convert.ToSingle(v) : 0;
        prefab.transform.position = new Vector3(el.X, obstacleParent.position.y + yOffset, obstacleParent.position.z);
        el.Properties.Add("game", this);
        prefab.GetComponent<LevelElement>().Init(el.Properties);
    }

    // Is run before first frame of the Scene
    void Start()
    {
        LoadLevel();
    }

    private void LoadLevel()
    {
        var levelString = (TextAsset)Resources.Load("Levels/" + levelName);
        level = JsonConvert.DeserializeObject<Level>(levelString.text, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto });
    }
    
    /// <summary>
    /// Updates the current position on the level and instantiates elements if needed
    /// </summary>
    private void FixedUpdate()
    {
        levelPosition += player.CurrentSpeed * Time.fixedDeltaTime;

        // Instantiates every element that is "before" a given position
        while (level.Elements.Count > 0 && level.Elements[0].Y <= levelPosition) {
            InstantiateElement(level.Elements[0]);
            level.Elements.RemoveAt(0);
        }
    }

    private void Update()
    {
        // Just an update to the background
        background.GetComponent<Renderer>().material.SetFloat("_Y", levelPosition / Screen.height * 15);
    }

}
