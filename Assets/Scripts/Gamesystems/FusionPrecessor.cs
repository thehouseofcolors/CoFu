

using System.Threading.Tasks;
using UnityEngine;
using GameEvents;
using NUnit.Framework.Interfaces;
using System.Collections.Generic;
using System;
using DG.Tweening;


// public class FusionProcessor : MonoBehaviour, IGameSystem
// {
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
//         disposables.Add(EventBus.Subscribe<TileFuseEvent>(HandleTileFuse));
//     }

//     private void UnsubscribeEvents()
//     {
//         foreach (var d in disposables)
//         {
//             d?.Dispose();
//         }
//     }


//     async Task HandleTileFuse(TileFuseEvent e)
//     {
//         var result = ColorFusion.Fuse(e.Source.PeekColor(), e.Target.PeekColor());
//         if (!result.IsValidColor)
//         {
//             CameraShaker.Instance.ShakeCamera();
//             return;
//         }

//         await ApproveMerge(e);
//         if (result.IsWhite)
//         {
//             e.Target.PopTopColor();
//         }

//     }


//     private ColorVector ProcessColorMerge(Tile source, Tile target)
//     {
//         return ColorFusion.Fuse(source.PeekColor(), target.PeekColor());
//     }


//     private async Task ApproveMerge(TileFuseEvent e)
//     {
//         PlayerPrefsService.RemainingMoves -= 1;
//         // await e.Source.TransferColorTo(e.Target);
//         await EventBus.PublishAuto(new UpdateMoveCountUIEvent(PlayerPrefsService.RemainingMoves));
//         await Task.CompletedTask;
//     }

//     // Öncelikle EffectManager referansını al
//     [SerializeField] private EffectManager effectManager;

//     async Task HandleColorMerge(Tile firstTile, Tile secondTile, Tile targetTile)
//     {
//         var color1 = firstTile.PopTopColor();
//         var color2 = secondTile.PopTopColor();

//         Vector3 mergePoint = Vector3.zero;

//         // İki renk efektini tile pozisyonlarından mergePoint'e gönder
//         await Task.WhenAll(
//             effectManager.SpawnAndMoveColorEffect(color1, firstTile.transform.position, mergePoint),
//             effectManager.SpawnAndMoveColorEffect(color2, secondTile.transform.position, mergePoint)
//         );

//         // Burada birleşme efektini oynat
//         var fusedColor = ColorFusion.Fuse(color1, color2);
//         await effectManager.PlayMergeEffect(fusedColor, mergePoint);

//         // Sonuca göre efekt hedefe hareket eder
//         if (fusedColor.IsWhite)
//             await effectManager.MoveEffectToUI(fusedColor, effectManager.scoreTarget.position);
//         else
//             await effectManager.MoveEffectToTile(fusedColor, targetTile.transform.position);

//         // Burada tile verilerini güncelle
//         targetTile.PushColor(fusedColor);
//     }


// }

public class FusionProcessor : MonoBehaviour, IGameSystem
{
    [Header("Dependencies")]
    // [SerializeField] private EffectManager effectManager;
    [SerializeField] private CameraShaker cameraShaker;
    [SerializeField] private GameObject prefab;
    [SerializeField] private Transform whiteTarget;

    [SerializeField] private Transform slotTarget;


    [Header("Settings")]
    [SerializeField] private float fusionDuration = 0.5f;
    [SerializeField] private float whiteScoreDelay = 0.3f;

    private readonly List<IDisposable> _disposables = new List<IDisposable>();
    private bool _isProcessingFusion;

    public void Initialize() => SubscribeEvents();
    public void Shutdown() => UnsubscribeEvents();

    private void SubscribeEvents()
    {
        _disposables.Add(EventBus.Subscribe<TileFuseEvent>(HandleTileFuse));
    }

    private void UnsubscribeEvents()
    {
        foreach (var disposable in _disposables)
        {
            disposable?.Dispose();
        }
        _disposables.Clear();
    }

