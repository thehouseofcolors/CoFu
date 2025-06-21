using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;


[System.Serializable]
public class LevelConfig
{
    public int level;
    public int tiles_in_a_row;
    public int tiles_in_a_column;
    public int targetWhiteTiles;
    public float timeLimit;
    public int moveLimit;
    public string seed;
}

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
