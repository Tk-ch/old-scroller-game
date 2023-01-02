using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Newtonsoft.Json;
using System;
using System.Reflection;
using System.IO;
using System.Linq;

//Trying to refactor this shit
[CustomEditor(typeof(Player))]
public class ObjectPosition : Editor {
    private void OnSceneGUI()
    {
        Handles.color = Color.red;
        Handles.DrawLine(new Vector3(LevelEditor.x, -10), new Vector3(LevelEditor.x, 10), 2);
        SceneView.RepaintAll();
    }
}

public class LevelEditor : EditorWindow
{
    string levelName = "0001";

    Level level;

    public static float x, y = 0;
    
    float levelSize = 1000;

    GameObject prefab = null;

    Vector2 scroll = new Vector2();

    GUILayoutOption expandDefault = GUILayout.ExpandWidth(false);

    LevelElementInfo selectedLevelElement;

    Rect sliderRect;

    SerializedObject so;

    public GameObject Prefab
    {
        get => prefab; set {
            if (value == prefab) return;
            LevelElement levelElement;
            if (value == null || !value.TryGetComponent(out levelElement))
            {
                Debug.Log("Prefab is not a level element");
                return;
            }
            if (selectedLevelElement != null) {
                selectedLevelElement.PrefabName = value.name;
                UpdateProperties();
            }
            prefab = value;
        }
    }

    [MenuItem("Window/Level Editor")]
    static void Init() {
        LevelEditor window = (LevelEditor)GetWindow(typeof(LevelEditor));
        window.titleContent = new GUIContent(text: "Level Editor");
        window.Show();
    }


