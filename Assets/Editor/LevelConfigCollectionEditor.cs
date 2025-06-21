
#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LevelConfigCollection))]
public class LevelConfigCollectionEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var collection = (LevelConfigCollection)target;

        if (GUILayout.Button("Auto Generate Seeds"))
        {
            foreach (var level in collection.AllLevels)
            {
                level.seed = SeedGenerator.GenerateSeed(level.targetWhiteTiles);
            }

            EditorUtility.SetDirty(collection);
        }

        DrawDefaultInspector();
    }
}
#endif




