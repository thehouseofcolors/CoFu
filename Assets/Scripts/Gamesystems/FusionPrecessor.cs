

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
    [SerializeField] private EffectManager effectManager;
    [SerializeField] private CameraShaker cameraShaker;

    [SerializeField] public RectTransform whiteTarget;

    [SerializeField] public RectTransform intermediateTarget;
    
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

    // private async Task ExecuteFusion(IColorSource source, IColorSource target, ColorVector resultColor)
    // {
    //     // Calculate merge position between the two tiles
    //     var mergePosition = (source.GetPosition() + target.GetPosition()) * 0.5f;
    //     ColorVector sourceColor = source.GetColor();
    //     ColorVector targetColor = target.GetColor();
    //     // Animate both colors moving to merge position
    //     await AnimateColorMerge(sourceColor, targetColor, source.GetPosition(), target.GetPosition());


    //     // Handle the fusion result
    //     if (resultColor.IsWhite)
    //     {
    //         await effectManager.MoveEffectToUI(resultColor, whiteTarget.anchoredPosition);
    //     }
    //     else
    //     {
    //         Vector2 targetPos;
    //         Vector2? slotPos = IntermediateSlotManager.Instance.GetSlotPosition();
    //         if (slotPos.HasValue)
    //         {
    //             targetPos = slotPos.Value;
                
    //             await effectManager.MoveEffectToUI(resultColor, targetPos);
    //             // Örneğin: efekti oraya taşı
    //             Debug.Log($"Slot pozisyonu bulundu: {targetPos}");
    //         }
    //         else
    //         {
    //             Debug.Log("Boş slot yok!");
    //         }

    //     }
        
    //     // Update game state
    //     PlayerPrefsService.RemainingMoves--;
    //     await EventBus.PublishAuto(new UpdateMoveCountUIEvent(PlayerPrefsService.RemainingMoves));
    // }

    private async Task ExecuteFusion(IColorSource source, IColorSource target, ColorVector resultColor)
    {
        var sourcePos = source.GetPosition();
        var targetPos = target.GetPosition();
        var mergePosition = (sourcePos + targetPos) * 0.5f;

        ColorVector sourceColor = source.GetColor();
        ColorVector targetColor = target.GetColor();

        // 1. Animasyonu oyna
        await AnimateColorMerge(sourceColor, targetColor, sourcePos, targetPos);

        // 2. Beyazsa beyaz hedefe gönder
        if (resultColor.IsWhite)
        {

            await effectManager.MoveEffectToUI(resultColor, whiteTarget.anchoredPosition);
        }
        else
        {
            SlotController slot = IntermediateSlotManager.Instance.GetSlot();
            if (slot != null)
            {
                Vector2 slotPos = slot.GetComponent<RectTransform>().position;
                await effectManager.MoveEffectToUI(resultColor, slotPos);
                slot.SetColor(resultColor);
            }
            else
            {
                await AnimateColorReturn(sourceColor, targetColor, sourcePos, targetPos);

                //     // Renkleri stack'e geri pushla
                //     // source.PushColor(sourceColor);
                //     // target.PushColor(targetColor);

                return; // ⛔ Fusion işlemi tamamlanmaz

            }
            // Vector2? slotPos = IntermediateSlotManager.Instance.GetSlotPosition();
            // if (slotPos.HasValue)
            // {
            //     var targetSlotPos = slotPos.Value;

                //     await effectManager.MoveEffectToUI(resultColor, targetSlotPos);
                //     Debug.Log($"Slot pozisyonu bulundu: {targetSlotPos}");
                // }
                // else
                // {
                //     Debug.Log("Boş slot yok! Geriye alma başlatılıyor...");

                //     // 🔁 Ters efekt: renkleri geri gönder
                //     await AnimateColorReturn(sourceColor, targetColor, sourcePos, targetPos);

                //     // Renkleri stack'e geri pushla
                //     // source.PushColor(sourceColor);
                //     // target.PushColor(targetColor);

                //     return; // ⛔ Fusion işlemi tamamlanmaz
                // }
        }

        // 3. State update
        PlayerPrefsService.RemainingMoves--;
        await EventBus.PublishAuto(new UpdateMoveCountUIEvent(PlayerPrefsService.RemainingMoves));
    }
    private async Task AnimateColorReturn(ColorVector sourceColor, ColorVector targetColor, Vector3 sourcePos, Vector3 targetPos)
    {
       
        var moveSourceTask = effectManager.ReverseSpawnAndMoveColorEffect(sourceColor, sourcePos);
        var moveTargetTask = effectManager.ReverseSpawnAndMoveColorEffect(targetColor, targetPos);
        await Task.WhenAll(moveSourceTask, moveTargetTask);


    }

    private async Task AnimateColorMerge(ColorVector sourceColor, ColorVector targetColor, Vector3 sourcePos, Vector3 targetPos)
    {
        var moveSourceTask = effectManager.SpawnAndMoveColorEffect(sourceColor, sourcePos);
        var moveTargetTask = effectManager.SpawnAndMoveColorEffect(targetColor, targetPos);
        await Task.WhenAll(moveSourceTask, moveTargetTask);
    }


    private async Task HandleInvalidFusion()
    {
        cameraShaker.ShakeCamera();
        await Task.Yield(); // Allow one frame for shake to register
    }
}