    void LoadLevel() {
        var levelString = (TextAsset)Resources.Load("Levels/" + levelName);
        if (levelString == null) {
            level = new Level();
            return;
        }
        level = JsonConvert.DeserializeObject<Level>(levelString.text, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto });
        selectedLevelElement = null;
    }

    void SaveLevel() {
        string levelString = JsonConvert.SerializeObject(level, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto });
        File.WriteAllText(Application.dataPath + "/Resources/Levels/" + levelName + ".json", levelString);
        AssetDatabase.Refresh();
    }


    void CreateProperties(LevelElement levelElement)
    {
        selectedLevelElement.Properties = new Dictionary<string, object>();
        so = new SerializedObject(levelElement);
        foreach (var el in levelElement.GetType().GetRuntimeFields())
        {
            if (so.FindProperty(el.Name) == null && el.GetValue(levelElement) != null)
                selectedLevelElement.Properties.Add(el.Name, el.GetValue(levelElement));
        }
    }

    void UpdateProperties() {
        LevelElement levelElement = Prefab.GetComponent<LevelElement>();
        so = new SerializedObject(levelElement);
        foreach (var el in levelElement.GetType().GetRuntimeFields())
        {
            if (so.FindProperty(el.Name) != null || el.GetValue(levelElement) == null || selectedLevelElement.Properties.ContainsKey(el.Name)) continue;
            selectedLevelElement.Properties.Add(el.Name, el.GetValue(levelElement));
        }
     }

    void AddField(KeyValuePair<string, object> field)
    {
        if (field.Value.GetType() == typeof(long) || field.Value.GetType() == typeof(int)) selectedLevelElement.Properties[field.Key] = EditorGUILayout.LongField(label: field.Key, Convert.ToInt64(selectedLevelElement.Properties[field.Key]), expandDefault);
        if (field.Value.GetType() == typeof(float) || field.Value.GetType() == typeof(double)) selectedLevelElement.Properties[field.Key] = EditorGUILayout.FloatField(label: field.Key, Convert.ToSingle(selectedLevelElement.Properties[field.Key]), expandDefault);
        if (field.Value.GetType() == typeof(string)) selectedLevelElement.Properties[field.Key] = EditorGUILayout.TextField(label: field.Key, (string)selectedLevelElement.Properties[field.Key], expandDefault);
    }

    void ListProperties()
    {
        foreach (var el in selectedLevelElement.Properties.ToList())
            AddField(el);
        
    }
    void SelectElement(LevelElementInfo element) {
        x = element.X;
        y = element.Y;
        selectedLevelElement = element;
        if (element.PrefabName != Prefab.name)
        {
            GUI.FocusControl(null);
            Prefab = Resources.Load("Prefabs/" + element.PrefabName, typeof(GameObject)) as GameObject;
        }
        UpdateProperties();
    }

    void ListElements()
    {
        foreach (var el in level.Elements)
        {

            using (new GUILayout.HorizontalScope())
            {
                if (GUILayout.Button(string.Format("{0}: y{1}, x{2}", el.PrefabName ,el.Y, el.X))) SelectElement(el);
                if (GUILayout.Button("Delete", expandDefault))
                {
                    level.Elements.Remove(el);
                    selectedLevelElement = null;
                    break;
                };
            }
        }
    }

    void OnGUI()
    {
        if (level == null) LoadLevel();
        level.Elements.Sort();

        GUILayout.Label("Nebuloic Level Editor", EditorStyles.boldLabel);

        GUILayout.BeginHorizontal();
        GUILayout.Label("Level Name: ", expandDefault);
        levelName = GUILayout.TextField(levelName, GUILayout.Width(100), expandDefault);
        if (GUILayout.Button("Load", expandDefault)) LoadLevel();
        GUILayout.EndHorizontal();
        levelSize = EditorGUILayout.FloatField("Level Size: ", levelSize, expandDefault);
        y = EditorGUILayout.Slider("Y Coordinate", y, 0, levelSize);
        sliderRect = GUILayoutUtility.GetLastRect();
        Vector2 start = sliderRect.min + new Vector2(150, 0);
        Vector2 end = sliderRect.max - new Vector2(55, 0);
        x = EditorGUILayout.Slider("X Coordinate", x, -3, 3, expandDefault);
        if (level.Elements.Count > 0)
        {
            foreach (var el in level.Elements) {
                Handles.DrawLine(new Vector2(Mathf.Lerp(start.x, end.x, el.Y/levelSize), start.y), new Vector2(Mathf.Lerp(start.x, end.x, el.Y / levelSize), end.y));
            }
        }

        Prefab = EditorGUILayout.ObjectField("Select Prefab: ", Prefab, typeof(GameObject), false, GUILayout.Width(300), expandDefault) as GameObject;
        GUILayout.BeginHorizontal();
        GUILayout.BeginVertical(GUILayout.Width(position.width/2), expandDefault);
        
        if (Prefab != null && selectedLevelElement == null)
        {
            if (GUILayout.Button("Add", expandDefault))
            {
                LevelElement levelElement = Prefab.GetComponent<LevelElement>();
                selectedLevelElement = new LevelElementInfo
                {
                    X = x,
                    Y = y,
                    PrefabName = Prefab.name,
                    Properties = { }
                };
                level.Elements.Add(selectedLevelElement);
                CreateProperties(levelElement);
            }
            
        }

        if (selectedLevelElement != null)
        {
            selectedLevelElement.X = x;
            selectedLevelElement.Y = y;
            ListProperties();
            GUILayout.BeginHorizontal(expandDefault);
            if (GUILayout.Button("Copy", expandDefault)) {
                Dictionary<string, object> temp = new Dictionary<string, object>();
                foreach (string key in selectedLevelElement.Properties.Keys)
                {
                    temp.Add(key, selectedLevelElement.Properties[key]);
                }

                selectedLevelElement = new LevelElementInfo {
                    X = selectedLevelElement.X,
                    Y = selectedLevelElement.Y,
                    PrefabName = selectedLevelElement.PrefabName,
                    Properties = temp
                };

                level.Elements.Add(selectedLevelElement);
            }
            if (GUILayout.Button("Ok", expandDefault)) {
                selectedLevelElement = null;
            }
            GUILayout.EndHorizontal();

        }


        GUILayout.EndVertical();
        //prefab.name is the name of the file

        scroll = GUILayout.BeginScrollView(scroll, GUILayout.Width(position.width/2), expandDefault);
        ListElements();

        GUILayout.EndScrollView();
        GUILayout.EndHorizontal();


        if (GUILayout.Button("Save File", expandDefault)) SaveLevel();
        

        

    }

   
}
