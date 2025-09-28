using UnityEngine;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;
using UnityEngine.UI;

#region  Extensions
public static class ListExtensions
{
    public static void Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        for (int i = n - 1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i + 1);
            T temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }
    }
    public static void Shuffle<T>(this IList<T> list, System.Random rng)
    {
        int n = list.Count;
        for (int i = n - 1; i > 0; i--)
        {
            int j = rng.Next(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }

    public static T PickRandom<T>(this IList<T> list)
    {
        if (list == null || list.Count == 0)
            throw new System.InvalidOperationException("Liste boş!");

        return list[UnityEngine.Random.Range(0, list.Count)];
    }
    public static bool IsNullOrEmpty<T>(this IList<T> list)
    {
        return list == null || list.Count == 0;
    }

}

public static class TaskExtensions
{
    public static void Forget(this Task task)
    {
        task.ContinueWith(t =>
        {
            if (t.IsFaulted) Debug.LogException(t.Exception);
        }, TaskContinuationOptions.OnlyOnFaulted);
    }
}

#endregion

#region  Color-System
public struct ColorVector
{
    public readonly int R, G, B;
    public bool IsNull;

    public static readonly ColorVector Null = new ColorVector { IsNull = true };

    public ColorVector(int r, int g, int b)
    {
        R = r;
        G = g;
        B = b;
        IsNull = false;
    }


    // Modify your existing methods to handle null case
    public Color ToUnityColor()
    {
        return IsNull ? Color.clear : new Color(R, G, B);
    }

    // Update operator overloads to handle null cases
    public static ColorVector operator +(ColorVector a, ColorVector b)
    {
        if (a.IsNull || b.IsNull) return Null;
        return new ColorVector(a.R + b.R, a.G + b.G, a.B + b.B);
    }

    public static bool operator ==(ColorVector a, ColorVector b)
    {
        return a.R == b.R && a.G == b.G && a.B == b.B;
    }

    public static bool operator !=(ColorVector a, ColorVector b)
    {
        return !(a == b);
    }

    // Similarly update other operators and properties
    public bool IsWhite => !IsNull && R == 1 && G == 1 && B == 1;

    public bool IsValidColor => !IsNull &&
        (R == 0 || R == 1) && (G == 0 || G == 1) && (B == 0 || B == 1)
        && (R + G + B > 0); // Siyah değil
    public bool IsBaseColor =>
        (R + G + B == 1) && IsValidColor;
    public bool IsIntermediateColor =>
        (R + G + B == 2) && IsValidColor;


    public override string ToString() =>
        IsNull ? "Null" : $"({R}, {G}, {B})";

    public bool Equals(ColorVector other) => this == other;

    public override bool Equals(object obj) =>
        obj is ColorVector other && Equals(other);

    public override int GetHashCode() =>
        HashCode.Combine(R, G, B);
}
public static class ColorFusion
{
    public static bool CanFuse(ColorVector a, ColorVector b)
    {
        if (a.IsNull || b.IsNull) return false;
        if (a == b) return false;

        var merged = a + b;
        return merged.IsValidColor;
    }

    public static ColorVector Fuse(ColorVector a, ColorVector b)
    {
        if (CanFuse(a, b))
        {
            return new ColorVector(
                Mathf.Clamp(a.R + b.R, 0, 1),
                Mathf.Clamp(a.G + b.G, 0, 1),
                Mathf.Clamp(a.B + b.B, 0, 1)
            );
        }
        return new ColorVector(0, 0, 0); // ya da ColorVector.Invalid


    }
}

public static class ColorPalette
{
    private static readonly ColorVector[] palette = new ColorVector[]
    {
        new ColorVector(1, 0, 0),
        new ColorVector(0, 1, 0),
        new ColorVector(0, 0, 1),
        new ColorVector(1, 1, 0),
        new ColorVector(0, 1, 1),
        new ColorVector(1, 0, 1)
    };

    public static ColorVector GetColorVector(int index) => palette[index];
    public static int GetColorIndex(ColorVector vector) => Array.IndexOf(palette, vector);
    public static ColorVector GetRandomColor()
    {
        return palette.PickRandom<ColorVector>();
    }
    public static int PaletteSize => palette.Count();

}

#endregion

#region Grid-System
public static class PaintManager
{
    public static async Task PaintTiles(
        List<Tile> tiles,
        Stack<ColorVector> colors,
        int delayBetweenStepsMs = 300,
        CancellationToken cancellationToken = default)
    {
        if (tiles == null) throw new ArgumentNullException(nameof(tiles));
        if (colors == null) throw new ArgumentNullException(nameof(colors));
        if (delayBetweenStepsMs < 0) throw new ArgumentOutOfRangeException(nameof(delayBetweenStepsMs));

        try
        {
            // Process colors until none left or cancelled
            while (colors.Count > 0 && !cancellationToken.IsCancellationRequested)
            {
                foreach (var tile in tiles)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    if (colors.Count == 0) break;

                    var color = colors.Pop();
                    await PaintTile(tile, color, delayBetweenStepsMs, cancellationToken);
                }
            }
        }
        catch (OperationCanceledException)
        {
            Debug.Log("Painting operation was cancelled");
            throw;
        }
        catch (Exception ex)
        {
            Debug.LogError($"Painting failed: {ex.Message}");
            throw;
        }
    }

    private static async Task PaintTile(
        Tile tile,
        ColorVector color,
        int delayMs,
        CancellationToken cancellationToken)
    {
        try
        {
            // Execute on main thread with cancellation support
            await MainThreadDispatcher.EnqueueAsync(() =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                tile.PushColor(color);
                tile.UpdateVisual();
            }, cancellationToken);

            // Add delay between paint operations if needed
            if (delayMs > 0)
            {
                await Task.Delay(delayMs, cancellationToken);
            }
        }
        catch (OperationCanceledException)
        {
            // Return the color if operation was cancelled
            if (!cancellationToken.IsCancellationRequested)
            {
                await MainThreadDispatcher.EnqueueAsync(() =>
                {
                    tile.PopTopColor();
                    tile.UpdateVisual();
                });
            }
            throw;
        }
    }
}
public static class GridBuilder
{
    public static List<Tile> GenerateGrid(Transform parent, GameObject tilePrefab, float spacing, int rows, int columns)
    {
        List<Tile> tiles = new List<Tile>();

        #region nullChecks

        if (parent == null)
        {
            Debug.LogError("GridBuilder: Parent transform is null!");
            return tiles;
        }

        if (tilePrefab == null)
        {
            Debug.LogError("GridBuilder: Tile prefab is null!");
            return tiles;
        }

        if (columns <= 0 || rows <= 0)
        {
            Debug.LogWarning($"GridBuilder: Invalid grid size ({columns}x{rows})");
            return tiles;
        }


        #endregion



        float gridWidth = (columns - 1) * spacing;
        float gridHeight = (rows - 1) * spacing;
        Vector2 offset = new Vector2(gridWidth, gridHeight) / 2f;

        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                Vector2 position = new Vector2(x * spacing, y * spacing) - offset;

                GameObject tileGO = UnityEngine.Object.Instantiate(tilePrefab, position, Quaternion.identity, parent);
                if (tileGO == null)
                {
                    Debug.LogError($"GridBuilder: Failed to instantiate tile at ({x},{y})");
                    continue;
                }

                tileGO.name = $"Tile {x},{y}";
                var tile = tileGO.GetComponent<Tile>();

                if (tile == null)
                {
                    Debug.LogError($"GridBuilder: Instantiated object at ({x},{y}) does not have a Tile component.");
                    UnityEngine.Object.Destroy(tileGO);
                    continue;
                }

                tile.SetCoordinates(x, y);
                tiles.Add(tile);
            }
        }

        Debug.Log($"GridBuilder: Generated {tiles.Count} tiles.");
        return tiles;
    }

}

