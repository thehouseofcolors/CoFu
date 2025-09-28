using UnityEngine;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameEvents;
using Unity.VisualScripting;



// [System.Serializable]
// public struct CurrentCurrencyData
// {
//     public CurrencyType Type;
//     public int Amount;

//     public CurrentCurrencyData(CurrencyType type, int amount)
//     {
//         Type = type;
//         Amount = amount;
//     }
// }

// [System.Serializable]
// public struct CurrentJokerData
// {
//     public RewardType Type;
//     public int Amount;

//     public CurrentJokerData(RewardType type, int amount)
//     {
//         Type = type;
//         Amount = amount;
//     }
// }

public static class PersistentResources
{
    private const string CurrentCoinKey = "CurrentCoin";
    private const string CurrentLifeKey = "CurrentLife";
    private const string CurrentSlotsKey = "CurrentSlots";
    private const string RemainingMovesKey = "Moves";
    private const string RemainingTimeKey = "Timer";

    // Default values
    private const int DefaultCoins = 100;
    private const int DefaultLives = 5;
    private const int DefaultSlots = 1;

    public static int Coins
    {
        get => PlayerPrefs.GetInt(CurrentCoinKey, DefaultCoins);
        set => PlayerPrefs.SetInt(CurrentCoinKey, Mathf.Max(0, value));
    }

    public static int Lives
    {
        get => PlayerPrefs.GetInt(CurrentLifeKey, DefaultLives);
        set => PlayerPrefs.SetInt(CurrentLifeKey, Mathf.Clamp(value, 0, 5));
    }

    public static int Slots
    {
        get => PlayerPrefs.GetInt(CurrentSlotsKey, DefaultSlots);
        set => PlayerPrefs.SetInt(CurrentSlotsKey, Mathf.Clamp(value, 1, 5));
    }

    public static int Moves
    {
        get => PlayerPrefs.GetInt(RemainingMovesKey, 0);
        set => PlayerPrefs.SetInt(RemainingMovesKey, Mathf.Max(0, value));
    }

    public static int Time
    {
        get => PlayerPrefs.GetInt(RemainingTimeKey, 0);
        set => PlayerPrefs.SetInt(RemainingTimeKey, Mathf.Max(0, value));
    }

    public static void Save() => PlayerPrefs.Save();
}

public static class ResourceManager
{
    public static void InitializeLevelResources(LevelConfig levelConfig)
    {
        if (levelConfig == null)
        {
            Debug.LogError("LevelConfig is null!");
            return;
        }

        // Deduct life when starting a level
        PersistentResources.Lives--;

        // Reset slots (or set from config if needed)
        PersistentResources.Slots = 1;

        // Set level-specific resources
        PersistentResources.Time = levelConfig.LevelTargetConfig.TimeLimit;
        PersistentResources.Moves = levelConfig.LevelTargetConfig.MoveLimit;
    }


    public static int GetCurrency(CurrencyType type)
    {
        return type switch
        {
            CurrencyType.Coin => PersistentResources.Coins,
            CurrencyType.Life => PersistentResources.Lives,
            _ => throw new System.ArgumentException($"Unsupported currency type: {type}")
        };
    }

    public static void SetCurrency(CurrencyType type, int amount)
    {
        switch (type)
        {
            case CurrencyType.Coin:
                PersistentResources.Coins = amount;
                break;
            case CurrencyType.Life:
                PersistentResources.Lives = amount;
                break;
            default:
                Debug.LogWarning($"Unrecognized currency type: {type}");
                break;
        }
    }

    public static void AddCurrency(CurrencyType type, int amount)
    {
        if (amount < 0)
        {
            Debug.LogWarning($"Use TrySpend() for negative amounts. Amount: {amount}");
            return;
        }

        switch (type)
        {
            case CurrencyType.Coin:
                PersistentResources.Coins += amount;
                break;
            case CurrencyType.Life:
                PersistentResources.Lives += amount;
                break;
            default:
                Debug.LogWarning($"Unrecognized currency type: {type}");
                break;
        }
    }

    public static bool TrySpend(CurrencyType type, int amount)
    {
        if (amount <= 0)
        {
            Debug.LogWarning($"Amount must be positive. Amount: {amount}");
            return false;
        }

        if (GetCurrency(type) < amount)
            return false;

        AddCurrency(type, -amount);
        return true;
    }

    public static void UpdateReward(RewardType rewardType, int change)
    {
        switch (rewardType)
        {
            case RewardType.Time:
                PersistentResources.Time += change;
                break;
            case RewardType.Moves:
                PersistentResources.Moves += change;
                break;
            case RewardType.Slot:
                PersistentResources.Slots += change;
                break;
            default:
                Debug.LogWarning($"Unrecognized reward type: {rewardType}");
                break;
        }
    }

    public static void SaveGame() => PersistentResources.Save();
    
}



// [System.Serializable]
// public struct CurrentCurrencyData
// {
//     public CurrencyType Type;
//     public int Amount;

//     public CurrentCurrencyData(CurrencyType type, int amount)
//     {
//         Type = type;
//         Amount = amount;
//     }
// }

// [System.Serializable]
// public struct CurrentJokerData
// {
//     public RewardType Type;
//     public int Amount;

//     public CurrentJokerData(RewardType type, int amount)
//     {
//         Type = type;
//         Amount = amount;
//     }
// }

// public static class Resources
// {
//     const string CurrentCoinKey = "CurrentCoin";
//     const string CurrentLifeKey = "CurrentLife";
//     const string CurrentSlotsKey = "CurrentSlots";
//     const string RemainingMovesKey = "Moves";
//     const string RemainingTimeKey = "Timer";
//     // Default values
//     private const int DefaultCoins = 100;
//     private const int DefaultLives = 5;
//     private const int DefaultSlots = 1;


