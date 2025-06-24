

using System.Threading.Tasks;
using UnityEngine;
using GameEvents;
using NUnit.Framework.Interfaces;
using System.Collections.Generic;
using System;


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
    [SerializeField] private EffectManager effectManager;
    [SerializeField] private CameraShaker cameraShaker;
    
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

    private async Task ProcessFusion(Tile source, Tile target)
    {
        // Validate colors
        if (source.IsEmpty || target.IsEmpty)
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

    private async Task ExecuteFusion(Tile source, Tile target, ColorVector resultColor)
    {
        // Calculate merge position between the two tiles
        var mergePosition = (source.transform.position + target.transform.position) * 0.5f;
        ColorVector sourceColor = source.PopTopColor();
        ColorVector targetColor = target.PopTopColor();
        // Animate both colors moving to merge position
        await AnimateColorMerge(sourceColor, targetColor, source.transform.position, target.transform.position);


        // Handle the fusion result
        if (resultColor.IsWhite)
        {
            await HandleWhiteFusion(effectManager.scoreTarget.position, mergePosition);
        }
        else
        {
            await HandleRegularFusion(target, resultColor, mergePosition);
        }
        
        // Update game state
        PlayerPrefsService.RemainingMoves--;
        await EventBus.PublishAuto(new UpdateMoveCountUIEvent(PlayerPrefsService.RemainingMoves));
    }

    private async Task AnimateColorMerge(ColorVector sourceColor, ColorVector targetColor, Vector3 sourcePos, Vector3 targetPos)
    {
        var moveSourceTask = effectManager.SpawnAndMoveColorEffect(sourceColor, sourcePos);
        var moveTargetTask = effectManager.SpawnAndMoveColorEffect(targetColor, targetPos);
        await Task.WhenAll(moveSourceTask, moveTargetTask);
    }

    private async Task HandleRegularFusion(Tile target, ColorVector color, Vector3 mergePosition)
    {
        await effectManager.MoveEffectToTile(color, target.transform.position);
        target.PushColor(color);
    }

    private async Task HandleWhiteFusion(Vector3 targetpos, Vector3 mergePosition)
    {
        await Task.Delay(TimeSpan.FromSeconds(whiteScoreDelay));
        await effectManager.MoveWhiteEffectToUI( targetpos);
    }

    private async Task HandleInvalidFusion()
    {
        cameraShaker.ShakeCamera();
        await Task.Yield(); // Allow one frame for shake to register
    }
}
