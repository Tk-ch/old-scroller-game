using log4net.Core;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLevelEditor : EditorWindow
{
    [MenuItem("Window/Scene Level Editor")]
    public static void OpenLevelEditor() => GetWindow<SceneLevelEditor>();


    SerializedObject so;
    SerializedProperty levelElementParentProperty;
    SerializedProperty levelNameProperty;
    public string levelName = "0001";
    public GameObject parent;
    GUILayoutOption expandDefault = GUILayout.ExpandWidth(false);
    private void OnEnable()
    {
        so = new SerializedObject(this);
        levelElementParentProperty = so.FindProperty("parent");
        levelNameProperty = so.FindProperty("levelName");
    }

    private void OnSelectionChange()
    {

        foreach (var prefab in Selection.gameObjects) {
            if (prefab.transform.IsChildOf(parent.transform)) continue;
            if (prefab.gameObject.scene.name == null || prefab.gameObject.scene.name == prefab.gameObject.name) continue;
            if (!prefab.TryGetComponent(out LevelElement _)) continue;
            if (prefab.transform.parent == parent) continue;
            prefab.transform.parent = parent.transform;
            prefab.transform.Rotate(0,0,-90);
        }
    }

    private void OnGUI()
    {
        so.Update();
        EditorGUILayout.PropertyField(levelElementParentProperty, expandDefault);
        EditorGUILayout.PropertyField(levelNameProperty, expandDefault);
        so.ApplyModifiedProperties();
        if (GUILayout.Button("Save")) {
            SaveLevel();
        }
        if (GUILayout.Button("Load")) {
            LoadLevel();
        }
    }

    private void LoadLevel() {
        foreach (var prefab in parent.GetComponentsInChildren<LevelElement>())
        {
            DestroyImmediate(prefab.gameObject);
        }

        var levelString = (TextAsset)Resources.Load("Levels/" + levelName);
        Level level;
        if (levelString == null)
        {
            level = new Level();
        }
        else level = JsonConvert.DeserializeObject<Level>(levelString.text, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto });
        foreach (var el in level.Elements) {
            GameObject prefab = PrefabUtility.InstantiatePrefab(Resources.Load("Prefabs/" + el.PrefabName, typeof(GameObject)), parent.transform) as GameObject;
            float yOffset = (el.Data is FieldObstacleData) ? ((FieldObstacleData)el.Data).Length : 0;
            prefab.transform.localPosition = new Vector3(el.X, el.Y + yOffset, 0);
            prefab.GetComponent<LevelElement>().Init(el.Data);
        }
    }

    private void SaveLevel()
    {
        Level level = new Level();

        foreach (var prefab in parent.GetComponentsInChildren<LevelElement>()) {
            float yOffset = (prefab.Data is FieldObstacleData) ? ((FieldObstacleData)prefab.Data).Length : 0;
            LevelElementInfo tempElement = new LevelElementInfo {
                Data = prefab.Data,
                X = prefab.transform.localPosition.x,
                Y = prefab.transform.localPosition.y - yOffset,
                PrefabName = PrefabUtility.GetCorrespondingObjectFromSource(prefab.gameObject).name
            };
            level.Elements.Add(tempElement);
            
        }
        level.Elements.Sort();
        string levelString = JsonConvert.SerializeObject(level, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto });
        File.WriteAllText(Application.dataPath + "/Resources/Levels/" + levelName + ".json", levelString);
        AssetDatabase.Refresh();
    }
}
