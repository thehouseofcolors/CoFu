using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class LevelConfig
{
    [SerializeField, Range(1, 99)] 
    private int level = 1;
    [SerializeField] private GridConfig gridConfig = new GridConfig();
    [SerializeField] private int colorCount;
    [SerializeField] private JokerConfig jokerConfig = new JokerConfig();

    public int Level => level;
    public GridConfig GridConfig => gridConfig;
    public int ColorCount => colorCount;
    public string Seed;
    public JokerConfig JokerConfig => jokerConfig;
}

[System.Serializable]
public struct GridConfig
{
    [SerializeField, Range(0, 5)] private int rows;
    [SerializeField, Range(0, 5)] private int columns;

    public int Rows => rows;
    public int Columns => columns;

    public GridConfig(int rows = 3, int columns = 3)
    {
        this.rows = Mathf.Clamp(rows, 0, 5);
        this.columns = Mathf.Clamp(columns, 0, 5);
    }

    public int TotalTiles => rows * columns;
}

[System.Serializable]
public struct JokerConfig
{
    public int TimeLimit;
    public int MoveCount;
    public int TargetWhite;
    public JokerConfig(int time, int move, int white)
    {
        TimeLimit = time;
        MoveCount = move;
        TargetWhite = white;
    }

}

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

