using System;
using System.Threading;
using System.Threading.Tasks;
using GameEvents;
using UnityEngine;

// // // public class GameTimer : Singleton<GameTimer>, IGameSystem
// // // {
// // //     private float _remainingTime;
// // //     private bool _isRunning;
// // //     private bool _isPaused;
// // //     private CancellationTokenSource _cts;

// // //     private List<IDisposable> _eventSubscriptions = new List<IDisposable>();


// // //     public void Initialize()
// // //     {  
// // //         SubscribeEvents();
// // //     }
// // //     public void Shutdown()
// // //     {
// // //         UnsubscribeEvents();
// // //     }
// // //     private void SubscribeEvents()
// // //     {
// // //         _eventSubscriptions.Add(EventBus.Subscribe<GameStartEvent>(HandleGameStart));
// // //         _eventSubscriptions.Add(EventBus.Subscribe<GamePauseEvent>(HandleGamePause));
// // //         _eventSubscriptions.Add(EventBus.Subscribe<GameResumeEvent>(HandleGameResume));
// // //         _eventSubscriptions.Add(EventBus.Subscribe<GameWinEvent>(HandleGameWin));
// // //     }
// // //     private void UnsubscribeEvents()
// // //     {
// // //         foreach (var subscription in _eventSubscriptions)
// // //         {
// // //             subscription?.Dispose();
// // //         }
// // //         _eventSubscriptions.Clear();
// // //     }

// // //     private void HandleGameStart(GameStartEvent e)
// // //     {
// // //         StartTimer(30);
// // //     }
// // //     private void HandleGameWin(GameWinEvent e) => StopTimer();
// // //     private void HandleGamePause(GamePauseEvent e) => Pause();

// // //     private void HandleGameResume(GameResumeEvent e) => Resume();
// // //     public void Disable()
// // //     {
// // //         StopTimer();
// // //     }

// // //     public void StartTimer(float duration)
// // //     {
// // //         if (duration <= 0)
// // //         {
// // //             Debug.LogWarning("Timer duration must be positive");
// // //             return;
// // //         }

// // //         StopTimer();
// // //         _remainingTime = duration;
// // //         _isRunning = true;
// // //         _isPaused = false;
// // //         _cts = new CancellationTokenSource();

// // //         _ = RunTimerAsync(_cts.Token);
// // //     }

// // //     public void StopTimer()
// // //     {
// // //         _cts?.Cancel();
// // //         _cts?.Dispose();
// // //         _cts = null;
// // //         _isRunning = false;
// // //         _isPaused = false;
// // //     }

// // //     public void Pause()
// // //     {
// // //         if (_isRunning && !_isPaused)
// // //             _isPaused = true;
// // //     }

// // //     public void Resume()
// // //     {
// // //         if (_isRunning && _isPaused)
// // //             _isPaused = false;
// // //     }

// // //     public async Task AddTime(float seconds)
// // //     {
// // //         if (_isRunning)
// // //         {
// // //             _remainingTime += seconds;
// // //             await Task.CompletedTask;
// // //         }
// // //     }

// // //     private async Task RunTimerAsync(CancellationToken token)
// // //     {
// // //         try
// // //         {
// // //             while (_remainingTime > 0 && !token.IsCancellationRequested)
// // //             {
// // //                 if (_isPaused)
// // //                 {
// // //                     await EventBus.PublishAuto(new UpdateTimerUIEvent(_remainingTime));
// // //                     await Task.Delay(100, token);
// // //                     continue;
// // //                 }

// // //                 await Task.Delay(1000, token);
// // //                 if (token.IsCancellationRequested) break;

// // //                 _remainingTime -= 1f;
// // //                 await EventBus.PublishAuto(new UpdateTimerUIEvent(_remainingTime));
// // //             }

// // //             if (_remainingTime <= 0 && !token.IsCancellationRequested)
// // //             {
// // //                 await GameStateMachine.ChangeStateAsync(new GamePauseState(PauseType.TimeOver));
// // //             }
// // //         }
// // //         catch (OperationCanceledException)
// // //         {
// // //             // Timer durduruldu
// // //         }
// // //         finally
// // //         {
// // //             _isRunning = false;
// // //             _isPaused = true;
// // //         }
// // //     }

