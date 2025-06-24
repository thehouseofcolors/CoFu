// using System.Collections;
// using UnityEngine;
// using GameEvents;
// using System.Threading.Tasks;
// using DG.Tweening;
// using System;
// using System.Collections.Generic;

// // public class TileSelectionHandler : MonoBehaviour, IGameSystem
// // {
// //     private Tile firstTile;
// //     private Tile secondTile;
// //     private bool isProcessing = false;
// //     private SelectionState currentState = SelectionState.None;

// //     private enum SelectionState
// //     {
// //         None,
// //         FirstSelected,
// //         SecondSelected,
// //         Processing
// //     }

// //     public void Initialize()
// //     {
// //         EventBus.Subscribe<TileSelectionEvent>(OnSelectTile);
// //     }

// //     public void Shutdown()
// //     {
// //         ClearSelection();
// //     }

// //     public async Task OnSelectTile(TileSelectionEvent e)
// //     {
// //         if (currentState == SelectionState.Processing)
// //         {
// //             Debug.LogWarning("[SelectionManager] Selection blocked - processing in progress");
// //             return;
// //         }

// //         if (e.Tile == null)
// //         {
// //             Debug.LogWarning("[SelectionManager] Received null tile selection");
// //             return;
// //         }

// //         // Prevent selecting the same tile twice
// //         if (e.Tile == firstTile || e.Tile == secondTile)
// //         {
// //             Debug.Log($"[SelectionManager] Tile {e.Tile.name} already selected - clearing selection");
// //             ClearSelection();
// //             return;
// //         }

// //         switch (currentState)
// //         {
// //             case SelectionState.None:
// //                 firstTile = e.Tile;
// //                 TileEffectController.SetHighlight(firstTile, true);
// //                 currentState = SelectionState.FirstSelected;
// //                 break;

// //             case SelectionState.FirstSelected:
// //                 secondTile = e.Tile;
// //                 TileEffectController.SetHighlight(secondTile, true);
// //                 currentState = SelectionState.SecondSelected;
// //                 await ProcessSelection();
// //                 break;
// //         }
// //     }

// //     private async Task ProcessSelection()
// //     {
// //         currentState = SelectionState.Processing;

// //         try
// //         {
// //             await EventBus.PublishAuto(new TileFuseEvent(firstTile, secondTile));
// //         }
// //         finally
// //         {
// //             ClearSelection();
// //         }
// //     }

// //     private void ClearSelection()
// //     {
// //         if (firstTile != null)
// //         {
// //                 TileEffectController.SetHighlight(firstTile, false);
// //             firstTile = null;
// //         }

// //         if (secondTile != null)
// //         {
// //                 TileEffectController.SetHighlight(secondTile, false);
// //             secondTile = null;
// //         }

// //         currentState = SelectionState.None;
// //         Debug.Log("[SelectionManager] Selection cleared");
// //     }
// // }

// public class TileSelectionHandler : MonoBehaviour, IGameSystem
// {
//     private Tile firstTile;
//     private Tile secondTile;
//     private SelectionState currentState = SelectionState.None;

//     private enum SelectionState { None, FirstSelected, Processing }

//     List<IDisposable> disposables = new List<IDisposable>();

//     public void Initialize()
//     {
//         SubscribeEvents();
//     }

//     public void Shutdown()
//     {
//         UnsubscribeEvents();
//     }
//     private void SubscribeEvents()
//     {
//         disposables.Add(EventBus.Subscribe<TileSelectionEvent>(HandleSelectTile));
//     }

//     private void UnsubscribeEvents()
//     {
//         foreach (var d in disposables)
//         {
//             d?.Dispose();
//         }
//     }

//     public async Task HandleSelectTile(TileSelectionEvent e)
//     {
//         if (currentState == SelectionState.Processing || e.Tile == null)
//             return;

//         if (e.Tile == firstTile)
//         {
//             ClearSelection();
//             return;
//         }

//         switch (currentState)
//         {
//             case SelectionState.None:
//                 firstTile = e.Tile;
//                 firstTile.SetHighlight(true);
//                 currentState = SelectionState.FirstSelected;
//                 break;

//             case SelectionState.FirstSelected:
//                 secondTile = e.Tile;
//                 secondTile.SetHighlight(true);
//                 currentState = SelectionState.Processing;
//                 await ProcessSelection();
//                 break;
//         }
//     }

//     private async Task ProcessSelection()
//     {
//         try
//         {
//             await EventBus.PublishAuto(new TileFuseEvent(firstTile, secondTile));
//         }
//         finally
//         {
//             ClearSelection();
//         }
//     }

//     private void ClearSelection()
//     {
//         if (firstTile != null)
//         {
//             firstTile.SetHighlight(false);
//             firstTile = null;
//         }

//         if (secondTile != null)
//         {
//             secondTile.SetHighlight( false);
//             secondTile = null;
//         }

//         currentState = SelectionState.None;
//     }
// }


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

    private async Task ProcessTileSelection(Tile selectedTile)
    {
        _isInCooldown = true;
        
        try
        {
            switch (_currentState)
            {
                case SelectionState.None:
                    HandleFirstSelection(selectedTile);
                    break;
                    
                case SelectionState.FirstSelected:
                    await HandleSecondSelection(selectedTile);
                    break;
            }
        }
        finally
        {
            await Task.Delay(TimeSpan.FromSeconds(selectionCooldown));
            _isInCooldown = false;
        }
    }

    private void HandleFirstSelection(Tile tile)
    {
        _firstTile = tile;
        _firstTile.SetHighlight(true);
        _currentState = SelectionState.FirstSelected;
    }

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
}