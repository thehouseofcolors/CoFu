using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using GameEvents;
using System;
using System.Linq;

public class PlayBoardManager : Singleton<PlayBoardManager>, IGameSystem
{
    [Header("Board Settings")]
    [SerializeField] private Tile _tilePrefab;
    [SerializeField] private Transform _gridParent;
    [SerializeField] private float _spacing = 1.1f;

    private List<Tile> _allTiles = new List<Tile>();
    private List<IDisposable> _eventSubscriptions = new List<IDisposable>();

    public void Initialize()
    {
        SubscribeEvents();
    }

    public void Shutdown()
    {
        UnsubscribeEvents();
        ClearGrid();
    }

    private void SubscribeEvents()
    {
        _eventSubscriptions.Add(EventBus.Subscribe<GameLoadEvent>(OnLevelLoad));
        _eventSubscriptions.Add(EventBus.Subscribe<GamePauseEvent>(OnGamePause));
        _eventSubscriptions.Add(EventBus.Subscribe<GameResumeEvent>(OnGameResume));
        _eventSubscriptions.Add(EventBus.Subscribe<TileFuseEvent>(OnTileFuse));
    }

    private void UnsubscribeEvents()
    {
        foreach (var subscription in _eventSubscriptions)
        {
            subscription?.Dispose();
        }
        _eventSubscriptions.Clear();
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
                level.tiles_in_a_column,
                level.tiles_in_a_row
            );

            if (_allTiles == null || _allTiles.Count == 0)
            {
                Debug.LogError("Failed to generate grid tiles");
                return;
            }

            // Decode and apply colors
            var colors = SeedEncoder.DecodeSeedToColors(level.seed);

            // Paint tiles with async loading effect
            await PaintManager.PaintTiles(_allTiles, colors);

            // Enable interaction after painting completes
            SetTilesSelectable(true);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Grid setup failed: {ex.Message}");
            ClearGrid(); // Clean up if something went wrong
            throw; // Re-throw to maintain error flow
        }
    }

    private async Task OnLevelLoad(GameLoadEvent e)
    {
        await SetupGrid(e.Level);

        await EventBus.PublishAuto(new GameStartEvent());
    }

    private async Task OnTileFuse(TileFuseEvent e)
    {
        try
        {
            // Check if all tiles are empty
            bool allTilesEmpty = true;
            foreach (var tile in _allTiles)
            {
                if (!tile.IsEmpty())
                {
                    allTilesEmpty = false;
                    break; // Exit early if we find a non-empty tile
                }
            }

            if (allTilesEmpty)
            {
                // Add a small delay for better game feel
                await Task.Delay(100);

                // Publish win event with error handling
                try
                {
                    // await EventBus.PublishAuto(new GameWinEvent());
                    await GameStateMachine.ChangeStateAsync(new GameWinState());
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Failed to publish GameWinEvent: {ex}");
                    // Consider adding fallback win handling here
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error in OnTileFuse: {ex}");
        }
    }



    private async Task OnGamePause(GamePauseEvent e)
    {
        SetTilesSelectable(false);
        await Task.CompletedTask;
    }

    private async Task OnGameResume(GameResumeEvent e)
    {
        SetTilesSelectable(true);
        await Task.CompletedTask;
    }

    private void SetTilesSelectable(bool selectable)
    {
        foreach (var tile in _allTiles)
        {
            tile.CanSelectable = selectable;
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
