
using UnityEngine;
using System;



#region Game-services

public static class SettingsConstants
{
    #region Settings
    private const string SoundEnabledKey = "SoundEnabled";
    private const string MusicEnabledKey = "MusicEnabled";
    private const string VibrationEnabledKey = "VibrationEnabled";
    private const string LanguageKey = "Language";
    #endregion


    #region Settings
    public static bool IsSoundOn
    {
        get => PlayerPrefs.GetInt(SoundEnabledKey, 1) == 1;
        set => PlayerPrefs.SetInt(SoundEnabledKey, value ? 1 : 0);
    }

    public static bool IsMusicOn
    {
        get => PlayerPrefs.GetInt(MusicEnabledKey, 1) == 1;
        set => PlayerPrefs.SetInt(MusicEnabledKey, value ? 1 : 0);
    }

    public static bool IsVibrationOn
    {
        get => PlayerPrefs.GetInt(VibrationEnabledKey, 1) == 1;
        set => PlayerPrefs.SetInt(VibrationEnabledKey, value ? 1 : 0);
    }

    public static string Language
    {
        get => PlayerPrefs.GetString(LanguageKey, "en");
        set => PlayerPrefs.SetString(LanguageKey, value);
    }
    #endregion


    #region Utility Methods
    public static void Save() => PlayerPrefs.Save();

    public static void ResetAll()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
    }


    public static void ResetSettings()
    {
        PlayerPrefs.DeleteKey(SettingsConstants.SoundEnabledKey);
        PlayerPrefs.DeleteKey(SettingsConstants.MusicEnabledKey);
        PlayerPrefs.DeleteKey(SettingsConstants.VibrationEnabledKey);
        PlayerPrefs.DeleteKey(SettingsConstants.LanguageKey);
        Save();
    }



    #endregion

}

#endregion

