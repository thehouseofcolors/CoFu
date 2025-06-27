
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameEvents;
using UnityEngine;

public class TileSelectionHandler : MonoBehaviour, IGameSystem
{
    // Serialized Fields
    [SerializeField] private float selectionCooldown = 0.3f;

    // Private State
    private Tile _firstTile;
    private Tile _secondTile;
    private SelectionState _currentState = SelectionState.None;
    private bool _isInCooldown;
    private readonly List<IDisposable> _disposables = new List<IDisposable>();

    private enum SelectionState
    {
        None,
        FirstSelected,
        Processing
    }

    public void Initialize()
    {
        SubscribeEvents();
    }

    public void Shutdown()
    {
        UnsubscribeEvents();
        ClearSelection();
    }

    private void SubscribeEvents()
    {
        _disposables.Add(EventBus.Subscribe<TileSelectionEvent>(HandleSelectTile));
    }

    private void UnsubscribeEvents()
    {
        foreach (var disposable in _disposables)
        {
            disposable?.Dispose();
        }
        _disposables.Clear();
    }

    private async void HandleSelectTile(TileSelectionEvent e)
    {
        if (ShouldIgnoreSelection(e))
            return;

        await ProcessTileSelection(e.Tile);
    }

    private bool ShouldIgnoreSelection(TileSelectionEvent e)
    {
        return _isInCooldown ||
               e.Tile == null ||
               _currentState == SelectionState.Processing;
    }

    // private async Task ProcessTileSelection(Tile selectedTile)
    // {
    //     _isInCooldown = true;

    //     try
    //     {
    //         switch (_currentState)
    //         {
    //             case SelectionState.None:
    //                 HandleFirstSelection(selectedTile);
    //                 break;

    //             case SelectionState.FirstSelected:
    //                 await HandleSecondSelection(selectedTile);
    //                 break;
    //         }
    //     }
    //     finally
    //     {
    //         await Task.Delay(TimeSpan.FromSeconds(selectionCooldown));
    //         _isInCooldown = false;
    //     }
    // }

    // private void HandleFirstSelection(Tile tile)
    // {
    //     _firstTile = tile;
    //     _firstTile.SetHighlight(true);
    //     _currentState = SelectionState.FirstSelected;
    // }

    private async Task HandleSecondSelection(Tile tile)
    {
        _secondTile = tile;
        _secondTile.SetHighlight(true);
        _currentState = SelectionState.Processing;

        await ProcessSelection();
    }

    private async Task ProcessSelection()
    {
        try
        {
            await EventBus.PublishAuto(new TileFuseEvent(_firstTile, _secondTile));
        }
        finally
        {
            ClearSelection();
        }
    }

    private void ClearSelection()
    {
        ResetTile(ref _firstTile);
        ResetTile(ref _secondTile);
        _currentState = SelectionState.None;
    }

    private void ResetTile(ref Tile tile)
    {
        if (tile != null)
        {
            tile.SetHighlight(false);
            tile = null;
        }
    }
    
    private async Task ProcessTileSelection(Tile selectedTile)
    {
        if (_isInCooldown) return;
        _isInCooldown = true;
        
        try
        {
            switch (_currentState)
            {
                case SelectionState.None:
                    await HandleFirstSelection(selectedTile);
                    break;

                case SelectionState.FirstSelected:
                    await HandleSecondSelection(selectedTile);
                    break;
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Selection failed: {e}");
            ClearSelection();
        }
        finally
        {
            await Task.Delay(TimeSpan.FromSeconds(selectionCooldown));
            _isInCooldown = false;
        }
    }

    private async Task HandleFirstSelection(Tile tile)
    {
        _firstTile = tile;
        _firstTile.SetHighlight(true);
        _currentState = SelectionState.FirstSelected;
        await Task.CompletedTask;
    }
}

