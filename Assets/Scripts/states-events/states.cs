using System.Threading.Tasks;
using UnityEngine;
using GameEvents;
using System;


public class MenuState : IGameState
{
    public async Task EnterAsync()
    {
        Debug.Log("[State] Entering Menu");

        TileInputHandler.Instance.DisableInput();
        await EventBus.PublishAuto(new ScreenChangeEvent(ScreenType.Menu));
    }
    public Task ExitAsync() => Task.CompletedTask;
}

public class GamePlayState : IGameState
{
    private readonly LevelConfig _levelConfig;
    private IDisposable _gameEndSubscription;

    public GamePlayState(LevelConfig levelConfig) => _levelConfig = levelConfig;

    public async Task EnterAsync()
    {
        Debug.Log($"[State] Starting Level {_levelConfig.level}");
        if (_levelConfig == null)
        {
            Debug.LogError("LevelConfig is NULL in GamePlayState!");
        }
        else
        {
            Debug.Log($"LevelConfig level is {_levelConfig.level}");
        }

        await EventBus.PublishAuto(new ScreenChangeEvent(ScreenType.Game));
        await EventBus.PublishAuto(new GameLoadEvent(_levelConfig));

        //müzik başlat
        GameTimer.Instance.StartTimer(_levelConfig.timeLimit);
        
        TileInputHandler.Instance.EnableInput();

        // Subscribe to game end conditions
        _gameEndSubscription = EventBus.Subscribe<GameEndConditionMetEvent>(OnGameEndCondition);
        
    }

    public async Task ExitAsync()
    {
        _gameEndSubscription?.Dispose();
        GameTimer.Instance.StopTimer();

        TileInputHandler.Instance.DisableInput();
        // müzik durdur
        await Task.CompletedTask;
    }
    private async void OnGameEndCondition(GameEndConditionMetEvent e)
    {
        if (e.IsWin)
        {
            await GameStateMachine.ChangeStateAsync(new GameWinState());
        }
        else
        {
            await GameStateMachine.ChangeStateAsync(new GameFailState(e.FailReason));
        }
    }

   
}

public class GamePauseState : IGameState
{
    public GamePauseType gamePauseType;
    public GamePauseState(GamePauseType gamePauseType)
    {
        this.gamePauseType = gamePauseType;
    }
    public async Task EnterAsync()
    {
        Debug.Log("[State] Game Paused");
        GameTimer.Instance.Pause();
        await EventBus.PublishAuto(new GamePauseEvent(gamePauseType));
    }

    public async Task ExitAsync()
    {
        Debug.Log("Game resumed");
        GameTimer.Instance.Resume();
        await Task.CompletedTask;
    }
}

public class GameWinState : IGameState
{
    public async Task EnterAsync()
    {
        Debug.Log("[State] Level Completed!");
        PlayerPrefsService.IncrementLevel();

        await EventBus.PublishAuto(new GameWinEvent());
        await EventBus.PublishAuto(new ScreenChangeEvent(ScreenType.Win));
        //müzük ve animasyon efect vs ekle

        await Task.Delay(2000);
    }

    public async Task ExitAsync()
    {
        //müzikdurdur, animasyon durdur
        await Task.CompletedTask;

    }
}
public class GameFailState : IGameState
{
    private readonly GameFailType _reason;

    public GameFailState(GameFailType reason) => _reason = reason;

    public async Task EnterAsync()
    {
        Debug.Log($"[State] Level Failed: {_reason}");
        //müzik animasyon effect ve ekle

        ScreenType screenType = _reason switch
        {
            GameFailType.TimeOver => ScreenType.Fail_TimeOver,
            GameFailType.NoMoves => ScreenType.Fail_NoMoves,
            _ => ScreenType.Menu
        };

        await EventBus.PublishAuto(new ScreenChangeEvent(screenType));
        await EventBus.PublishAuto(new GameFailEvent(_reason));
    }

    public Task ExitAsync() => Task.CompletedTask;//müzik ve ekliyse durdur
}





