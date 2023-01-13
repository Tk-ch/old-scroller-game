using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Newtonsoft.Json;
using System;
using System.Reflection;
using System.IO;
using System.Linq;
/// <summary>
/// Solely used to show the X position of a Level Element in the Scene view
/// </summary>
[CustomEditor(typeof(Player))]
public class ObjectPosition : Editor {
    private void OnSceneGUI()
    {
        Handles.color = Color.red;
        Handles.DrawLine(new Vector3(LevelEditor.x, -10), new Vector3(LevelEditor.x, 10), 2);
        SceneView.RepaintAll();
    }
}

/// <summary>
/// Huh, a level editor. Makes a window to make some levels
/// </summary>
public class LevelEditor : EditorWindow
{
    string levelName = "0001";

    Level level;

    public static float x, y = 0;
    
    float levelSize = 1000;

    float SnapY = 1f;
    float SnapX = 1f;

    GameObject prefab = null;

    Vector2 scroll = new Vector2();

    GUILayoutOption expandDefault = GUILayout.ExpandWidth(false);

    LevelElementInfo selectedLevelElement;

    Rect sliderRect;

    SerializedObject so;

    public GameObject Prefab // It just works
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
            }
            prefab = value;

            UpdateProperties();
        }
    }

    [MenuItem("Window/Level Editor")]
    static void Init() { //Boilerplate
        LevelEditor window = (LevelEditor)GetWindow(typeof(LevelEditor));
        window.titleContent = new GUIContent(text: "Level Editor");
        window.Show();
    }

    // Methods that load and save the level, whatever
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

    // Generates a dictionary of properties for a given Level Element. 
    // Serialized fields are skipped and assumed to be filled in the prefab
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

    // Updates a dictionary of properties for the current Level Element. 
    // Serialized fields are skipped and assumed to be filled in the prefab
    void UpdateProperties() {
        LevelElement levelElement = Prefab.GetComponent<LevelElement>();
        so = new SerializedObject(levelElement);
        foreach (var el in levelElement.GetType().GetRuntimeFields())
        {
            if (so.FindProperty(el.Name) != null || el.GetValue(levelElement) == null || selectedLevelElement.Properties.ContainsKey(el.Name)) continue;
            selectedLevelElement.Properties.Add(el.Name, el.GetValue(levelElement));
        }
     }


    // Adds an EditorGUI field to edit a Property
    void AddField(KeyValuePair<string, object> field)
    {
        if (field.Value.GetType() == typeof(long) || field.Value.GetType() == typeof(int)) selectedLevelElement.Properties[field.Key] = EditorGUILayout.LongField(label: field.Key, Convert.ToInt64(selectedLevelElement.Properties[field.Key]), expandDefault);
        if (field.Value.GetType() == typeof(float) || field.Value.GetType() == typeof(double)) selectedLevelElement.Properties[field.Key] = EditorGUILayout.FloatField(label: field.Key, Convert.ToSingle(selectedLevelElement.Properties[field.Key]), expandDefault);
        if (field.Value.GetType() == typeof(string)) selectedLevelElement.Properties[field.Key] = EditorGUILayout.TextField(label: field.Key, (string)selectedLevelElement.Properties[field.Key], expandDefault);
    }

   


    // Just to list all the property GUI fields
    void ListProperties()
    {
        foreach (var el in selectedLevelElement.Properties.ToList())
            AddField(el);
    }

    // Select a level element from loaded Level Elements
    void SelectElement(LevelElementInfo element) {
        x = element.X;
        y = element.Y;
        selectedLevelElement = element;
        if (Prefab == null || element.PrefabName != Prefab.name)
        {
            GUI.FocusControl(null);
            Prefab = Resources.Load("Prefabs/" + element.PrefabName, typeof(GameObject)) as GameObject;
        }
    }

    // List all the elements in the level
    void ListElements()
    {
        foreach (var el in level.Elements)
            using (new GUILayout.HorizontalScope())
            {
                if (GUILayout.Button(string.Format("{0}: Y({1}), X({2})", el.PrefabName ,el.Y, el.X))) SelectElement(el);
                if (GUILayout.Button("Delete", expandDefault))
                {
                    level.Elements.Remove(el);
                    selectedLevelElement = null;
                    break;
                };
            }
        
    }

    /// <summary>
    /// Draws the editor window with the level elements and their properties
    /// </summary>
    void OnGUI()
    {
        if (level == null) LoadLevel();
        level.Elements.Sort(); // TODO: only sort if a change is detected

        GUILayout.Label("Nebuloic Level Editor", EditorStyles.boldLabel);

        using (new EditorGUILayout.HorizontalScope())
        {
            GUILayout.Label("Level Name: ", expandDefault);
            levelName = GUILayout.TextField(levelName, GUILayout.Width(100), expandDefault);
            if (GUILayout.Button("Load", expandDefault)) LoadLevel();
        }

        levelSize = EditorGUILayout.FloatField("Level Size: ", levelSize, expandDefault);
        
        SnapY = SnapControls("SNAP Y", SnapY);
        SnapX = SnapControls("SNAP X", SnapX);

        y = SnapY <= 0 ? EditorGUILayout.Slider("Y Coordinate", y, 0, levelSize) : Mathf.Round(EditorGUILayout.Slider("Y Coordinate", y, 0, levelSize) / SnapY) * SnapY;

        sliderRect = GUILayoutUtility.GetLastRect();


        Vector2 start = sliderRect.min + new Vector2(150, 0);
        Vector2 end = sliderRect.max - new Vector2(55, 0);
        if (level.Elements.Count > 0)
        {
            foreach (var el in level.Elements)
            {
                DrawElement(start, end, el);
            }
        }

        x = SnapX <= 0 ? EditorGUILayout.Slider("X Coordinate", x, -3, 3, expandDefault) : Mathf.Round(EditorGUILayout.Slider("X Coordinate", x, -3, 3, expandDefault) / SnapX) * SnapX;

        Prefab = EditorGUILayout.ObjectField("Select Prefab: ", Prefab, typeof(GameObject), false, GUILayout.Width(300), expandDefault) as GameObject;

        using (new EditorGUILayout.HorizontalScope())
        {
            using (new EditorGUILayout.VerticalScope(GUILayout.Width(position.width / 2), expandDefault))
            {
                if (Prefab != null && selectedLevelElement == null)
                    CreateAddButton();
                if (selectedLevelElement != null)
                    UpdateSelectedLevelElement();
            }

            scroll = GUILayout.BeginScrollView(scroll, GUILayout.Width(position.width / 2), expandDefault);
            ListElements();
            GUILayout.EndScrollView();
        }
        if (GUILayout.Button("Save File", expandDefault)) SaveLevel();
    }

    private void DrawElement(Vector2 start, Vector2 end, LevelElementInfo el)
    {
        Color color = Color.HSVToRGB(Mathf.Abs(el.PrefabName.GetHashCode()) % 20 / 20f, 1.0f, 1.0f);
        Color outlineColor = el == selectedLevelElement ? Color.white : Color.black;
        if (el.Properties.TryGetValue("length", out object v)) // if the element is a field, draw a rect
        {
            float length = Convert.ToSingle(v) * (end.x - start.x) / levelSize;
            Handles.DrawSolidRectangleWithOutline(new Rect(new Vector2(Mathf.Max(Mathf.Lerp(start.x, end.x, el.Y / levelSize), 0), start.y), new Vector2(length, end.y - start.y)), new Color(1, 1, 1, 0.3f), outlineColor);
        }
        else // Draw a rectangle on Y coordinate slider to indicate the position of each element
        {
            Handles.DrawSolidRectangleWithOutline(
                new Rect(Mathf.Lerp(start.x, end.x, el.Y / levelSize) - 2.5f, Mathf.Lerp(start.y, end.y, (el.X + 3) / 6) - 2.5f, 5, 5),
                color,
                outlineColor);
        }
    }

    private float SnapControls(string label, float value)
    {
        using (new EditorGUILayout.HorizontalScope(expandDefault))
        {
            value = EditorGUILayout.FloatField(label, value, expandDefault);
            if (GUILayout.Button("NO", expandDefault)) {
                value = -1;
            }
            if (GUILayout.Button("0.25", expandDefault))
            {
                value = 0.25f;
            }
            if (GUILayout.Button("1.0", expandDefault)) {
                value = 1;
            }
            if (GUILayout.Button("2.5", expandDefault)) {
                value = 2.5f;
            }
            if (GUILayout.Button("5", expandDefault)) {
                value = 5;
            }
        }
        return value;
    }

    //Updates all the properties of a selected levelElementInfo
    private void UpdateSelectedLevelElement()
    {
        selectedLevelElement.X = x;
        selectedLevelElement.Y = y;
        ListProperties();

        using (new EditorGUILayout.HorizontalScope(expandDefault))
        {
            if (GUILayout.Button("Copy", expandDefault))
            {
                Dictionary<string, object> temp = new Dictionary<string, object>();
                foreach (string key in selectedLevelElement.Properties.Keys)
                {
                    temp.Add(key, selectedLevelElement.Properties[key]);
                }

                selectedLevelElement = new LevelElementInfo
                {
                    X = selectedLevelElement.X,
                    Y = selectedLevelElement.Y,
                    PrefabName = selectedLevelElement.PrefabName,
                    Properties = temp
                };

                level.Elements.Add(selectedLevelElement);
            }
            if (GUILayout.Button("Ok", expandDefault))
            {
                selectedLevelElement = null;
            }
        }
    }

    /// <summary>
    /// Just a function to create a new level element
    /// </summary>
    private void CreateAddButton()
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

}
