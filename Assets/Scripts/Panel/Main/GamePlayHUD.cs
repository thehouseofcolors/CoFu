
// using System.Threading.Tasks;
// using TMPro;
// using UnityEngine;
// using GameEvents;
// using System;
// using System.Collections.Generic;

// // Handles game HUD logic and updates
// public class GamePlayHUD : MonoBehaviour, IGameSystem
// {
//     [Header("UI References")]
//     [SerializeField] private TextMeshProUGUI moveText;
//     [SerializeField] private TextMeshProUGUI timerText;
//     [SerializeField] private TextMeshProUGUI coinText;
//     [SerializeField] private TextMeshProUGUI lifeText;


//     // [Header("Animation Settings")]
//     // [SerializeField] private float textUpdateScale = 1.1f;
//     // [SerializeField] private float textUpdateDuration = 0.2f;

//     private List<IDisposable> disposables = new List<IDisposable>();
//     private Vector3 _originalMoveTextScale;
//     private Vector3 _originalTimerTextScale;

//     public async Task Initialize()
//     {
//         CacheOriginalScales();
//         ResetUI();
//         SubscribeToEvents();
//         await Task.CompletedTask;
//     }

//     private void CacheOriginalScales()
//     {
//         _originalMoveTextScale = moveText.transform.localScale;
//         _originalTimerTextScale = timerText.transform.localScale;
//     }

//     private void ResetUI()
//     {
//         moveText.text = "Moves: 0";
//         timerText.text = "Time: 0";
//     }

//     private void SubscribeToEvents()
//     {
//         disposables.Add(EventBus.Subscribe<UpdateMoveCountUIEvent>(OnMoveUpdate));
//         disposables.Add(EventBus.Subscribe<UpdateTimerUIEvent>(OnTimerUpdate));
//     }

//     public async Task Shutdown()
//     {
//         foreach (var disposable in disposables)
//         {
//             disposable?.Dispose();
//         }
//         await Task.CompletedTask;
//     }

//     private async Task OnMoveUpdate(UpdateMoveCountUIEvent evt)
//     {
//         moveText.text = $"Moves: {evt.MoveCount}";
//         await Task.CompletedTask;
//     }

//     private async Task OnTimerUpdate(UpdateTimerUIEvent evt)
//     {
//         timerText.text = $"Time: {Mathf.CeilToInt(evt.RemainingTime)}";
//         if (evt.RemainingTime == 0)
//         {
//             //publişhh timeout
//         }
//         // Add urgency effect when time is low
//         if (evt.RemainingTime < 10f)
//         {
//             timerText.color = Color.red;
//         }
//         else
//         {
//             timerText.color = Color.white;
//         }

//         await Task.CompletedTask;
//     }

// }

//panele ekle

using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using GameEvents;
using System;
using System.Collections.Generic;

// Handles game HUD logic and updates
//panele ekle

public class GamePlayHUD : MonoBehaviour, IPausable, IQuittable
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI moveText;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI coinText;
    [SerializeField] private TextMeshProUGUI lifeText;



    private List<IDisposable> disposables = new List<IDisposable>();
    private Vector3 _originalMoveTextScale;
    private Vector3 _originalTimerTextScale;
    private bool isInitialized;
    void Awake()
    {
        Setup();
    }
    public void Setup()
    {
        if (isInitialized) return;
        CacheOriginalScales();
        ResetUI();
        SubscribeToEvents();
        isInitialized = true;
    }

    public void OnPause() { }

    public void OnResume() { }

    public void OnQuit() { }

    private void CacheOriginalScales()
    {
        _originalMoveTextScale = moveText.transform.localScale;
        _originalTimerTextScale = timerText.transform.localScale;
    }

    private void ResetUI()
    {
        moveText.text = "Moves: 0";
        timerText.text = "Time: 0";
    }

    private void SubscribeToEvents()
    {
        disposables.Add(EventBus.Subscribe<UpdateMoveCountUIEvent>(OnMoveUpdate));
        disposables.Add(EventBus.Subscribe<UpdateTimerUIEvent>(OnTimerUpdate));
    }

    public void OnDestroy()
    {
        foreach (var disposable in disposables)
        {
            disposable?.Dispose();
        }
    }

    private async Task OnMoveUpdate(UpdateMoveCountUIEvent evt)
    {
        moveText.text = $"Moves: {evt.MoveCount}";
        await Task.CompletedTask;
    }

    private async Task OnTimerUpdate(UpdateTimerUIEvent evt)
    {
        timerText.text = $"Time: {Mathf.CeilToInt(evt.RemainingTime)}";
        if (evt.RemainingTime == 0)
        {
            //publişhh timeout
        }
        // Add urgency effect when time is low
        if (evt.RemainingTime < 10f)
        {
            timerText.color = Color.red;
        }
        else
        {
            timerText.color = Color.white;
        }

        await Task.CompletedTask;
    }

}

