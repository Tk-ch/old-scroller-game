using Newtonsoft.Json;
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
        if (SceneManager.GetActiveScene().name != "LevelEditor") return;
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
        if (SceneManager.GetActiveScene().name != "LevelEditor") return;
        if (GUILayout.Button("Save")) {
            SaveLevel();
        }
        if (GUILayout.Button("Load")) {
            LoadLevel();
        }
    }

    private string GetPrefabPath(GameObject prefab) {
        return AssetDatabase.GetAssetPath(PrefabUtility.GetCorrespondingObjectFromSource(prefab)).Replace(".prefab", "").Replace("Assets/Resources/Prefabs/", "");
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
            float yOffset = (el.Data is FieldObstacleData) ? ((FieldObstacleData)el.Data).Length : prefab.GetComponent<Renderer>().bounds.extents.x; // the objects are rotated 90 degrees so it's x coordinate here and not y
            prefab.transform.localPosition = new Vector3(el.X, el.Y + yOffset, 0);
            prefab.GetComponent<LevelElement>().Init(el.Data);
        }
    }

    private void SaveLevel()
    {
        Level level = new Level();

        foreach (var prefab in parent.GetComponentsInChildren<LevelElement>()) {
            float yOffset = (prefab.Data is FieldObstacleData) ? ((FieldObstacleData)prefab.Data).Length : prefab.GetComponent<Renderer>().bounds.extents.x;
            LevelElementInfo tempElement = new LevelElementInfo {
                Data = prefab.Data,
                X = prefab.transform.localPosition.x,
                Y = prefab.transform.localPosition.y - yOffset,
                PrefabName = GetPrefabPath(prefab.gameObject)
            };

            level.Elements.Add(tempElement);
            
        }
        level.Elements.Sort();
        string levelString = JsonConvert.SerializeObject(level, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto });
        File.WriteAllText(Application.dataPath + "/Resources/Levels/" + levelName + ".json", levelString);
        AssetDatabase.Refresh();
    }
}