// // //     public float RemainingTime => _remainingTime;
// // //     public bool IsRunning => _isRunning;
// // //     public bool IsPaused => _isPaused;
// // // }

// // public static class GameTimer
// // {
// //     private static float _remainingTime;
// //     private static bool _isRunning;
// //     private static bool _isPaused;
// //     private static CancellationTokenSource _cts;



// //     public static void StartTimer(float duration)
// //     {
// //         if (duration <= 0)
// //         {
// //             Debug.LogWarning("Timer duration must be positive");
// //             return;
// //         }

// //         StopTimer();
// //         _remainingTime = duration;
// //         _isRunning = true;
// //         _isPaused = false;
// //         _cts = new CancellationTokenSource();

// //         _ = RunTimerAsync(_cts.Token);
// //     }

// //     public static void StopTimer()
// //     {
// //         _cts?.Cancel();
// //         _cts?.Dispose();
// //         _cts = null;
// //         _isRunning = false;
// //         _isPaused = false;
// //     }

// //     public static void Pause()
// //     {
// //         if (_isRunning && !_isPaused)
// //             _isPaused = true;
// //     }

// //     public static void Resume()
// //     {
// //         if (_isRunning && _isPaused)
// //             _isPaused = false;
// //     }

// //     public static async Task AddTime(float seconds)
// //     {
// //         if (_isRunning)
// //         {
// //             _remainingTime += seconds;
// //             await Task.CompletedTask;
// //         }
// //     }

// //     private static async Task RunTimerAsync(CancellationToken token)
// //     {
// //         try
// //         {
// //             while (_remainingTime > 0 && !token.IsCancellationRequested)
// //             {
// //                 if (_isPaused)
// //                 {
// //                     await EventBus.PublishAuto(new UpdateTimerUIEvent(_remainingTime));
// //                     await Task.Delay(100, token);
// //                     continue;
// //                 }

// //                 await Task.Delay(1000, token);
// //                 if (token.IsCancellationRequested) break;

// //                 _remainingTime -= 1f;
// //                 await EventBus.PublishAuto(new UpdateTimerUIEvent(_remainingTime));
// //             }

// //             if (_remainingTime <= 0 && !token.IsCancellationRequested)
// //             {
// //                 await GameStateMachine.ChangeStateAsync(new GamePauseState(PauseType.TimeOver));
// //             }
// //         }
// //         catch (OperationCanceledException)
// //         {
// //             // Timer durduruldu
// //         }
// //         finally
// //         {
// //             _isRunning = false;
// //             _isPaused = true;
// //         }
// //     }

// //     public static float RemainingTime => _remainingTime;
// //     public static bool IsRunning => _isRunning;
// //     public static bool IsPaused => _isPaused;
// // }


// // public static class GameTimer
// // {
// //     private static float _remainingTime;
// //     private static bool _isRunning;
// //     private static bool _isPaused;
// //     private static CancellationTokenSource _cts;

// //     // Events for external listeners
// //     // public static event Action<float> OnTimerUpdated;
// //     // public static event Action OnTimerStarted;
// //     // public static event Action OnTimerStopped;
// //     // public static event Action OnTimerPaused;
// //     // public static event Action OnTimerResumed;
// //     // public static event Action OnTimeExpired;

// //     public static void StartTimer(float duration)
// //     {
// //         if (duration <= 0)
// //         {
// //             Debug.LogWarning("Timer duration must be positive");
// //             return;
// //         }

// //         StopTimer();

// //         _remainingTime = duration;
// //         _isRunning = true;
// //         _isPaused = false;
// //         _cts = new CancellationTokenSource();

// //         OnTimerStarted?.Invoke();
// //         _ = RunTimerAsync(_cts.Token);
// //     }

// //     public static void StopTimer()
// //     {
// //         if (!_isRunning) return;

// //         _cts?.Cancel();
// //         _cts?.Dispose();
// //         _cts = null;
// //         _isRunning = false;
// //         _isPaused = false;

