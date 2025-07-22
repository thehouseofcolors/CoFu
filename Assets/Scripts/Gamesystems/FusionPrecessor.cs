
using System.Threading.Tasks;
using UnityEngine;
using GameEvents;
using System.Collections.Generic;
using System;

public class FusionProcessor : MonoBehaviour, IGameSystem
{
    [Header("Dependencies")]
    [SerializeField] private IntermediateSlotManager _slotManager;
    [SerializeField] private GameObject _colorEffectPrefab;
    [SerializeField] private Transform _whiteScoreTarget;
    private Transform _mergeCenterPoint;
    

    private readonly List<IDisposable> _disposables = new List<IDisposable>();
    private bool _isProcessingFusion;

    public Task Initialize()
    {
        SubscribeEvents();
        return Task.CompletedTask;
    }

    public Task Shutdown()
    {
        UnsubscribeEvents();
        return Task.CompletedTask;
    }

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
        if (_isProcessingFusion || !IsValidFusion(e))
            return;

        _isProcessingFusion = true;

        try
        {
            await ProcessFusion(e.Source, e.Target);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Fusion failed: {ex}");
        }
        finally
        {
            _isProcessingFusion = false;
        }
    }

    private bool IsValidFusion(TileFuseEvent e)
    {
        return e.Source != null &&
               e.Target != null &&
               !e.Source.IsEmpty() &&
               !e.Target.IsEmpty();
    }

    private async Task ProcessFusion(IColorSource source, IColorSource target)
    {
        // Get colors and fusion result
        var sourceColor = source.PeekColor();
        var targetColor = target.PeekColor();
        var fusionResult = ColorFusion.Fuse(sourceColor, targetColor);

        if (!fusionResult.IsValidColor)
        {
            await HandleInvalidFusion();
            return;
        }

        // Execute fusion flow
        source.SetTemporarilyDisabled(true);
        target.SetTemporarilyDisabled(true);

        try
        {
            await AnimateColorMerge(source, target);
            await HandleFusionResult(source, target, fusionResult);
            
        }
        finally
        {
            source.SetTemporarilyDisabled(false);
            target.SetTemporarilyDisabled(false);
        }
    }

    private async Task AnimateColorMerge(IColorSource source, IColorSource target)
    {
        var moveSourceTask = CreateAndMoveEffect(
            source.PeekColor(),
            source.GetWorldPosition(),
            _mergeCenterPoint.position
        );

        var moveTargetTask = CreateAndMoveEffect(
            target.PeekColor(),
            target.GetWorldPosition(),
            _mergeCenterPoint.position
        );

        await Task.WhenAll(moveSourceTask, moveTargetTask);
    }

    private async Task HandleFusionResult(IColorSource source, IColorSource target, ColorVector resultColor)
    {
        var mergeEffect = await CreateMergeEffect(resultColor);

        if (resultColor.IsWhite)
        {
            await HandleWhiteResult(mergeEffect);
            RemoveColors(source, target);
        }
        else
        {
            var success = await HandleColorResult(mergeEffect, resultColor);
            if (success) RemoveColors(source, target);
        }
    }

    private async Task<GameObject> CreateMergeEffect(ColorVector color)
    {
        return await Effects.Gameplay.PlayMergeEffectAsync(new EffectParams
        {
            Prefab = _colorEffectPrefab,
            Color = color,
            StartPos = _mergeCenterPoint.position,
            EndPos = _mergeCenterPoint.position
        });
    }

    private async Task HandleWhiteResult(GameObject effect)
    {
        await Effects.Gameplay.PlayMoveEffectAsync(effect, _whiteScoreTarget.position);
        

    }

    private async Task<bool> HandleColorResult(GameObject effect, ColorVector color)
    {
        if (!_slotManager.HasEmptySlot())
        {
            await Effects.Gameplay.PlayMergeFailEffectAsync(effect);
            return false;
        }

        var slot = _slotManager.GetSlot();
        await Effects.Gameplay.PlayMoveEffectAsync(effect, slot.GetWorldPosition());
        slot.SetColor(color);
        return true;
    }

    private void RemoveColors(IColorSource source, IColorSource target)
    {
        source.PopTopColor();
        target.PopTopColor();
    }

    private async Task UpdateGameState()
    {
        PlayerPrefsService.RemainingMoves--;
        await EventBus.PublishAuto(new UpdateMoveCountUIEvent(PlayerPrefsService.RemainingMoves));
    }

    private async Task HandleInvalidFusion()
    {
        Effects.Camera.Shake();
        await Task.Yield(); // Allow one frame for shake to register
    }

    private Task CreateAndMoveEffect(ColorVector color, Vector3 startPos, Vector3 endPos)
    {
        return Effects.Gameplay.PlayMoveEffectAsync(new EffectParams
        {
            Prefab = _colorEffectPrefab,
            Color = color,
            StartPos = startPos,
            EndPos = endPos
        });
    }
}

