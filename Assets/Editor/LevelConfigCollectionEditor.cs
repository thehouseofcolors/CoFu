#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LevelConfigCollection))]
public class LevelConfigCollectionEditor : Editor
{
    private SerializedProperty _levelsProperty;
    private bool _showTools = true;
    private bool _showAdvanced = false;

    private void OnEnable()
    {
        _levelsProperty = serializedObject.FindProperty("_levels");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        
        var collection = (LevelConfigCollection)target;

        // Tools foldout
        _showTools = EditorGUILayout.Foldout(_showTools, "Level Tools", true);
        if (_showTools)
        {
            EditorGUILayout.BeginVertical("Box");
            {
                // Auto-generate seeds button
                if (GUILayout.Button("Generate All Seeds", GUILayout.Height(30)))
                {
                    Undo.RecordObject(collection, "Generate Seeds");
                    foreach (var level in collection.Levels)
                    {
                        if (level != null)
                        {
                            level.Seed = SeedGenerator.GenerateSeed(level.ColorCount);
                        }
                    }
                    EditorUtility.SetDirty(collection);
                }

                // Validate levels button
                if (GUILayout.Button("Validate All Levels", GUILayout.Height(25)))
                {
                    ValidateLevels(collection);
                }

                // Sort levels button
                if (GUILayout.Button("Sort Levels by Number", GUILayout.Height(25)))
                {
                    Undo.RecordObject(collection, "Sort Levels");
                    collection.SortLevels();
                }
            }
            EditorGUILayout.EndVertical();
        }

        // Advanced options
        _showAdvanced = EditorGUILayout.Foldout(_showAdvanced, "Advanced", true);
        if (_showAdvanced)
        {
            EditorGUILayout.BeginVertical("Box");
            EditorGUILayout.PropertyField(_levelsProperty, true);
            EditorGUILayout.EndVertical();
        }
        else
        {
            DrawDefaultInspector();
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void ValidateLevels(LevelConfigCollection collection)
    {
        bool hasErrors = false;
        HashSet<int> levelNumbers = new HashSet<int>();

        foreach (var level in collection.Levels)
        {
            if (level == null)
            {
                Debug.LogError("Null level found in collection!");
                hasErrors = true;
                continue;
            }

            // Check for duplicate level numbers
            if (!levelNumbers.Add(level.Level))
            {
                Debug.LogError($"Duplicate level number found: {level.Level}");
                hasErrors = true;
            }

            // Validate target white tiles
            int maxPossibleTiles = level.GridConfig.Rows * level.GridConfig.Columns;
            if (level.ColorCount > maxPossibleTiles)
            {
                Debug.LogError($"Level {level.Level}: Target white tiles ({level.ColorCount}) exceeds grid capacity ({maxPossibleTiles})");
                hasErrors = true;
            }
        }

        if (!hasErrors)
        {
            Debug.Log("All levels validated successfully!");
        }
    }
}

#endif

