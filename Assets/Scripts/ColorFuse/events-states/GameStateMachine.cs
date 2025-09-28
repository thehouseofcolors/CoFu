// using System;
// using System.Threading.Tasks;
// using UnityEngine;
// using GameEvents;

// public static class GameStateMachine
// {
//     private static IGameState _currentState;
//     private static bool _isTransitioning;

//     public static IGameState CurrentState => _currentState;
//     public static bool IsTransitioning => _isTransitioning; // Optional: expose if needed

//     public static async Task SetStateAsync(IGameState newState)
//     {
//         if (_isTransitioning)
//         {
//             Debug.LogWarning($"Blocked transition to {newState?.GetType().Name} (already transitioning)");
//             return;
//         }

//         _isTransitioning = true;

//         try
//         {
//             await (_currentState?.ExitAsync() ?? Task.CompletedTask);
//             _currentState = newState;
//             await (_currentState?.EnterAsync() ?? Task.CompletedTask);
//         }
//         catch (Exception ex)
//         {
//             Debug.LogError($"State failed: {ex}");
//             // Optional: Add recovery logic here if needed
//             throw; // Re-throw if you want callers to handle it
//         }
//         finally
//         {
//             _isTransitioning = false;
//         }
//     }

//     public static bool IsInState<T>() where T : IGameState => _currentState is T;
// }

// public abstract class GameStateBase : IGameState
// {
//     public virtual async Task EnterAsync()
//     {
//         //ses çalmaya başla
//         await Task.CompletedTask;
//     }

//     public virtual async Task ExitAsync()
//     {
//         //sesi sustur
//         await Task.CompletedTask;
//     }

//     protected async Task PublishEventAsync(IGameEvent gameEvent)
//     {
//         try
//         {
//             await EventBus.PublishAuto(gameEvent);
//         }
//         catch (Exception ex)
//         {
//             Debug.LogError($"Event publish failed: {ex}");
//         }
//     }
// }

// public class PlayingState : GameStateBase
// {
//     LevelConfig levelConfig = LevelManager.Instance.CurrentLevel;
    
//     public override async Task EnterAsync()
//     {
//         if (levelConfig == null)
//         {
//             Debug.LogError("Missing level config!");
//             return;
//         }
//         Debug.Log("[State] Entering PlayingState");

//         await EventBus.PublishAuto(new UIChangeAsyncEvent(ScreenType.Game));
//         await Task.Delay(100);
//         await EventBus.PublishAuto(new GridAnimateEvent(levelConfig));

//         await PublishEventAsync(new GamePlayActivityChangedEvent(true));//bunu input ve timer dinlemeli
//     }
//     public override async Task ExitAsync()
//     {
//         PauseTimer();
//         await PublishEventAsync(new GamePlayActivityChangedEvent(false));
//         await Task.CompletedTask;
//     }


//     public void StartTimer(float duration) => GameTimer.StartTimer(duration);
//     public void PauseTimer() => GameTimer.Pause();
//     public void ResumeTimer() => GameTimer.Resume();
//     public void ExtendTimer(float duration) => GameTimer.StartTimer(duration);//sadece zaman bittiğinde çağrılabildiği için yeniden başlatmakla aynı

// }

// public class MenuState : GameStateBase
// {
    
// }


// public class WonState : GameStateBase
// {
//     LevelConfig levelConfig = LevelManager.Instance.CurrentLevel;
    
//     public override async Task EnterAsync()
//     {
        
//         Debug.Log("[State] Entering PlayingState");
//         await EventBus.PublishAuto(new UIChangeAsyncEvent(OverlayType.Win));
//         await Task.Delay(1000);
//         Debug.Log("[State] Entering game loading");

//         await PublishEventAsync(new GamePlayActivityChangedEvent(false));//bunu input ve timer dinlemeli
//     }
//     public override async Task ExitAsync()
//     {
//         await Task.CompletedTask;
//     }
// }
// public class FailedState : GameStateBase
// {
//     LevelConfig levelConfig = LevelManager.Instance.CurrentLevel;
    
