

using System.Threading.Tasks;
using UnityEngine;
using GameEvents;
using NUnit.Framework.Interfaces;
using System.Collections.Generic;
using System;


public class FusionProcessor : MonoBehaviour, IGameSystem
{
    List<IDisposable> disposables = new List<IDisposable>();

    public void Initialize()
    {
        SubscribeEvents();
    }

    public void Shutdown()
    {
        UnsubscribeEvents();
    }

    private void SubscribeEvents()
    {
        disposables.Add(EventBus.Subscribe<TileFuseEvent>(OnTileFuse));
    }

    private void UnsubscribeEvents()
    {
        foreach (var d in disposables)
        {
            d?.Dispose();
        }
    }
    async Task OnTileFuse(TileFuseEvent e)
    {
        var result = ColorFusion.Fuse(e.Source.PeekColor(), e.Target.PeekColor());
        if (!result.IsValidColor)
        {
            CameraShaker.Instance.ShakeCamera();
            return;
        }

        await ApproveMerge(e);
        if (result.IsWhite)
        {
            e.Target.PopTopColor();
        }

    }


    private ColorVector ProcessColorMerge(Tile source, Tile target)
    {
        return ColorFusion.Fuse(source.PeekColor(), target.PeekColor());
    }


    private async Task ApproveMerge(TileFuseEvent e)
    {
        PlayerPrefsService.RemainingMoves -= 1;
        await e.Source.TransferColorTo(e.Target);
        await EventBus.PublishAuto(new UpdateMoveCountUIEvent(PlayerPrefsService.RemainingMoves));
        await Task.CompletedTask;
    }

  
   
}

