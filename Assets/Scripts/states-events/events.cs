using System;
using UnityEngine;

namespace GameEvents
{

    #region UIEvents
    // public class UIChangeEvent:IGameEvent
    // {
    //     public ScreenType ScreenType { get; }
    //     public OverlayType OverlayType { get; }
    //     public object TransitionData { get; }
    //     public bool IsOverlay => OverlayType != OverlayType.None;

    //     public UIChangeEvent(ScreenType screenType, object transitionData = null)
    //     {
    //         ScreenType = screenType;
    //         OverlayType = OverlayType.None;
    //         TransitionData = transitionData;
    //     }

    //     public UIChangeEvent(OverlayType overlayType, object transitionData = null)
    //     {
    //         OverlayType = overlayType;
    //         TransitionData = transitionData;
    //     }
    // }
    public readonly struct UIChangeEvent : IGameEvent
    {
        public readonly ScreenType ScreenType;
        public readonly OverlayType OverlayType;
        public readonly object TransitionData;

        public bool IsOverlay => OverlayType != OverlayType.None;

        public UIChangeEvent(ScreenType screenType, object transitionData = null)
        {
            ScreenType = screenType;
            OverlayType = OverlayType.None;
            TransitionData = transitionData;
        }

        public UIChangeEvent(OverlayType overlayType, object transitionData = null)
        {
            ScreenType = default;
            OverlayType = overlayType;
            TransitionData = transitionData;
        }
    }


    public readonly struct ScreenChangeEvent : IGameEvent
    {
        public readonly ScreenType Screen;
        public readonly object TransitionData;

        public ScreenChangeEvent(ScreenType newScreen,  object data = null)
        {
            Screen = newScreen;
            TransitionData = data;
        }
    }
    public readonly struct OverlayChangeEvent : IGameEvent
    {
        public readonly OverlayType Screen;
        public readonly object TransitionData;

        public OverlayChangeEvent(OverlayType screen, object data = null)
        {
            Screen = screen;
            TransitionData = data;
        }
    }

    public readonly struct TransactionCompletedEvent : IGameEvent { }

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
        // public TileFuseEvent(IColorSource source, IColorSource target) => (Source, Target) = (source, target);
        public TileFuseEvent(IColorSource source, IColorSource target)
        {
            if (source == null || target == null)
                throw new ArgumentNullException();
            (Source, Target) = (source, target);
        }
    }



    #endregion


    #region GameFlow
    //bunlar buttonlara tıklama olayları
    public readonly struct GameLoadRequestedEvent : IGameEvent { }
    public readonly struct MenuRequestedEvent : IGameEvent { }
    public readonly struct NextLevelRequestedEvent : IGameEvent { }
    public readonly struct LevelRestartRequestedEvent : IGameEvent { }



    public readonly struct GameLoadEvent : IGameEvent { }
    public readonly struct GameStartEvent : IGameEvent { }
    public readonly struct GameWinEvent : IGameEvent { }
    public readonly struct GameFailEvent : IGameEvent { }
    public readonly struct GamePauseEvent : IGameEvent { }
    public readonly struct GameResumeEvent : IGameEvent { }

    #endregion

    #region AdRewards

    public readonly struct ExtraTimeRequestedEvent : IGameEvent { }
    public readonly struct ExtraMovesRequestedEvent : IGameEvent { }
    public readonly struct ExtraSlotRequestedEvent : IGameEvent { }
    public readonly struct ExtraCoinRequestedEvent : IGameEvent { }
    public readonly struct ExtraLifeRequestedEvent : IGameEvent { }

    #endregion
    #region GameEconomy

    public readonly struct ExtraTimeEarnEvent : IGameEvent { }
    public readonly struct ExtraMovesEarnEvent : IGameEvent { }
    public readonly struct ExtraSlotEarnEvent : IGameEvent { }

    public readonly struct ExtraCoinEarnEvent : IGameEvent { }
    public readonly struct ExtraLifeEarnEvent : IGameEvent { }

    

    public readonly struct CoinUseEvent : IGameEvent { }
    public readonly struct LifeUseEvent : IGameEvent { }
    #endregion

}

