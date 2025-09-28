using UnityEngine;
using System.Collections.Generic;



[CreateAssetMenu(fileName = "LevelCollection", menuName = "Game/Level Collection", order = 1)]
public class LevelConfigCollection : ScriptableObject
{

    [SerializeField] private List<LevelConfig> _levels = new List<LevelConfig>();

    public IReadOnlyList<LevelConfig> Levels => _levels.AsReadOnly();
    public int Count => _levels.Count;

    private LevelConfig GetLevel(int levelNumber)
    {
        if (levelNumber < 1 || levelNumber > _levels.Count)
        {
            Debug.LogWarning($"Level {levelNumber} doesn't exist in collection!");
            return null;
        }
        return _levels.Find(level => level.Level == levelNumber);
    }

    public bool TryGetLevel(int levelNumber, out LevelConfig levelConfig)
    {
        levelConfig = GetLevel(levelNumber);
        return levelConfig != null;
    }

#if UNITY_EDITOR
    public void AddLevel(LevelConfig level)
    {
        if (!_levels.Contains(level))
        {
            _levels.Add(level);
            UnityEditor.EditorUtility.SetDirty(this);
        }
    }

    public void RemoveLevel(LevelConfig level)
    {
        if (_levels.Remove(level))
        {
            UnityEditor.EditorUtility.SetDirty(this);
        }
    }

    public void SortLevels()
    {
        _levels.Sort((a, b) => a.Level.CompareTo(b.Level));
        UnityEditor.EditorUtility.SetDirty(this);
    }
#endif
}

