using System.Threading.Tasks;
using UnityEngine;

public class LevelManager : Singleton<LevelManager>, IGameSystem
{
    [Header("Configuration")]
    [SerializeField] private LevelConfigCollection _collection;
    private int _defaultFallbackLevel = 1;

    private LevelConfig _currentLevel;
    private int _currentLevelId;

    public LevelConfig CurrentLevel => _currentLevel;
    public int CurrentLevelId => _currentLevelId;

    public bool TrySetLevel(int levelId)
    {
        // int levelId = PlayerPrefsService.CurrentLevel;
        if (!TryGetLevelConfig(levelId, out var levelConfig))
        {
            Debug.LogWarning($"Failed to load level {levelId}, attempting fallback");
            return TryGetLevelConfig(_defaultFallbackLevel, out _currentLevel);
        }
        _currentLevel = levelConfig;
        _currentLevelId = levelId;
        return true;
    }

    private bool TryGetLevelConfig(int levelId, out LevelConfig config)
    {
        config = null;

        if (_collection == null)
        {
            Debug.LogError("Level collection not assigned in LevelManager!");
            return false;
        }

        if (levelId < 0 || levelId >= _collection.Count)
        {
            Debug.LogWarning($"Level ID {levelId} out of range (0-{_collection.Count - 1})");
            return false;
        }

        return _collection.TryGetLevel(levelId, out config);
    }

    public async Task Initialize()
    {
        TrySetLevel(PlayerPrefsService.CurrentLevel);
        await Task.CompletedTask;
    }
    public async Task Shutdown()
    {
        //savedata
        await Task.CompletedTask;
    }


}
