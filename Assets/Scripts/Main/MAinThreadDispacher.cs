// using UnityEngine;
// using System.Collections.Generic;
// using System;
// using System.Threading.Tasks;
// using System.Threading;

// public class MainThreadDispatcher : MonoBehaviour
// {
//     private static readonly Queue<Action> _executionQueue = new Queue<Action>();
//     private static MainThreadDispatcher _instance;

//     public static MainThreadDispatcher Instance
//     {
//         get
//         {
//             if (_instance == null)
//             {
//                 Initialize();
//             }
//             return _instance;
//         }
//     }

//     [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
//     private static void Initialize()
//     {
//         if (_instance == null)
//         {
//             var obj = new GameObject("MainThreadDispatcher");
//             _instance = obj.AddComponent<MainThreadDispatcher>();
//             DontDestroyOnLoad(obj);
//         }
//     }

//     public void Update()
//     {
//         lock (_executionQueue)
//         {
//             while (_executionQueue.Count > 0)
//             {
//                 _executionQueue.Dequeue().Invoke();
//             }
//         }
//     }

//     public void Enqueue(Action action)
//     {
//         lock (_executionQueue)
//         {
//             _executionQueue.Enqueue(action);
//         }
//     }

//     // Async uyumlu versiyon
//     public Task EnqueueAsync(Action action, CancellationToken cancellationToken = default)
//     {
//         if (cancellationToken.IsCancellationRequested)
//             return Task.FromCanceled(cancellationToken);

//         var tcs = new TaskCompletionSource<bool>();

//         Enqueue(() =>
//         {
//             if (cancellationToken.IsCancellationRequested)
//             {
//                 tcs.SetCanceled();
//                 return;
//             }

//             try
//             {
//                 action();
//                 tcs.SetResult(true);
//             }
//             catch (Exception ex)
//             {
//                 tcs.SetException(ex);
//             }
//         });

//         return tcs.Task;
//     }
// }
// public class UnityMainThreadDispatcher : MonoBehaviour
// {
//     private static UnityMainThreadDispatcher _instance;
//     private readonly Queue<Action> _actions = new Queue<Action>();
//     private readonly Queue<TaskCompletionSource<bool>> _taskCompletions = new Queue<TaskCompletionSource<bool>>();

//     public static UnityMainThreadDispatcher Instance
//     {
//         get
//         {
//             if (_instance == null)
//             {
//                 _instance = FindObjectOfType<UnityMainThreadDispatcher>();
//                 if (_instance == null)
//                 {
//                     var go = new GameObject(nameof(UnityMainThreadDispatcher));
//                     _instance = go.AddComponent<UnityMainThreadDispatcher>();
//                     DontDestroyOnLoad(go);
//                 }
//             }
//             return _instance;
//         }
//     }

//     public Task EnqueueAsync(Action action)
//     {
//         var tcs = new TaskCompletionSource<bool>();

//         lock (_actions)
//         {
//             _actions.Enqueue(() =>
//             {
//                 try
//                 {
//                     action();
//                     tcs.SetResult(true);
//                 }
//                 catch (Exception ex)
//                 {
//                     tcs.SetException(ex);
//                 }
//             });
//             _taskCompletions.Enqueue(tcs);
//         }

//         return tcs.Task;
//     }

//     private void Update()
//     {
//         lock (_actions)
//         {
//             while (_actions.Count > 0 && _taskCompletions.Count > 0)
//             {
//                 var action = _actions.Dequeue();
//                 var tcs = _taskCompletions.Dequeue();
//                 action.Invoke();
//             }
//         }
//     }
// }

using UnityEngine;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;

[DefaultExecutionOrder(-1000)] // Ensure early execution
public class MainThreadDispatcher : MonoBehaviour
{
    private static MainThreadDispatcher _instance;
    private static readonly Queue<(Action action, TaskCompletionSource<bool> tcs)> _executionQueue = 
        new Queue<(Action, TaskCompletionSource<bool>)>();
    private static readonly object _lock = new object();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        if (_instance == null && !ApplicationQuitDetector.IsQuitting)
        {
            var obj = new GameObject(nameof(MainThreadDispatcher));
            _instance = obj.AddComponent<MainThreadDispatcher>();
            DontDestroyOnLoad(obj);
        }
    }

    public static MainThreadDispatcher Instance
    {
        get
        {
            if (_instance == null && !ApplicationQuitDetector.IsQuitting)
            {
                Initialize();
            }
            return _instance;
        }
    }

    // Basic synchronous enqueue
    public static void Enqueue(Action action)
    {
        if (action == null) throw new ArgumentNullException(nameof(action));
        
        if (Instance == null) 
        {
            action();
            return;
        }

        lock (_lock)
        {
            _executionQueue.Enqueue((action, null));
        }
    }

    // Async version with cancellation support
    public static Task EnqueueAsync(Action action, CancellationToken cancellationToken = default)
    {
        if (action == null) throw new ArgumentNullException(nameof(action));

        if (cancellationToken.IsCancellationRequested)
            return Task.FromCanceled(cancellationToken);

        if (Instance == null)
        {
            try
            {
                action();
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                return Task.FromException(ex);
            }
        }

        var tcs = new TaskCompletionSource<bool>();

        lock (_lock)
        {
            _executionQueue.Enqueue((() =>
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    tcs.SetCanceled();
                    return;
                }

                try
                {
                    action();
                    tcs.SetResult(true);
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }
            }, tcs));
        }

        return tcs.Task;
    }

    private void Update()
    {
        lock (_lock)
        {
            while (_executionQueue.Count > 0)
            {
                var (action, tcs) = _executionQueue.Dequeue();
                try
                {
                    action?.Invoke();
                }
                catch (Exception ex)
                {
                    tcs?.SetException(ex);
                    Debug.LogError($"MainThreadDispatcher action failed: {ex}");
                }
            }
        }
    }

    private void OnDestroy()
    {
        if (_instance == this)
        {
            lock (_lock)
            {
                while (_executionQueue.Count > 0)
                {
                    var (_, tcs) = _executionQueue.Dequeue();
                    tcs?.SetCanceled();
                }
            }
            _instance = null;
        }
    }
}

// Helper to detect application quit
public static class ApplicationQuitDetector
{
    public static bool IsQuitting { get; private set; }

    [RuntimeInitializeOnLoadMethod]
    private static void Initialize()
    {
        Application.quitting += () => IsQuitting = true;
    }
}