#endregion

#region  seed-System
public static class SeedGenerator
{
    public static List<ColorVector> GenerateAllTileColors(int colorCount)
    {
        var colors = new List<ColorVector>();
        for (int i = 0; i < colorCount; i++)
        {
            colors.Add(ColorPalette.GetRandomColor());
        }
        colors.Shuffle();
        return colors;
    }

    public static string GenerateSeed(int colorCount)
    {
        var colors = GenerateAllTileColors(colorCount);

        return SeedEncoder.EncodeColorsToSeed(colors);
    }
}
public static class SeedEncoder
{
    public static string EncodeColorsToSeed(List<ColorVector> colorVectors)
    {
        var builder = new System.Text.StringBuilder();
        foreach (var c in colorVectors)
        {
            builder.Append(ColorPalette.GetColorIndex(c));
        }
        return builder.ToString();
    }


    public static Stack<ColorVector> DecodeSeedToColors(string seed)
    {
        var colorVectors = new Stack<ColorVector>();
        foreach (char ch in seed)
        {
            int index = int.Parse(ch.ToString());
            ColorVector vector = ColorPalette.GetColorVector(index);
            colorVectors.Push(vector);
        }
        return colorVectors;
    }

}

#endregion


#region Joker-System

[Serializable]
public class JokerButtonData
{
    public Button EarnButton;
    public RewardType RewardType;
    [Tooltip("Optional minimum duration for visual feedback")]
    public float MinProcessingTime = 0.5f;
}

public struct Cost
{
    public CurrencyType currencyType;
    public int cost;
    public Cost(CurrencyType type, int amount)
    {
        currencyType = type;
        cost = amount;

    }

}
public struct Reward
{
    public RewardType RewardType;
    public int RewardAmount;
    public Reward(RewardType rewardType, int rewardAmount)
    {
        RewardType = rewardType;
        RewardAmount = rewardAmount;

    }

}

public readonly struct JokerData
{
    public readonly Cost[] Cost;
    public readonly Reward[] Reward;
    public JokerData(Cost[] cost, Reward[] reward)
    {
        Cost = cost;
        Reward = reward;
    }
}

