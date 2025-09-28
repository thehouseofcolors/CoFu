using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using GameEvents;
using System;

public class PlayBoardManager : MonoBehaviour, IGameSystem
{
    [Header("Board Settings")]
    [SerializeField] private GameObject _tilePrefab;
    [SerializeField] private Transform _gridParent;
    [SerializeField] private float _spacing = 1.1f;

    private List<Tile> _allTiles = new List<Tile>();
    private List<IDisposable> _eventSubscriptions = new List<IDisposable>();

    public async Task Initialize()
    {
        SubscribeEvents();
        await Task.CompletedTask;
    }

    public async Task Shutdown()
    {
        UnsubscribeEvents();
        ClearGrid();
        await Task.CompletedTask;
    }

    private void SubscribeEvents()
    {
        _eventSubscriptions.Add(EventBus.Subscribe<GridAnimateEvent>(HandleGridAnimateEvent));
        _eventSubscriptions.Add(EventBus.Subscribe<GridDestroyEvent>(HandleGridDestroyEvent));
    }

    private void UnsubscribeEvents()
    {
        foreach (var subscription in _eventSubscriptions)
        {
            subscription?.Dispose();
        }
        _eventSubscriptions.Clear();
    }


    private async Task HandleGridAnimateEvent(GridAnimateEvent e)
    {
        await SetupGrid(LevelManager.Instance.CurrentLevelConfig);
        await Task.CompletedTask;
    }
    private async Task HandleGridDestroyEvent(GridDestroyEvent e)
    {
        ClearGrid();
        await Task.CompletedTask;
    }
    public async Task SetupGrid(LevelConfig level)
    {
        try
        {
            // Clear existing grid
            ClearGrid();

            // Validate input parameters
            if (level == null)
            {
                Debug.LogError("LevelConfig is null");
                return;
            }

            if (_tilePrefab == null || _gridParent == null)
            {
                Debug.LogError("Required prefab or parent transform not assigned");
                return;
            }

            // Generate grid tiles
            _allTiles = GridBuilder.GenerateGrid(
                _gridParent,
                _tilePrefab,
                _spacing,
                level.GridConfig.Columns,
                level.GridConfig.Rows
            );

            if (_allTiles == null || _allTiles.Count == 0)
            {
                Debug.LogError("Failed to generate grid tiles");
                return;
            }

            // Decode and apply colors
            var colors = SeedEncoder.DecodeSeedToColors(level.Seed);

            // Paint tiles with async loading effect
            await PaintManager.PaintTiles(_allTiles, colors);

            await EventBus.PublishAuto(new GamePlayActivityChangedEvent(true));
        }
        catch (Exception ex)
        {
            Debug.LogError($"Grid setup failed: {ex.Message}");
            ClearGrid(); // Clean up if something went wrong
            throw; // Re-throw to maintain error flow
        }
    }

    private void ClearGrid()
    {
        // try
        // {
        //     // Destroy all tiles
        //     foreach (var tile in _allTiles)
        //     {
        //         if (tile != null && tile.gameObject != null)
        //         {
        //             Destroy(tile.gameObject);
        //         }
        //     }
        //     _allTiles.Clear();

        //     // Additional cleanup of any leftover objects
        //     foreach (Transform child in _gridParent)
        //     {
        //         Destroy(child.gameObject);
        //     }
        // }
        // catch (Exception ex)
        // {
        //     Debug.LogError($"Cleargrid failed: {ex.Message}");
        //     throw; // Re-throw to maintain error flow
        // }

    }
}

