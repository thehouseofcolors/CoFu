namespace GameEvents
{
    #region GameFlow
    public readonly struct GameLoadEvent : IGameEvent
    {
        public readonly LevelConfig Level;
        public GameLoadEvent(LevelConfig level) => Level = level;
    }

    public readonly struct GameWinEvent : IGameEvent { }

    public readonly struct GameFailEvent : IGameEvent { }


    public readonly struct GamePauseEvent : IGameEvent
    {
        public readonly GamePauseType Reason;
        public GamePauseEvent(GamePauseType reason) => Reason = reason;
    }

    public readonly struct GameResumeEvent : IGameEvent { }

    #endregion

    #region UIEvents


    public readonly struct ScreenChangeEvent : IGameEvent
    {
        public readonly ScreenType Screen;
        public readonly object TransitionData;

        public ScreenChangeEvent(ScreenType screen, object data = null)
        {
            Screen = screen;
            TransitionData = data;
        }
    }

    public readonly struct PanelShownEvent : IGameEvent
    {
        public readonly ScreenType ScreenType;
        public readonly object TransitionData;

        public PanelShownEvent(ScreenType screenType, object transitionData)
        {
            ScreenType = screenType;
            TransitionData = transitionData;
        }
    }

    public readonly struct NextLevelRequestedEvent : IGameEvent { }
    public readonly struct MenuRequestedEvent : IGameEvent { }
    public readonly struct LevelRestartRequestedEvent : IGameEvent { }
    public readonly struct GameStartRequestedEvent : IGameEvent { }

    public readonly struct LevelInfoUpdateEvent : IGameEvent
    {
        public readonly int LevelNumber;
        public LevelInfoUpdateEvent(int levelNumber) => LevelNumber = levelNumber;
    }
    #endregion

    #region Gameplay
    public readonly struct TileSelectionEvent : IGameEvent
    {
        public readonly Tile Tile;
        public TileSelectionEvent(Tile tile) => Tile = tile;
    }

    public readonly struct TileFuseEvent : IGameEvent
    {
        public readonly Tile Source;
        public readonly Tile Target;
        public TileFuseEvent(Tile source, Tile target) => (Source, Target) = (source, target);
    }

    public readonly struct UpdateTimerUIEvent : IGameEvent
    {
        public readonly float RemainingTime;
        public UpdateTimerUIEvent(float time) => RemainingTime = time;
    }

    public readonly struct UpdateMoveCountUIEvent : IGameEvent
    {
        public readonly int MoveCount;
        public UpdateMoveCountUIEvent(int count) => MoveCount = count;
    }
    #endregion


}

