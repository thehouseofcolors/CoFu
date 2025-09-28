using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GameEvents;
using UnityEngine;

public class MoveCounter : MonoBehaviour, IGameSystem,IPausable,IQuittable
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
    public async void OnPause()
    {
        
        await Task.CompletedTask;
    }

    public async void OnResume()
    {
        
        await Task.CompletedTask;

    }

    public void OnQuit()
    {
                
    }



    private void SubscribeEvents()
    {
        // _eventSubscriptions.Add(EventBus.Subscribe<GamePlayStartedEvent>(HandleGameStart));
        // _eventSubscriptions.Add(EventBus.Subscribe<GamePlayPausedEvent>(HandleGamePause));
        // _eventSubscriptions.Add(EventBus.Subscribe<GamePlayResumedEvent>(HandleGameResume));
        // _eventSubscriptions.Add(EventBus.Subscribe<GamePlayWonEvent>(HandleGameWin));
        _eventSubscriptions.Add(EventBus.Subscribe<TileFuseEvent>(HandleTileFuse));
    }
    private void UnsubscribeEvents()
    {
        foreach (var subscription in _eventSubscriptions)
        {
            subscription?.Dispose();
        }
        _eventSubscriptions.Clear();
    }
    private void HandleTileFuse(TileFuseEvent e) { }
    // private void HandleGameStart(GamePlayStartedEvent e)
    // {
        
    // }
    // private void HandleGameWin(GamePlayWonEvent e) {}
    // private void HandleGamePause(GamePlayPausedEvent e) {}

    // private void HandleGameResume(GamePlayResumedEvent e) {}





}