// //         OnTimerStopped?.Invoke();
// //     }

// //     public static void Pause()
// //     {
// //         if (!_isRunning || _isPaused) return;

// //         _isPaused = true;
// //         OnTimerPaused?.Invoke();
// //     }

// //     public static void Resume()
// //     {
// //         if (!_isRunning || !_isPaused) return;

// //         _isPaused = false;
// //         OnTimerResumed?.Invoke();
// //     }

// //     public static void AddTime(float seconds)
// //     {
// //         if (!_isRunning) return;

// //         _remainingTime = Mathf.Max(0, _remainingTime + seconds);
// //         UpdateTimerDisplay();
// //     }

// //     private static async Task RunTimerAsync(CancellationToken token)
// //     {
// //         try
// //         {
// //             var timerInterval = TimeSpan.FromSeconds(1);
// //             var nextUpdateTime = DateTime.UtcNow + timerInterval;

// //             while (_remainingTime > 0 && !token.IsCancellationRequested)
// //             {
// //                 if (_isPaused)
// //                 {
// //                     await Task.Delay(100, token);
// //                     continue;
// //                 }

// //                 var currentTime = DateTime.UtcNow;
// //                 if (currentTime < nextUpdateTime)
// //                 {
// //                     await Task.Delay(nextUpdateTime - currentTime, token);
// //                     continue;
// //                 }

// //                 _remainingTime -= 1f;
// //                 UpdateTimerDisplay();
// //                 nextUpdateTime += timerInterval;

// //                 if (_remainingTime <= 0)
// //                 {
// //                     HandleTimeExpired();
// //                     break;
// //                 }
// //             }
// //         }
// //         catch (OperationCanceledException)
// //         {
// //             // Timer was stopped
// //         }
// //         catch (Exception ex)
// //         {
// //             Debug.LogError($"Timer error: {ex.Message}");
// //         }
// //         finally
// //         {
// //             _isRunning = false;
// //             _isPaused = false;
// //         }
// //     }
// //     private static async Task _RunTimerAsync(CancellationToken token)
// //     {
// //         try
// //         {
// //             while (_remainingTime > 0 && !token.IsCancellationRequested)
// //             {
// //                 if (!Application.isFocused)
// //                 {
// //                     // Oyun arka planda, bekle
// //                     await Task.Delay(500, token);
// //                     continue;
// //                 }

// //                 if (Time.timeScale == 0)
// //                 {
// //                     // Oyun duraklatılmış, bekle
// //                     await Task.Delay(500, token);
// //                     continue;
// //                 }

// //                 // Normal timer işlemi
// //                 await Task.Delay(1000, token);
// //                 _remainingTime -= 1f;
// //                 await EventBus.PublishAuto(new UpdateTimerUIEvent(_remainingTime));
// //             }

// //             if (_remainingTime <= 0 && !token.IsCancellationRequested)
// //             {
// //                 // await EventBus.PublishAuto(new TimerExpiredEvent());
// //                 // await GameStateMachine.ChangeStateAsync(new GamePauseState(PauseType.TimeOver));
// //             }
// //         }
// //         catch (OperationCanceledException) { }
// //         finally
// //         {
// //             _isRunning = false;
// //         }
// //     }

// //     private static void UpdateTimerDisplay()
// //     {
// //         OnTimerUpdated?.Invoke(_remainingTime);
// //         EventBus.PublishAuto(new UpdateTimerUIEvent(_remainingTime));
// //     }

// //     private static void HandleTimeExpired()
// //     {
// //         OnTimeExpired?.Invoke();
// //         // _ = GameStateMachine.ChangeStateAsync(new GamePauseState(PauseType.TimeOver));
// //     }

// //     public static float RemainingTime => _remainingTime;
// //     public static bool IsRunning => _isRunning;
// //     public static bool IsPaused => _isPaused;
// // }

// public static class GameTimer
// {
//     private static float _remainingTime;
//     private static bool _isRunning;
//     private static bool _isPaused;
//     private static CancellationTokenSource _cts;


//     public static void StartTimer(float duration)
//     {
//         if (duration <= 0)
//         {
//             Debug.LogWarning("Timer duration must be positive");
//             return;
//         }

