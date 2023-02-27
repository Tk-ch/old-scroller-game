using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System;

namespace Nebuloic
{

    /// <summary>
    /// The main game class, attached to the camera
    /// Is used to connect player with the other classes, and also instantiates level prefabs
    /// </summary>
    public class LevelManager : MonoBehaviour
    {
        // Player field 
        // The background of the scene
        [SerializeField] GameObject background;

        // Filename of the level to load
        [SerializeField] string levelName;


        [SerializeField] float endCoord;

        // Current level position
        public float levelPosition;

        float gameTimeInSeconds = 0;

        private bool isPaused = false;
        public bool IsPaused => isPaused;

        // Loaded level
        Level level;

        public static LevelManager instance;


        /// <summary>
        /// Instantiates a level element and initializes it
        /// </summary>
        /// <param name="el">The element to instantiate</param>
        void InstantiateElement(LevelElementInfo el)
        {
            GameObject prefab = Instantiate(Resources.Load("Prefabs/" + el.PrefabName, typeof(GameObject)), transform, true) as GameObject;
            float yOffset = (el.Data is FieldObstacleData) ? ((FieldObstacleData) el.Data).Length : prefab.GetComponent<Renderer>().bounds.extents.y;

            prefab.transform.position = new Vector3(el.X, transform.position.y + yOffset, transform.position.z);
            prefab.GetComponent<LevelElement>().Init(el.Data);
        }


        private void Awake()
        {
            instance = this;
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

        public void InvertPause() {
            isPaused = !isPaused;
        }

        private void FixedUpdate()
        {
            if (isPaused) return;
            levelPosition += Player.instance.Ship.Engine.CurrentSpeed * Time.fixedDeltaTime;

            // Instantiates every element that is "before" a given position
            while (level.Elements.Count > 0 && level.Elements[0].Y <= levelPosition)
            {
                InstantiateElement(level.Elements[0]);
                level.Elements.RemoveAt(0);
            }
        }

        private void Update()
        {
            // Just an update to the background
            if (background != null) background.GetComponent<Renderer>().material.SetFloat("_Y", levelPosition / Screen.height * 15);
            gameTimeInSeconds += Time.deltaTime;
            UIHandler.instance.guiHandler.time = gameTimeInSeconds;
            UIHandler.instance.guiHandler.tValueMap = levelPosition / endCoord;
        }

        public void FinishGame()
        {
            if (Player.instance.Ship.Armor.HP <= 0)
            {
                UIHandler.instance.guiHandler.ShowFinishGame("You died lol");
            }
            else
            {
                UIHandler.instance.guiHandler.ShowFinishGame(string.Format("Your time: {0} s", gameTimeInSeconds));
            }
        }

    }
}