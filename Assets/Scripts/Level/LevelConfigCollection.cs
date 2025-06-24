using UnityEngine;
using System.Collections.Generic;


[CreateAssetMenu(fileName ="levels", menuName ="level",order =1)]
public class LevelConfigCollection:ScriptableObject
{
    [SerializeField] private List<LevelConfig> allLevels;

    public IReadOnlyList<LevelConfig> AllLevels => allLevels;

    public LevelConfig GetLevelConfig(int levelNumber)
    {
        return allLevels?.Find(l => l.level == levelNumber);
    }


}