//         StopTimer();

//         _remainingTime = duration;
//         _isRunning = true;
//         _isPaused = false;
//         _cts = new CancellationTokenSource();

//         _ = RunTimerAsync(_cts.Token);
//     }

//     public static void StopTimer()
//     {
//         if (!_isRunning) return;

//         _cts?.Cancel();
//         _cts?.Dispose();
//         _cts = null;
//         _isRunning = false;
//         _isPaused = false;

//     }

//     public static void Pause()
//     {
//         if (!_isRunning || _isPaused) return;

//         _isPaused = true;
//     }

//     public static void Resume()
//     {
//         if (!_isRunning || !_isPaused) return;

//         _isPaused = false;
//     }

//     public static void AddTime(float seconds)
//     {
//         if (!_isRunning) return;

//         _remainingTime = Mathf.Max(0, _remainingTime + seconds);
//         UpdateTimerDisplay();
//     }

//     private static async Task RunTimerAsync(CancellationToken token)
//     {
//         try
//         {
//             var timerInterval = TimeSpan.FromSeconds(1);
//             var nextUpdateTime = DateTime.UtcNow + timerInterval;

//             while (_remainingTime > 0 && !token.IsCancellationRequested)
//             {
//                 if (_isPaused)
//                 {
//                     await Task.Delay(100, token);
//                     continue;
//                 }

//                 var currentTime = DateTime.UtcNow;
//                 if (currentTime < nextUpdateTime)
//                 {
//                     await Task.Delay(nextUpdateTime - currentTime, token);
//                     continue;
//                 }

//                 _remainingTime -= 1f;
//                 UpdateTimerDisplay();
//                 nextUpdateTime += timerInterval;

//                 if (_remainingTime <= 0)
//                 {
//                     HandleTimeExpired();
//                     break;
//                 }
//             }
//         }
//         catch (OperationCanceledException)
//         {
//             // Timer was stopped
//         }
//         catch (Exception ex)
//         {
//             Debug.LogError($"Timer error: {ex.Message}");
//         }
//         finally
//         {
//             _isRunning = false;
//             _isPaused = false;
//         }
//     }
//     private static async Task _RunTimerAsync(CancellationToken token)
//     {
//         try
//         {
//             while (_remainingTime > 0 && !token.IsCancellationRequested)
//             {
//                 if (!Application.isFocused)
//                 {
//                     // Oyun arka planda, bekle
//                     await Task.Delay(500, token);
//                     continue;
//                 }

//                 if (Time.timeScale == 0)
//                 {
//                     // Oyun duraklatılmış, bekle
//                     await Task.Delay(500, token);
//                     continue;
//                 }

//                 // Normal timer işlemi
//                 await Task.Delay(1000, token);
//                 _remainingTime -= 1f;
//                 await EventBus.PublishAuto(new UpdateTimerUIEvent(_remainingTime));
//             }

//             if (_remainingTime <= 0 && !token.IsCancellationRequested)
//             {
//                 await EventBus.PublishAuto(new GameFailEvent());
//             }
//         }
//         catch (OperationCanceledException) { }
//         finally
//         {
//             _isRunning = false;
//         }
//     }

//     private static void UpdateTimerDisplay()
//     {
//         EventBus.PublishAuto(new UpdateTimerUIEvent(_remainingTime));
//     }

//     private static async  void HandleTimeExpired()
//     {
//         await EventBus.PublishAuto(new GameFailEvent());
//     }

//     public static float RemainingTime => _remainingTime;
//     public static bool IsRunning => _isRunning;
//     public static bool IsPaused => _isPaused;
// }

public static class GameTimer
{
    private static float _remainingTime;
    private static bool _isRunning;
    private static bool _isPaused;
    private static CancellationTokenSource _cts;

    private static DateTime _lastLifeGivenTime;

    private const string LastLifeTimeKey = "LastLifeGivenTime";
    private static readonly TimeSpan LifeCooldown = TimeSpan.FromHours(24);

    // public static void Initialize()
    // {
    //     LoadLifeTimerData();
    // }