//     public static int CurrentCoin
//     {
//         get => PlayerPrefs.GetInt(CurrentCoinKey, DefaultCoins);
//         set => PlayerPrefs.SetInt(CurrentCoinKey, Mathf.Max(0, value));
//     }
//     public static int CurrentLife
//     {
//         get => PlayerPrefs.GetInt(CurrentLifeKey, DefaultLives);
//         set => PlayerPrefs.SetInt(CurrentLifeKey, Mathf.Clamp(value, 0, 5));
//     }
//     public static int CurrentSlots
//     {
//         get => PlayerPrefs.GetInt(CurrentSlotsKey, DefaultSlots);
//         set => PlayerPrefs.SetInt(CurrentSlotsKey, Math.Clamp(value, 1, 5));
//     }

//     public static int RemainingMoves
//     {
//         get => PlayerPrefs.GetInt(RemainingMovesKey, 0);
//         set => PlayerPrefs.SetInt(RemainingMovesKey, Mathf.Max(0, value));
//     }
//     public static int RemainingTime
//     {
//         get => PlayerPrefs.GetInt(RemainingTimeKey, 0);
//         set => PlayerPrefs.SetInt(RemainingTimeKey, Mathf.Max(0, value));
//     }


// }


// public static class ResourcesManager
// {
//     // private static Dictionary<CurrencyType, int> _balances = new();
//     // private static List<CurrentCurrencyData> currency = new List<CurrentCurrencyData>();
//     // private static List<CurrentJokerData> _jokers;//her level başında o levenin jokerlerine eşitle

//     // public static void InitializeResources(LevelConfig levelConfig)
//     // {
//     //     //level başında çağır
//     //     Resources.CurrentLife -= 1;
//     //     Resources.CurrentSlots = 1;
//     //     Resources.RemainingTime = levelConfig.LevelTargetConfig.TimeLimit;
//     //     Resources.RemainingMoves = levelConfig.LevelTargetConfig.MoveLimit;

//     // }

//     public static void InitializeLevelResources(LevelConfig levelConfig)
//     {
//         if (levelConfig == null)
//         {
//             Debug.LogError("LevelConfig is null!");
//             return;
//         }

//         // Deduct life when starting a level
//         Resources.CurrentLife--;
        
//         // Reset slots (or set from config if needed)
//         Resources.CurrentSlots = 1;
        
//         // Set level-specific resources
//         Resources.RemainingTime = levelConfig.LevelTargetConfig.TimeLimit;
//         Resources.RemainingMoves = levelConfig.LevelTargetConfig.MoveLimit;
//     }
//     public static int GetCurrencyAmount(CurrencyType type)
//     {
//         switch (type)
//         {
//             case CurrencyType.Coin:
//                 return Resources.CurrentCoin;
//             case CurrencyType.Life:
//                 return Resources.CurrentLife;
//             default:
//                 Debug.LogWarning($"Unrecognized currency type: {type}");
//                 return 0;
//         }
//     }
//     public static void SetCurrencyAmount(CurrencyType type, int amount)
//     {
//         switch (type)
//         {
//             case CurrencyType.Coin:
//                 Resources.CurrentCoin = amount;
//                 break;
//             case CurrencyType.Life:
//                 Resources.CurrentLife = amount;
//                 break;
//             default:
//                 Debug.LogWarning($"Unrecognized currency type: {type}");
//                 break;
//         }
//     }
//     public static void AddCurrencyAmount(CurrencyType type, int amount)
//     {
//         switch (type)
//         {
//             case CurrencyType.Coin:
//                 Resources.CurrentCoin += amount;
//                 break;
//             case CurrencyType.Life:
//                 Resources.CurrentLife += amount;
//                 break;
//             default:
//                 Debug.LogWarning($"Unrecognized currency type: {type}");
//                 break;
//         }
//     }

//     public static bool TrySpend(CurrencyType type, int amount)
//     {
//         if (GetCurrencyAmount(type) < amount)
//             return false;

//         AddCurrencyAmount(type, -amount);
//         Save();
//         return true;
//     }
//     private static void Save()
//     {

//     }
//     static void SaveCurrency(CurrentCurrencyData data)
//     {
//         CurrencyType type = data.Type;
//         switch (type)
//         {
//             case CurrencyType.Coin:
//                 Resources.CurrentCoin = data.Amount;
//                 break;
//             case CurrencyType.Life:
//                 Resources.CurrentLife = data.Amount;
//                 break;
//             default:
//                 Debug.LogWarning($"Unrecognized currency type: {type}");
//                 break;
//         }
//     }

//     static void InitializeJokersJoker(CurrentJokerData data)
//     {
//         RewardType type = data.Type;
//         switch (type)
//         {
//             case RewardType.Time:
//                 Resources.RemainingTime = data.Amount;
//                 break;
//             case RewardType.Moves:
//                 Resources.RemainingMoves = data.Amount;
//                 break;
//             default:
//                 Debug.LogWarning($"Unrecognized currency type: {type}");
//                 break;
//         }
//     }
//     static void UpdateReward(RewardType rewardType, int change)
//     {
//         switch (rewardType)
//         {
//             case RewardType.Time:
//                 Resources.RemainingTime += change;
//                 break;
//             case RewardType.Moves:
//                 Resources.RemainingMoves += change;
//                 break;
//             default:
//                 Debug.LogWarning($"Unrecognized type: {rewardType}");
//                 break;
//         }
//     }


// }

