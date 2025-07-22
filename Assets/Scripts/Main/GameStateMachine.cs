using System;
using System.Threading.Tasks;
using UnityEngine;
using GameEvents;

public static class GameStateMachine
{
    private static IGameState _currentState;
    private static bool _isTransitioning;

    public static IGameState CurrentState => _currentState;
    public static bool IsTransitioning => _isTransitioning; // Optional: expose if needed

    public static async Task SetStateAsync(IGameState newState)
    {
        if (_isTransitioning)
        {
            Debug.LogWarning($"Blocked transition to {newState?.GetType().Name} (already transitioning)");
            return;
        }

        _isTransitioning = true;

        try
        {
            await (_currentState?.ExitAsync() ?? Task.CompletedTask);
            _currentState = newState;
            await (_currentState?.EnterAsync() ?? Task.CompletedTask);
        }
        catch (Exception ex)
        {
            Debug.LogError($"State failed: {ex}");
            // Optional: Add recovery logic here if needed
            throw; // Re-throw if you want callers to handle it
        }
        finally
        {
            _isTransitioning = false;
        }
    }

    public static bool IsInState<T>() where T : IGameState => _currentState is T;
}

public abstract class GameStateBase : IGameState
{
    public virtual Task EnterAsync() => Task.CompletedTask;
    public virtual Task ExitAsync() => Task.CompletedTask;

    protected async Task PublishEventAsync(IGameEvent gameEvent)
    {
        try
        {
            await EventBus.PublishAuto(gameEvent);
        }
        catch(Exception ex)
        {
            Debug.LogError($"Event publish failed: {ex}");
        }
    }
}

public class PlayingState : GameStateBase
{
    LevelConfig levelConfig = LevelManager.Instance.CurrentLevel;
    
    public override async Task EnterAsync()
    {
        if (levelConfig == null)
        {
            Debug.LogError("Missing level config!");
            return;
        }
        await PublishEventAsync(new GameLoadEvent());

        Debug.Log("[State] Entering game loading");

        await PublishEventAsync(new GameStartEvent());
        EnableInput();
        StartTimer(levelConfig.JokerConfig.TimeLimit);
    }
    public override async Task ExitAsync()
    {
        DisableInput();
        PauseTimer();
        await Task.CompletedTask;
    }


    public void EnableInput() => TileInputHandler.Instance.EnableInput();
    public void DisableInput() => TileInputHandler.Instance.DisableInput();
    public void StartTimer(float duration) => GameTimer.StartTimer(duration);
    public void PauseTimer() => GameTimer.Pause();
    public void ResumeTimer() => GameTimer.Resume();
    public void ExtendTimer(float duration) => GameTimer.StartTimer(duration);//sadece zaman bittiğinde çağrılabildiği için yeniden başlatmakla aynı

}

public class NonPlayingState : GameStateBase
{
    public NonPlayingState()
    {
    }
    public override async Task EnterAsync()
    {
        Debug.Log("[State] Game Paused");
        
        await Task.CompletedTask;

    }
}