public static class ExtraJokerDataConfigs
{
    public static readonly JokerData ExtraMoves = new JokerData(
        cost: new Cost[] {new Cost(CurrencyType.Coin, 100)},
        reward: new Reward[] {new Reward(RewardType.Moves, 5)});
    public static readonly JokerData ExtraTime = new JokerData(
        cost: new Cost[] {new Cost(CurrencyType.Coin, 100)},
        reward:new Reward[] {new Reward(RewardType.Time, 30)});
    public static readonly JokerData ExtraSlot = new JokerData(
        cost: new Cost[] {new Cost(CurrencyType.Coin, 25)},
        reward:new Reward[] {new Reward(RewardType.Slot, 1)});
    public static JokerData GetJokerData(RewardType type)
    {
        return type switch
        {
            RewardType.Moves => ExtraMoves,
            RewardType.Time => ExtraTime,
            RewardType.Slot => ExtraSlot,
            _ => throw new ArgumentOutOfRangeException(nameof(type), $"No JokerData for {type}")
        };
    }

}


#endregion

#region GameEconomy-System
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

// public static class GameEconomy
// {
//     private static readonly Dictionary<CurrencyType, int> _balances = new();

//     public static int GetAmount(CurrencyType type)
//     {
//         return _balances.TryGetValue(type, out var amount) ? amount : 0;
//     }

//     public static void Add(CurrencyType type, int amount)
//     {
//         if (!_balances.ContainsKey(type))
//             _balances[type] = 0;

//         _balances[type] += amount;
//         Save(type);
//     }

//     public static bool TrySpend(CurrencyType type, int amount)
//     {
//         if (GetAmount(type) < amount)
//             return false;

//         _balances[type] -= amount;
//         Save(type);
//         return true;
//     }

//     private static void Save(CurrencyType type)
//     {
//         PlayerPrefs.SetInt(type.ToString(), _balances[type]);
//         PlayerPrefs.Save();
//     }

// }


#endregion

#region  UIConfigs

[Serializable]
public abstract class PanelConfigBase<TPanelType>
{
    [NonSerialized] public IPanel panel;

    public PanelType category { get; protected set; }  // korumalı set
    public TPanelType panelType;
    public GameObject gameObject;
    public RectTransform rectTransform;
    public CanvasGroup canvasGroup;

    public virtual bool TryInitialize()
    {
        if (gameObject == null)
        {
            Debug.LogError($"{typeof(TPanelType)} config for {panelType} has null GameObject");
            return false;
        }

        panel = gameObject.GetComponent<IPanel>();
        if (panel == null)
        {
            Debug.LogError($"GameObject {gameObject.name} doesn't implement IPanel");
            return false;
        }
        rectTransform = gameObject.GetComponent<RectTransform>();
        if (rectTransform == null)
            Debug.LogWarning($"{gameObject.name} is missing RectTransform.");

        canvasGroup = gameObject.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            Debug.LogWarning($"{gameObject.name} is missing CanvasGroup.");

        return true;
    }

}

[Serializable]
public class ScreenConfig : PanelConfigBase<ScreenType>
{
    public ScreenConfig()
    {
        category = PanelType.Screen;
    }
}


[Serializable]
public class OverlayConfig : PanelConfigBase<OverlayType>
{
    public OverlayConfig()
    {
        category = PanelType.Overlay;
    }
}


#endregion

// #region RuntimeData
// public class GameRuntimeData
// {
//     public int RemainingMoves { get; set; }
//     public int RemainingTime { get; set; }
//     public int CoinsEarned { get; set; }
//     public int LifeEarned { get; set; }

//     public LevelConfig CurrentLevelConfig { get; set; }

//     public GameStatus Status { get; set; }

//     public void Setup(LevelConfig config)
//     {
//         CurrentLevelConfig = config;
//         RemainingTime = config.JokerConfig.TimeLimit;
//         RemainingMoves = config.JokerConfig.MoveCount;
//         CoinsEarned = 0;
//         LifeEarned = 0;
//         Status = GameStatus.None;
//     }
//     public void Reset()
//     {
//         RemainingMoves = 0;
//         RemainingTime = 0;
//         CoinsEarned = 0;
//         LifeEarned = 0;
//         CurrentLevelConfig = null;
//         Status = GameStatus.None;
//     }
// }

// public static class GameContext
// {
//     public static GameRuntimeData Runtime { get; } = new GameRuntimeData();
// }

// #endregion



#region levelConfig
[Serializable]
public class LevelConfig
{
    [SerializeField, Range(1, 99)]
    private int level = 1;
    [SerializeField] private GridConfig gridConfig = new GridConfig();
    [SerializeField] private int colorCount;
    [SerializeField] private LevelTargetConfig levelTargetConfig = new LevelTargetConfig();

    public int Level => level;
    public GridConfig GridConfig => gridConfig;
    public int ColorCount => colorCount;
    public string Seed;
    public LevelTargetConfig LevelTargetConfig => levelTargetConfig;
}

[Serializable]
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

[Serializable]
public struct LevelTargetConfig
{
    public int TimeLimit;
    public int MoveLimit;
    public int TargetWhite;
    public LevelTargetConfig(int time, int move, int white)
    {
        TimeLimit = time;
        MoveLimit = move;
        TargetWhite = white;
    }

}

#endregion

