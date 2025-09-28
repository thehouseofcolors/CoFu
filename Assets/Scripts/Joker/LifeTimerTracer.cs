using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GameEvents;
using UnityEngine;

public class LifeTimeTracer : MonoBehaviour, IGameSystem
{
    private CancellationTokenSource _cts;

    private List<IDisposable> _eventSubscriptions = new List<IDisposable>();


    public async Task Initialize()
    {
        SubscribeEvents();
        await Task.CompletedTask;

    }
    public async Task Shutdown()
    {
        UnsubscribeEvents();
        await Task.CompletedTask;
    }

    private void SubscribeEvents()
    {
        
    }
    private void UnsubscribeEvents()
    {
        foreach (var subscription in _eventSubscriptions)
        {
            subscription?.Dispose();
        }
        _eventSubscriptions.Clear();
    }
    
    public void Disable()
    {

    }




}