    public static void StartTimer(float duration)
    {
        if (duration <= 0)
        {
            Debug.LogWarning("Timer duration must be positive");
            return;
        }

        StopTimer();

        _remainingTime = duration;
        _isRunning = true;
        _isPaused = false;
        _cts = new CancellationTokenSource();

        _ = RunTimerAsync(_cts.Token);
    }

    public static void StopTimer()
    {
        if (!_isRunning) return;

        _cts?.Cancel();
        _cts?.Dispose();
        _cts = null;
        _isRunning = false;
        _isPaused = false;
    }

    public static void Pause()
    {
        if (!_isRunning || _isPaused) return;
        _isPaused = true;
    }

    public static void Resume()
    {
        if (!_isRunning || !_isPaused) return;
        _isPaused = false;
    }

    public static void AddTime(float seconds)
    {
        if (!_isRunning) return;

        _remainingTime = Mathf.Max(0, _remainingTime + seconds);
        UpdateTimerDisplay();
    }

    private static async Task RunTimerAsync(CancellationToken token)
    {
        try
        {
            var timerInterval = TimeSpan.FromSeconds(1);
            var nextUpdateTime = DateTime.UtcNow + timerInterval;

            while (_remainingTime > 0 && !token.IsCancellationRequested)
            {
                if (_isPaused)
                {
                    await Task.Delay(100, token);
                    continue;
                }

                var currentTime = DateTime.UtcNow;
                if (currentTime < nextUpdateTime)
                {
                    await Task.Delay(nextUpdateTime - currentTime, token);
                    continue;
                }

                _remainingTime -= 1f;
                UpdateTimerDisplay();
                nextUpdateTime += timerInterval;

                if (_remainingTime <= 0)
                {
                    HandleTimeExpired();
                    break;
                }
            }
        }
        catch (OperationCanceledException) { }
        catch (Exception ex)
        {
            Debug.LogError($"Timer error: {ex.Message}");
        }
        finally
        {
            _isRunning = false;
            _isPaused = false;
        }
    }

    private static async void UpdateTimerDisplay()
    {
        await EventBus.PublishAuto(new UpdateTimerUIEvent(_remainingTime));
    }

    private static async void HandleTimeExpired()
    {
        // await EventBus.PublishAuto(new GamePlayFailedEvent());
    }

    public static float RemainingTime => _remainingTime;
    public static bool IsRunning => _isRunning;
    public static bool IsPaused => _isPaused;

    // -------------------------------
    // LIFE TIMER (Gerçek Zaman Tabanlı)
    // -------------------------------

    public static void CheckLifeRegen()
    {
        DateTime now = DateTime.UtcNow;

        if (now - _lastLifeGivenTime >= LifeCooldown)
        {
            _lastLifeGivenTime = now;
            SaveLifeTimerData();

            // Life verildiğini event ile bildir
            // EventBus.PublishAuto(new LifeRegenEvent());

            Debug.Log("[Life Regen] Yeni life verildi.");
        }
        else
        {
            Debug.Log("[Life Regen] Henüz 24 saat geçmemiş.");
        }
    }

    public static TimeSpan TimeUntilNextLife()
    {
        var next = _lastLifeGivenTime + LifeCooldown;
        return next > DateTime.UtcNow ? next - DateTime.UtcNow : TimeSpan.Zero;
    }

    public static void SetLastLifeGivenTime(DateTime time)
    {
        _lastLifeGivenTime = time;
        SaveLifeTimerData();
    }

    private static void LoadLifeTimerData()
    {
        if (PlayerPrefs.HasKey(LastLifeTimeKey))
        {
            long binary = long.Parse(PlayerPrefs.GetString(LastLifeTimeKey));
            _lastLifeGivenTime = DateTime.FromBinary(binary);
        }
        else
        {
            _lastLifeGivenTime = DateTime.UtcNow - LifeCooldown; // ilk açılışta hemen life ver
        }
    }

    private static void SaveLifeTimerData()
    {
        PlayerPrefs.SetString(LastLifeTimeKey, _lastLifeGivenTime.ToBinary().ToString());
        PlayerPrefs.Save();
    }

}
