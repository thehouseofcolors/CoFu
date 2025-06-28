
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
    private IColorSource _firstTile;
    private IColorSource _secondTile;
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

        await ProcessTileSelection(e.colorSource);
    }

    private bool ShouldIgnoreSelection(TileSelectionEvent e)
    {
        return _isInCooldown ||
               e.colorSource == null ||
               _currentState == SelectionState.Processing;
    }


    private async Task HandleSecondSelection(IColorSource tile)
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

    private void ResetTile(ref IColorSource tile)
    {
        if (tile != null)
        {
            tile.SetHighlight(false);
            tile = null;
        }
    }
    
    private async Task ProcessTileSelection(IColorSource selectedTile)
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

    private async Task HandleFirstSelection(IColorSource tile)
    {
        _firstTile = tile;
        _firstTile.SetHighlight(true);
        _currentState = SelectionState.FirstSelected;
        await Task.CompletedTask;
    }
}

