using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLevelEditor : EditorWindow
{
    [MenuItem("Window/Scene Level Editor")]
    public static void OpenLevelEditor() => GetWindow<SceneLevelEditor>();

    private List<GameObject> addedPrefabs;

    SerializedObject so;
    SerializedProperty levelElementParent;

    public GameObject parent;

    private void OnEnable()
    {
        so = new SerializedObject(this);
        levelElementParent = so.FindProperty("parent");
    }

    private void OnSelectionChange()
    {

        foreach (var prefab in Selection.gameObjects) {
            if (addedPrefabs.Contains(prefab)) continue;
            if (prefab.gameObject.scene.name == null || prefab.gameObject.scene.name == prefab.gameObject.name) continue;
            if (!prefab.TryGetComponent(out LevelElement _)) continue;
            if (prefab.transform.parent == parent) continue;
            prefab.transform.parent = parent.transform;
            prefab.transform.Rotate(0,0,-90);
            addedPrefabs.Add(prefab);
        }
    }

    private void OnGUI()
    {
        so.Update();
        EditorGUILayout.PropertyField(levelElementParent);
        so.ApplyModifiedProperties();
        if (GUILayout.Button("Save")) {
            SaveLevel();
        }
    }

    private void SaveLevel()
    {
        throw new NotImplementedException();
    }
}
