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
        public readonly PauseType Reason;
        public GamePauseEvent(PauseType reason) => Reason = reason;
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
    public readonly struct LayOverChangeEvent : IGameEvent
    {
        public readonly LayOverType Screen;
        public readonly object TransitionData;

        public LayOverChangeEvent(LayOverType screen, object data = null)
        {
            Screen = screen;
            TransitionData = data;
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
        public readonly IColorSource colorSource;
        public TileSelectionEvent(IColorSource colorSource) => this.colorSource = colorSource;
    }

    public readonly struct TileFuseEvent : IGameEvent
    {
        public readonly IColorSource Source;
        public readonly IColorSource Target;
        public TileFuseEvent(IColorSource source, IColorSource target) => (Source, Target) = (source, target);
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

    #region requests-ad

    public readonly struct ExtraUndoReguestedEvent : IGameEvent
    {

    }
    public readonly struct ExtraTimeRequestedEvent : IGameEvent
    {

    }
    public readonly struct ExtraMovesRequestedEvent : IGameEvent
    {

    }
    public readonly struct ExrtaReverseTileRequestedEvent : IGameEvent
    {

    }
    public readonly struct ExtraSlotRequestedEvent : IGameEvent
    {

    }
    #endregion
    
}

