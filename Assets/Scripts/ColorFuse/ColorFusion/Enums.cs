// #region enum
// public enum ButtonType{Menu,Start, Restart, Next, Fail, ExtraTime, ExtraMoves, ExtraSlot}
// public enum GameStatus { Playing, Win, Fail, TimeOver, MoveOver, Pause, Resume, Quit, None }

// public enum PanelType { Screen, Overlay }
// public enum ScreenType { Menu, Game, Loading, Pause}
// public enum OverlayType { Win, Fail, TimeOver, NoMoves, None}
// public enum AudioType { ButtonClick, Explosion, PowerUp, BackgroundMusic }
// public enum RewardType { Moves, Time, Slot, Coin, Life }

// public enum CurrencyType { Coin, Life }




// #endregion

#region UI
public enum ButtonType { Menu, Start, Restart, Next, Fail, ExtraTime, ExtraMoves, ExtraSlot }
public enum PanelType { Screen, Overlay }
public enum ScreenType { Menu, Game, Loading, Pause }
public enum OverlayType { Win, Fail, TimeOver, NoMoves, None ,Resume}
public enum TransitionType {FadeIn, FadeOut, SlideIn, SlideOut, Swipe}
#endregion

#region Game
public enum GameStatus { Playing, Win, Fail, TimeOver, MoveOver, Pause, Resume, Quit, None }
#endregion

#region Audio
public enum _AudioType { ButtonClick, RewardSFX, BackgroundMusic, TransitionSFX }

public enum AudioType { SFX, Music }
// public enum BackgroundMusic { Menu, Playing, Win, Fail, Pause, Resume, None }
// public enum RewardSFX { Moves, Time, Slot, Coin, Life }
// public enum TransitionSFX { Fade, Slide, Swipe}
#endregion

#region Economy
public enum RewardType { Moves, Time, Slot, Coin, Life }
public enum CurrencyType { Coin, Life }
#endregion
