

using System;
using System.Threading.Tasks;
using UnityEngine;
using GameEvents;
using System.Collections.Generic;

public class GamePlayRequestProsessor : MonoBehaviour, IGameSystem
{
    private int _defaultFallbackLevel = 1;
    private List<IDisposable> _subscriptions = new();
    private bool _isProcessingRequest;

    public async Task Initialize()
    {
        DisposeSubscriptions();

        _subscriptions.AddRange(new IDisposable[]
        {
            EventBus.Subscribe<GameStartRequestedEvent>(HandleGameStart),
            // EventBus.Subscribe<NextLevelRequestedEvent>(HandleNextLevelRequested),
            EventBus.Subscribe<LevelRestartRequestedEvent>(HandleLevelRestartRequested),
            EventBus.Subscribe<MenuRequestedEvent>(HandleMenuRequested)
        });

        await Task.CompletedTask;
    }

    public async Task Shutdown()
    {
        DisposeSubscriptions();
        await Task.CompletedTask;
    }
    private void DisposeSubscriptions()
    {
        foreach (var sub in _subscriptions)
            sub?.Dispose();
        _subscriptions.Clear();
    }


    private async Task HandleGameStart(GameStartRequestedEvent e)
    {
        await EventBus.PublishAuto(new GridAnimateEvent());
        Debug.Log("request işleniyoru");
        await Task.CompletedTask;
    }

    private async Task<LevelConfig> ResolveLevelConfig()
    {
        int levelId = LevelManager.CurrentLevel;
        LevelConfig levelConfig = LevelManager.Instance.CurrentLevelConfig;

        if (levelConfig != null)
        {
            Debug.LogWarning($"Level {levelId} not found! Falling back to {_defaultFallbackLevel}");
            LevelManager.CurrentLevel = _defaultFallbackLevel;
            return await ResolveLevelConfig(); // Retry with fallback
        }

        return levelConfig;
    }


    // private async Task HandleNextLevelRequested(NextLevelRequestedEvent e)
    // {
    //     // PlayerPrefsService.IncrementLevel();
    //     // await OnGameLoad(new GameLoadRequestedEvent());
    //     await Task.CompletedTask;
    // }

    private async Task HandleLevelRestartRequested(LevelRestartRequestedEvent e)
    {
        // await OnGameLoad(new GameLoadRequestedEvent());
        await Task.CompletedTask;
    }

    private async Task HandleMenuRequested(MenuRequestedEvent e)
    {
        // if (_isProcessingRequest) return;

        // await GameStateMachine.SetStateAsync(new NonPlayingState());
        await Task.CompletedTask;
    }


    
}

