/// <summary>
/// 📄 Contains core interface declarations used across the game architecture.
/// 
/// 🔌 Interface List:
/// 
/// • IGameEvent – Marker interface for game-related event types
/// • IColorSource – Describes objects that can provide, display, or manipulate color data (e.g., tiles)
/// • IJoker – Defines common behavior for passive/active jokers (TryUse, TryBuy, Data)
/// • IGameSystem – Lifecycle-based systems with Initialize and Shutdown methods
/// • IPausable – For systems or UI that respond to pause/resume logic
/// • IQuittable – For objects that need cleanup on application quit
/// • IPanel – UI panels supporting transitions and pause/quit interaction (extends IPausable, IQuittable)
/// • IGameState – Defines game states with async enter/exit logic (e.g., gameplay, win, fail)
/// • IPersistent – For systems that support saving/loading to/from GameData
/// 
/// ⚙️ These interfaces define contracts between systems and are not meant to be modified frequently.
/// </summary>

using UnityEngine;
using System.Threading.Tasks;
public interface IGameEvent { }

public interface IColorSource
{
    void SetHighlight(bool on);
    void SetTemporarilyDisabled(bool disabled);
    ColorVector PopTopColor();
    ColorVector PeekColor();
    Vector3 GetWorldPosition();
    bool IsEmpty();
}



public interface IJoker
{
    JokerData Data { get; }
    bool TryUse();
    bool TryBuy();
}


public interface IGameSystem
{
    Task Initialize();
    Task Shutdown();
}
public interface IPausable
{
    void OnPause();
    void OnResume();
}

public interface IQuittable
{
    void OnQuit();
}

// public interface IPanel: IPausable, IQuittable
// {
//     // PanelType Type { get; set; }
//     Task ShowAsync(object transitionData);
//     Task HideAsync(object transitionData);
// }

public interface IPanel:IPausable,IQuittable
{
    // AudioSource AudioSource { get; }
    // ParticleSystem TransitionParticles { get; }
    PanelType PanelType { get; }
    
    Task ShowAsync(object transitionData=null);
    Task HideAsync(object transitionData=null);
    
    // void PrepareForTransition();
    // void CompleteTransition();
}

// public struct TransitionParams
// {
//     public TransitionType Type;
//     public float Duration;
//     public AudioClip CustomAudio;
//     public ParticleSystem CustomParticles;

//     // Default parameters
//     public static TransitionParams Default => new TransitionParams
//     {
//         Type = TransitionType.FadeIn,
//         Duration = 0.5f,
//     };
    
// }

public interface IGameState
{
    Task EnterAsync();
    Task ExitAsync();
    //pause resume eklesem mi??
}




public struct GameData { }

public interface IPersistent
{
    void Save(GameData data);
    void Load(GameData data);
}

