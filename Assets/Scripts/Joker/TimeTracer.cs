using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GameEvents;
using UnityEngine;

public class TimeTracer : MonoBehaviour, IGameSystem,IPausable,IQuittable
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
        GameTimer.Pause();
        await Task.CompletedTask;
    }

    public async void OnResume()
    {
        GameTimer.Resume();
        await Task.CompletedTask;

    }

    public void OnQuit()
    {
        GameTimer.StopTimer();
        

    }



    private void SubscribeEvents()
    {
        // _eventSubscriptions.Add(EventBus.Subscribe<GamePlayStartedEvent>(HandleGameStart));
        // _eventSubscriptions.Add(EventBus.Subscribe<GamePlayPausedEvent>(HandleGamePause));
        // _eventSubscriptions.Add(EventBus.Subscribe<GamePlayResumedEvent>(HandleGameResume));
    }
    private void UnsubscribeEvents()
    {
        foreach (var subscription in _eventSubscriptions)
        {
            subscription?.Dispose();
        }
        _eventSubscriptions.Clear();
    }

    // private void HandleGameStart(GamePlayStartedEvent e)
    // {
    //     GameTimer.StopTimer();
    //     GameTimer.StartTimer(30);
    // }
    // private void HandleGamePause(GamePlayPausedEvent e) => GameTimer.Pause();

    // private void HandleGameResume(GamePlayResumedEvent e) => GameTimer.Resume();





}


