
using System;
using UnityEngine;


#region Game-services
public static class JokerCosts
{
    public const int ExtraTime = 100;
    public const int ExtraMoves = 100;
    public const int ExtraSlot = 25;
}

public static class Constants
{
    
    #region GameEconomy
    public const string CurrentCoinKey = "CurrentCoin";
    public const string CurrentLifeKey = "CurrentLife";
    #endregion

    #region Game Progress
    public const string CurrentLevelKey = "CurrentLevel";
    public const string HighestLevelKey = "HighestLevel";
    #endregion

    #region Game Session
    public const string RemainingMovesKey = "Moves";
    public const string TimerStartKey = "Timer";
    
    public const string TargetWhiteKey = "TargetWhite";
    #endregion
    
    #region  joker

    #endregion

    #region Settings
    public const string SoundEnabledKey = "SoundEnabled";
    public const string MusicEnabledKey = "MusicEnabled";
    public const string VibrationEnabledKey = "VibrationEnabled";
    public const string LanguageKey = "Language";
    #endregion

    #region Player Stats
    public const string TotalScoreKey = "TotalScore";
    public const string LevelsCompletedKey = "LevelsCompleted";
    #endregion
}

public static class PlayerPrefsService
{
    #region GameEconomy
    public static int CurrentCoin
    {
        get => PlayerPrefs.GetInt(Constants.CurrentCoinKey, 100);
        set => PlayerPrefs.SetInt(Constants.CurrentCoinKey, Mathf.Max(0, value));
    }
    public static int CurrentLife
    {
        get => PlayerPrefs.GetInt(Constants.CurrentLifeKey, 5);
        set => PlayerPrefs.SetInt(Constants.CurrentLifeKey, Mathf.Clamp(value, 0, 5));
    }
    #endregion
   
    #region Game Progress
    public static int CurrentLevel
    {
        get => PlayerPrefs.GetInt(Constants.CurrentLevelKey, 1);
        set
        {
            PlayerPrefs.SetInt(Constants.CurrentLevelKey, Math.Clamp(value, 1, 99));
            if (value > HighestLevel)
            {
                HighestLevel = value;
            }
        }
    }

    public static int HighestLevel
    {
        get => PlayerPrefs.GetInt(Constants.HighestLevelKey, 1);
        private set => PlayerPrefs.SetInt(Constants.HighestLevelKey,  Math.Clamp(value, 1,99));
    }

    #endregion

    #region Game Session


    public static int RemainingMoves
    {
        get => PlayerPrefs.GetInt(Constants.RemainingMovesKey, 20);
        set => PlayerPrefs.SetInt(Constants.RemainingMovesKey, Mathf.Max(0, value));
    }

    public static int TimerStart
    {
        get => PlayerPrefs.GetInt(Constants.TimerStartKey, 60);
        set => PlayerPrefs.SetInt(Constants.TimerStartKey, Mathf.Max(0, value));
    }
    public static int TargetWhite
    {
        get => PlayerPrefs.GetInt(Constants.TargetWhiteKey, 1);
        set => PlayerPrefs.SetInt(Constants.TargetWhiteKey, Mathf.Max(1, value));
    }
    #endregion

    #region  joker


    #endregion

    #region Settings
    public static bool IsSoundOn
    {
        get => PlayerPrefs.GetInt(Constants.SoundEnabledKey, 1) == 1;
        set => PlayerPrefs.SetInt(Constants.SoundEnabledKey, value ? 1 : 0);
    }

    public static bool IsMusicOn
    {
        get => PlayerPrefs.GetInt(Constants.MusicEnabledKey, 1) == 1;
        set => PlayerPrefs.SetInt(Constants.MusicEnabledKey, value ? 1 : 0);
    }

    public static bool IsVibrationOn
    {
        get => PlayerPrefs.GetInt(Constants.VibrationEnabledKey, 1) == 1;
        set => PlayerPrefs.SetInt(Constants.VibrationEnabledKey, value ? 1 : 0);
    }

    public static string Language
    {
        get => PlayerPrefs.GetString(Constants.LanguageKey, "en");
        set => PlayerPrefs.SetString(Constants.LanguageKey, value);
    }
    #endregion

    #region Player Stats
    public static int TotalScore
    {
        get => PlayerPrefs.GetInt(Constants.TotalScoreKey, 0);
        set => PlayerPrefs.SetInt(Constants.TotalScoreKey, Mathf.Max(0, value));
    }

    public static int LevelsCompleted
    {
        get => PlayerPrefs.GetInt(Constants.LevelsCompletedKey, 0);
        set => PlayerPrefs.SetInt(Constants.LevelsCompletedKey, Mathf.Clamp(value, 0,99));
    }
    #endregion

    #region Utility Methods
    public static void Save() => PlayerPrefs.Save();

    public static void ResetAll()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
    }

    public static void ResetProgress()
    {
        PlayerPrefs.DeleteKey(Constants.CurrentLevelKey);
        PlayerPrefs.DeleteKey(Constants.HighestLevelKey);
        PlayerPrefs.DeleteKey(Constants.TotalScoreKey);
        PlayerPrefs.DeleteKey(Constants.LevelsCompletedKey);
        Save();
    }

    public static void ResetSettings()
    {
        PlayerPrefs.DeleteKey(Constants.SoundEnabledKey);
        PlayerPrefs.DeleteKey(Constants.MusicEnabledKey);
        PlayerPrefs.DeleteKey(Constants.VibrationEnabledKey);
        PlayerPrefs.DeleteKey(Constants.LanguageKey);
        Save();
    }

    public static void IncrementLevel()
    {
        CurrentLevel++;
    }

    public static void AddScore(int score)
    {
        TotalScore += score;
        if (score > 0)
        {
            LevelsCompleted++;
        }
    }
    #endregion

}
#endregion

