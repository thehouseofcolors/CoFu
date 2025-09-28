using System;
using System.Threading.Tasks;
using UnityEngine;
using GameEvents;


public abstract class GameStateBase : IGameState
{
    protected LevelConfig LevelConfig => LevelManager.Instance.CurrentLevelConfig;
    

    public virtual async Task EnterAsync() => await Task.CompletedTask;
    public virtual async Task ExitAsync() => await Task.CompletedTask;

    protected async Task SetGameplayActivity(bool isActive) =>
        await EventBus.PublishAuto(new GamePlayActivityChangedEvent(isActive));
}
public class PlayingState : GameStateBase
{
    public override async Task EnterAsync()
    {
        if (LevelConfig == null)
        {
            Debug.LogError("Missing level config!");
            return;
        }

        Debug.Log("[State] Entering PlayingState");

        ResourceManager.InitializeLevelResources(LevelConfig);

        await EventBus.PublishAuto(new UIChangeAsyncEvent(ScreenType.Game));
        await Task.Delay(1000);
        await EventBus.PublishAuto(new GridAnimateEvent(LevelConfig));

        await SetGameplayActivity(true);
    }

    public override async Task ExitAsync()
    {
        await SetGameplayActivity(false);
    }
}
public class WonState : GameStateBase
{
    public override async Task EnterAsync()
    {
        Debug.Log("[State] Entering WonState");
        await EventBus.PublishAuto(new UIChangeAsyncEvent(OverlayType.Win));
        await Task.Delay(1000);
    }
}
public class FailedState : GameStateBase
{
    public override async Task EnterAsync()
    {
        Debug.Log("[State] Entering FailedState");
        await EventBus.PublishAuto(new UIChangeAsyncEvent(OverlayType.Fail));
        await Task.Delay(1000);
    }
}
public class TimeOutState : GameStateBase
{
    public override async Task EnterAsync()
    {
        Debug.Log("[State] Entering TimeOutState");
        await EventBus.PublishAuto(new UIChangeAsyncEvent(OverlayType.TimeOver));
        await Task.Delay(1000);
    }
}

public class MoveOutState : GameStateBase
{
    public override async Task EnterAsync()
    {
        Debug.Log("[State] Entering ");
        await EventBus.PublishAuto(new UIChangeAsyncEvent(OverlayType.NoMoves));
        await Task.Delay(1000);
        await SetGameplayActivity(false);
    }
}
public class MenuState : GameStateBase
{
    public override async Task EnterAsync()
    {
        Debug.Log("[State] Entering  ");
        await EventBus.PublishAuto(new UIChangeAsyncEvent(ScreenType.Menu));
        await Task.Delay(1000);
    }
}

//bu ikisine daha sonra telefonda test ederken dön
public class PlayingResumeState : GameStateBase
{
    public override async Task EnterAsync()
    {
        if (LevelConfig == null)
        {
            Debug.LogError("Missing level config!");
            return;
        }

        Debug.Log("[State] Entering PlayingState");

        await EventBus.PublishAuto(new UIChangeAsyncEvent(ScreenType.Game));
        await Task.Delay(1000);
        await EventBus.PublishAuto(new GridAnimateEvent(LevelConfig));

        await SetGameplayActivity(true);
    }

    public override async Task ExitAsync()
    {
        GameTimer.Pause();
        await SetGameplayActivity(false);
    }
}
public class PlayingPausetate : GameStateBase
{
    public override async Task EnterAsync()
    {
        if (LevelConfig == null)
        {
            Debug.LogError("Missing level config!");
            return;
        }

        Debug.Log("[State] Entering PlayingState");

        await EventBus.PublishAuto(new UIChangeAsyncEvent(ScreenType.Game));
        await Task.Delay(1000);
        await EventBus.PublishAuto(new GridAnimateEvent(LevelConfig));

        await SetGameplayActivity(true);
    }

    public override async Task ExitAsync()
    {
        GameTimer.Pause();
        await SetGameplayActivity(false);
    }
}
