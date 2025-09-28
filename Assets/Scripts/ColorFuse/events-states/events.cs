using System;
using UnityEngine;
using UnityEngine.UI;

namespace GameEvents
{

    public readonly struct UIChangeAsyncEvent : IGameEvent
    {
        //tek dinleyen yer UIManager
        //tetikleyen çok yer var 

        public readonly ScreenType ScreenType;
        public readonly OverlayType OverlayType;
        public readonly object TransitionData;

        public bool IsOverlay => OverlayType != OverlayType.None;

        public UIChangeAsyncEvent(ScreenType screenType, object transitionData = null)
        {
            ScreenType = screenType;
            OverlayType = OverlayType.None;
            TransitionData = transitionData;
        }

        public UIChangeAsyncEvent(OverlayType overlayType, object transitionData = null)
        {
            ScreenType = default;
            OverlayType = overlayType;
            TransitionData = transitionData;
        }
    }

    #region HUDTextChangeEvents
    //HUD control dinliyor
    //miktarın değiştiği sınıflardan ayrı ayrı tetikleniyorlar
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
    public readonly struct UpdateLifeUIEvent : IGameEvent
    {
        public readonly int LifeCount;
        public UpdateLifeUIEvent(int count) => LifeCount = count;
    }
    public readonly struct UpdateCoinUIEvent : IGameEvent
    {
        public readonly int CoinCount;
        public UpdateCoinUIEvent(int count) => CoinCount = count;
    }

    #endregion

    #region AdRewards-GameEconomy
    public readonly struct CurrencyChangedEvent : IGameEvent
    {
        //belki ses veya effect eklerim diye duruyor
        public readonly CurrencyType currencyType;
        public readonly int amount;
        public CurrencyChangedEvent(CurrencyType currencyType, int amount)
        {
            this.currencyType = currencyType;
            this.amount = amount;
        }
    }
    public readonly struct RewardRequestedEvent : IGameEvent
    {
        //şimdilik rewardrequestprosessor dinliyor
        //ilgili buttonlardan tetikleniyor. eksik varsa ekle

        public RewardType RewardType { get; }

        public RewardRequestedEvent(RewardType type)
        {
            RewardType = type;
        }
    }
    //eğer coin ve life varsa result tetiklenieck
    //ama burayıda aynı script hem dinliyo hem tetikliyor kararsızım
    //resultu yine ses ve görüntü dinlesin diye bırakabilirim
    public readonly struct RewardResultEvent : IGameEvent
    {
        public RewardType RewardType { get; }
        public bool IsSuccess { get; }
        public int? Amount { get; }

        public RewardResultEvent(RewardType rewardType, bool isSuccess, int? amount = null)
        {
            RewardType = rewardType;
            IsSuccess = isSuccess;
            Amount = amount;
        }
    }

    #endregion

    #region Gameplay
    //ilk iki eventte sorun yok şimdilik hiç elleme
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

    //son iki eventide ses ve görüntü için yazdım 
    //fussionprosesor içinden tetiklenecekler
    public readonly struct InvalidFusionEvent : IGameEvent { }
    public readonly struct WhiteFusionEvent : IGameEvent { }




    #endregion

    public readonly struct PlayAudioEvent : IGameEvent
    {
        public readonly AudioEntry AudioEntry;
        public PlayAudioEvent(AudioEntry audioEntry)
        {
            AudioEntry = audioEntry;
        }
    }

    // public readonly struct StopPlayingAudioEvent : IGameEvent { }
    //stopu kendi içine yazdım dışardan çağırınca hata veriyor


    #region GameFlow
    public readonly struct ButtonClickedEvent : IGameEvent
    {
        //bu eevent görüntü ve ses için 
        //henüz dinleyen yok 
        //request eventleriyle beraber onclick methodlarının içinden tetikle
        public readonly ButtonType buttonType;
        public readonly Button button;
        public ButtonClickedEvent(ButtonType type, Button button)
        {
            buttonType = type;
            this.button = button;
        }
    }
    public readonly struct GameStartRequestedEvent : IGameEvent { }//menü ekranındaki start button basılınca tetiklenicek
    public readonly struct MenuRequestedEvent : IGameEvent { }//menü buttonlarına basınca tetiklenicek
    // public readonly struct NextLevelRequestedEvent : IGameEvent { }//win panelde next basılınca tetiklenecek
    public readonly struct LevelRestartRequestedEvent : IGameEvent { }//restart buttonuna basılınca tetiklenecek



    // public readonly struct GameStatusChangeRequestEvent : IGameEvent
    // {
    //     public GameStatus requestedStatus { get; }

    //     public GameStatusChangeRequestEvent(GameStatus status)
    //     {
    //         requestedStatus = status;
    //     }
    // }

    // public readonly struct GameStatusChangedEvent : IGameEvent
    // {
    //     public GameStatus Status { get; }
    //     public object Data { get; }

    //     public GameStatusChangedEvent(GameStatus status, object data = null)
    //     {
    //         Status = status;
    //         Data = data;
    //     }
    // }


    public readonly struct GridAnimateEvent : IGameEvent
    {
        public LevelConfig Config { get; }

        public GridAnimateEvent(LevelConfig config)
        {
            Config = config;
        }
    }

    public readonly struct GridDestroyEvent : IGameEvent { }

    //tile animasyonu bitince oyun playmodu başlasın istiyorum

    public readonly struct GamePlayActivityChangedEvent : IGameEvent
    {
        //bunu input ve timer dinlemeli 
        //ve belki buttonları active inactive yaparım
        public readonly bool IsActive;
        public GamePlayActivityChangedEvent(bool isActive)
        {
            IsActive = isActive;
        }
    }




    // public readonly struct GamePlayStartedEvent : IGameEvent { }//bunu input ve timer dinlemeli
    // public readonly struct GamePlayWonEvent : IGameEvent { }
    // public readonly struct GamePlayFailedEvent : IGameEvent { }

    // public readonly struct GamePlayPausedEvent : IGameEvent { }//bunu input ve timer dinlemeli
    // public readonly struct GamePlayResumedEvent : IGameEvent { }//bunu input ve timer dinlemeli
    //pause ve resume 
    //oyun içi overlay gösterildiğinde tetiklenecek
    //time duracak input inactif olacak
    #endregion

}