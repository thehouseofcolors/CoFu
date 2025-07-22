using UnityEngine;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;

#region  Color-System
public  struct ColorVector
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
        // Validate parameters
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
    public static List<Tile> GenerateGrid(Transform parent, Tile tilePrefab, float spacing, int rows, int columns)
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

                GameObject tileGO = UnityEngine.Object.Instantiate(tilePrefab.gameObject, position, Quaternion.identity, parent);
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

                tile.SetCoordinates(x, y);tile.CanSelectable = false;
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
public readonly struct JokerData
{
    public int Min { get; }
    public int Max { get; }
    public int Cost { get; }

    public JokerData(int min, int max, int cost)
    {
        Min = min;
        Max = max;
        Cost = cost;
    }
}

public static class JokerDataConfigs
{
    public static readonly JokerData ExtraMoves = new JokerData(cost: 100, min: 3, max: 10);
    public static readonly JokerData ExtraTime = new JokerData(cost: 100, min: 10, max: 30);
    public static readonly JokerData ExtraSlot = new JokerData(cost: 25, min: 1, max: 5);
}

// public class SlotJoker : IJoker
// {
//     private JokerData _data;
//     public JokerData Data => _data;

//     public SlotJoker(JokerData data)
//     {
//         _data = data;
//     }

//     public bool TryUse()
//     {
//         if (PlayerPrefsService.CurrentCoin >= _data.Cost)
//         {
//             PlayerPrefsService.CurrentCoin -= _data.Cost;
//             return true;
//         }
//         return false;
//     }

//     public bool TryBuy()
//     {
//         if (PlayerPrefsService.CurrentCoin >= _data.Cost)
//         {
//             PlayerPrefsService.CurrentCoin -= _data.Cost;
//             return true;
//         }
//         return false;
//     }

// }

#endregion

#region GameEconomy-System
[System.Serializable]
public struct CurrencyData
{
    public CurrencyType Type;
    public int Amount;

    public CurrencyData(CurrencyType type, int amount)
    {
        Type = type;
        Amount = amount;
    }
}

public static class GameEconomy
{
    private static readonly Dictionary<CurrencyType, int> _balances = new();

    public static event Action<CurrencyType, int> OnCurrencyChanged;



    public static int GetAmount(CurrencyType type)
    {
        return _balances.TryGetValue(type, out var amount) ? amount : 0;
    }

    public static void Add(CurrencyType type, int amount)
    {
        if (!_balances.ContainsKey(type))
            _balances[type] = 0;

        _balances[type] += amount;
        OnCurrencyChanged?.Invoke(type, _balances[type]);
        Save(type);
    }

    public static bool Spend(CurrencyType type, int amount)
    {
        if (GetAmount(type) < amount)
            return false;

        _balances[type] -= amount;
        OnCurrencyChanged?.Invoke(type, _balances[type]);
        Save(type);
        return true;
    }

    private static void Save(CurrencyType type)
    {
        PlayerPrefs.SetInt(type.ToString(), _balances[type]);
        PlayerPrefs.Save();
    }

    private static void Load()
    {
        foreach (CurrencyType type in Enum.GetValues(typeof(CurrencyType)))
        {
            _balances[type] = PlayerPrefs.GetInt(type.ToString(), 0);
        }
    }
}


#endregion