//     public override async Task EnterAsync()
//     {
//         await EventBus.PublishAuto(new UIChangeAsyncEvent(OverlayType.Fail));
//         await Task.Delay(1000);
//         Debug.Log("[State] Entering game loading");

//         await PublishEventAsync(new GamePlayActivityChangedEvent(false));//bunu input ve timer dinlemeli
//     }
//     public override async Task ExitAsync()
//     {
//         await Task.CompletedTask;
//     }


// }
// public class ResumingState : GameStateBase
// {
//     LevelConfig levelConfig = LevelManager.Instance.CurrentLevel;
    
//     public override async Task EnterAsync()
//     {
//         if (levelConfig == null)
//         {
//             Debug.LogError("Missing level config!");
//             return;
//         }
//         Debug.Log("[State] Entering PlayingState");

//         // GameContext.Runtime.Setup(levelConfig);
//         // await PublishEventAsync(new UIChangeAsyncEvent(ScreenType.Game));
//         // await Task.Delay(1000);
//         // await PublishEventAsync(new GridAnimateEvent(levelConfig));
//         await EventBus.PublishAuto(new UIChangeAsyncEvent(ScreenType.Game));
//         await Task.Delay(1000);
//         await EventBus.PublishAuto(new GridAnimateEvent(levelConfig));
//         Debug.Log("[State] Entering game loading");

//         await PublishEventAsync(new GamePlayActivityChangedEvent(true));//bunu input ve timer dinlemeli
//     }
//     public override async Task ExitAsync()
//     {
//         PauseTimer();
//         await Task.CompletedTask;
//     }


//     public void StartTimer(float duration) => GameTimer.StartTimer(duration);
//     public void PauseTimer() => GameTimer.Pause();
//     public void ResumeTimer() => GameTimer.Resume();
//     public void ExtendTimer(float duration) => GameTimer.StartTimer(duration);//sadece zaman bittiğinde çağrılabildiği için yeniden başlatmakla aynı

// }
// public class TimeOutState : GameStateBase
// {
//     LevelConfig levelConfig = LevelManager.Instance.CurrentLevel;
    
//     public override async Task EnterAsync()
//     {
//         if (levelConfig == null)
//         {
//             Debug.LogError("Missing level config!");
//             return;
//         }
//         Debug.Log("[State] Entering PlayingState");
//         await EventBus.PublishAuto(new UIChangeAsyncEvent(OverlayType.TimeOver));
//         await Task.Delay(1000);
//         Debug.Log("[State] Entering game loading");

//         await PublishEventAsync(new GamePlayActivityChangedEvent(false));//bunu input ve timer dinlemeli
//     }
//     public override async Task ExitAsync()
//     {
//         await Task.CompletedTask;
//     }

// }
// public class MoveOutState : GameStateBase
// {
//     LevelConfig levelConfig = LevelManager.Instance.CurrentLevel;
    
//     public override async Task EnterAsync()
//     {
//         if (levelConfig == null)
//         {
//             Debug.LogError("Missing level config!");
//             return;
//         }
//         Debug.Log("[State] Entering PlayingState");
//         await EventBus.PublishAuto(new UIChangeAsyncEvent(OverlayType.NoMoves));
//         await Task.Delay(1000);
//         Debug.Log("[State] Entering game loading");

//         await PublishEventAsync(new GamePlayActivityChangedEvent(false));//bunu input ve timer dinlemeli
//     }
//     public override async Task ExitAsync()
//     {
//         await Task.CompletedTask;
//     }


// }

using System;
using System.Threading.Tasks;
using UnityEngine;
using GameEvents;

public static class GameStateMachine
{
    private static IGameState _currentState;
    private static bool _isTransitioning;

    public static IGameState CurrentState => _currentState;
    public static bool IsTransitioning => _isTransitioning;

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
            Debug.LogError($"State transition failed: {ex}");
            throw;
        }
        finally
        {
            _isTransitioning = false;
        }
    }

    public static bool IsInState<T>() where T : IGameState => _currentState is T;
}