    private async void HandleTileFuse(TileFuseEvent e)
    {
        if (_isProcessingFusion || e.Source == null || e.Target == null)
            return;

        _isProcessingFusion = true;

        try
        {
            await ProcessFusion(e.Source, e.Target);
        }
        finally
        {
            _isProcessingFusion = false;
        }
    }

    private async Task ProcessFusion(IColorSource source, IColorSource target)
    {
        // Validate colors
        if (source.IsEmpty() || target.IsEmpty())
        {
            await HandleInvalidFusion();
            return;
        }

        var sourceColor = source.PeekColor();
        var targetColor = target.PeekColor();
        var fusionResult = ColorFusion.Fuse(sourceColor, targetColor);

        if (!fusionResult.IsValidColor)
        {
            await HandleInvalidFusion();
            return;
        }

        await ExecuteFusion(source, target, fusionResult);
    }

    private async Task ExecuteFusion(IColorSource source, IColorSource target, ColorVector resultColor)
    {
        source.SetTemporarilyDisabled(true);
        target.SetTemporarilyDisabled(true);
        await AnimateColorMerge(source, target);

        GameObject effectGO = await EffectManager.PlayMergeEffectAsync(new EffectParams
        {
            Prefab=prefab,
            Color = resultColor,
            StartPos = Vector3.zero,
            EndPos = Vector3.zero,
        });

        bool isSucced = await HandleMergedFusion(effectGO, resultColor);
        if (isSucced)
        {
            source.PopTopColor();
            target.PopTopColor();
        }
        source.SetTemporarilyDisabled(false);
        target.SetTemporarilyDisabled(false);
        // 3. State update
        PlayerPrefsService.RemainingMoves--;
        await EventBus.PublishAuto(new UpdateMoveCountUIEvent(PlayerPrefsService.RemainingMoves));
    }
    private async Task<bool> HandleMergedFusion(GameObject effectGO, ColorVector resultColor)
    {
        if (resultColor.IsWhite)
        {
            await EffectManager.PlayMoveEffectAsync(effectGO, whiteTarget.position);
            return true;
        }
        else
        {
            if (IntermediateSlotManager.Instance.HasEmptySlot())
            {
                SlotController slot = IntermediateSlotManager.Instance.GetSlot();
                await EffectManager.PlayMoveEffectAsync(effectGO, slot.GetPosition());
                slot.SetColor(resultColor);
                return true;

            }
            else
            {
                await HandleInvalidFusion();
                await EffectManager.PlayMergeFailEffectAsync(effectGO);
                return false;
            }
        }
    }
    private async Task AnimateColorMerge(IColorSource source, IColorSource target)
    {
        var task1 = EffectManager.PlayMoveEffectAsync(new EffectParams
        {
            Prefab = prefab,
            Color = source.PeekColor(),
            StartPos = source.GetPosition(),
            EndPos = Vector3.zero,
        });

        var task2 = EffectManager.PlayMoveEffectAsync(new EffectParams
        {
            Prefab = prefab,
            Color = target.PeekColor(),
            StartPos = target.GetPosition(),
            EndPos = Vector3.zero,
        });

        await Task.WhenAll(task1, task2);
    }
    private async Task HandleInvalidFusion()
    {
        cameraShaker.ShakeCamera();
        await Task.Yield(); // Allow one frame for shake to register
    }
}


// // Recording an action
// var undoAction = new CombineTilesUndoAction(
//     tileA, 
//     tileB,
//     originalColorA,
//     originalColorB,
//     hadResult: true);

// UndoManager.Instance.RecordAction(undoAction);

// // Executing undo (from your joker UI)
// try
// {
//     await undoAction.UndoAsync();
//     // Update UI or handle success
// }
// catch (Exception ex)
// {
//     Debug.LogError($"Failed to undo: {ex.Message}");
//     // Return joker or show error
// }

