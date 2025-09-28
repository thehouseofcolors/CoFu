using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public static class EventBus
{
    private static readonly Dictionary<Type, List<Subscription>> _subscribers = new();
    private static readonly object _lock = new();

    public static IDisposable Subscribe<T>(Func<T, Task> handler) where T : IGameEvent
    {
        if (handler == null) throw new ArgumentNullException(nameof(handler));

        var subscription = new Subscription<T>(handler);

        lock (_lock)
        {
            if (!_subscribers.TryGetValue(typeof(T), out var subscriptions))
            {
                subscriptions = new List<Subscription>();
                _subscribers[typeof(T)] = subscriptions;
            }
            subscriptions.Add(subscription);
        }

        return new DisposableToken(() => Unsubscribe(subscription));
    }

    // Sync handler support by wrapping in a Task
    public static IDisposable Subscribe<T>(Action<T> handler) where T : IGameEvent
    {
        if (handler == null) throw new ArgumentNullException(nameof(handler));

        // Wrap the sync action in a Task-returning function
        Func<T, Task> asyncHandler = (e) =>
        {
            handler(e);
            return Task.CompletedTask;
        };

        return Subscribe(asyncHandler);
    }

    private static void Unsubscribe(Subscription subscription)
    {
        lock (_lock)
        {
            if (_subscribers.TryGetValue(subscription.EventType, out var subscriptions))
            {
                subscriptions.Remove(subscription);
                if (subscriptions.Count == 0)
                {
                    _subscribers.Remove(subscription.EventType);
                }
            }
        }
    }

    public static async Task PublishAuto<T>(T gameEvent) where T : IGameEvent
    {
        if (gameEvent == null) throw new ArgumentNullException(nameof(gameEvent));

        Subscription[] subscriptionsCopy;
        lock (_lock)
        {
            if (!_subscribers.TryGetValue(typeof(T), out var subscriptions))
                return;

            subscriptionsCopy = subscriptions.ToArray();
        }

        foreach (var subscription in subscriptionsCopy)
        {
            try
            {
                await subscription.Handle(gameEvent).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Debug.LogError($"[EventBus] handler error: {ex}");
            }
        }
    }

    public static void Clear()
    {
        lock (_lock)
        {
            _subscribers.Clear();
        }
    }

    #region Internal Types
    private abstract class Subscription
    {
        public abstract Type EventType { get; }
        public abstract Task Handle(IGameEvent gameEvent);
    }

    private class Subscription<T> : Subscription where T : IGameEvent
    {
        private readonly Func<T, Task> _handler;
        public override Type EventType => typeof(T);

        public Subscription(Func<T, Task> handler) => _handler = handler;

        public override Task Handle(IGameEvent gameEvent) => _handler((T)gameEvent);
    }

    private class DisposableToken : IDisposable
    {
        private readonly Action _onDispose;

        public DisposableToken(Action onDispose) => _onDispose = onDispose;
        public void Dispose() => _onDispose?.Invoke();
    }
    #endregion
}

// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Threading.Tasks;
// using UnityEngine;


// // Synchronous EventBus for UI and immediate operations
// public static class SyncEventBus
// {
//     private static readonly Dictionary<Type, List<ISubscription>> _subscribers = new();
//     private static readonly object _lock = new();

//     public static IDisposable Subscribe<T>(Action<T> handler) where T : IGameEvent
//     {
//         if (handler == null) throw new ArgumentNullException(nameof(handler));

//         var subscription = new Subscription<T>(handler);

//         lock (_lock)
//         {
//             if (!_subscribers.TryGetValue(typeof(T), out var subscriptions))
//             {
//                 subscriptions = new List<ISubscription>();
//                 _subscribers[typeof(T)] = subscriptions;
//             }
//             subscriptions.Add(subscription);
//         }

//         return new DisposableToken(() => Unsubscribe(subscription));
//     }

//     public static void Publish<T>(T gameEvent) where T : IGameEvent
//     {
//         if (gameEvent == null) throw new ArgumentNullException(nameof(gameEvent));

//         ISubscription[] subscriptionsCopy;
//         lock (_lock)
//         {
//             if (!_subscribers.TryGetValue(typeof(T), out var subscriptions))
//                 return;

//             subscriptionsCopy = subscriptions.ToArray();
//         }

//         foreach (var subscription in subscriptionsCopy)
//         {
//             try
//             {
//                 subscription.Handle(gameEvent);
//             }
//             catch (Exception ex)
//             {
//                 Debug.LogError($"[SyncEventBus] handler error: {ex}");
//             }
//         }
//     }

//     private static void Unsubscribe(ISubscription subscription)
//     {
//         lock (_lock)
//         {
//             if (_subscribers.TryGetValue(subscription.EventType, out var subscriptions))
//             {
//                 subscriptions.Remove(subscription);
//                 if (subscriptions.Count == 0)
//                 {
//                     _subscribers.Remove(subscription.EventType);
//                 }
//             }
//         }
//     }

//     public static void Clear()
//     {
//         lock (_lock)
//         {
//             _subscribers.Clear();
//         }
//     }

//     private interface ISubscription
//     {
//         Type EventType { get; }
//         void Handle(IGameEvent gameEvent);
//     }

//     private class Subscription<T> : ISubscription where T : IGameEvent
//     {
//         private readonly Action<T> _handler;
//         public Type EventType => typeof(T);

//         public Subscription(Action<T> handler) => _handler = handler;

//         public void Handle(IGameEvent gameEvent) => _handler((T)gameEvent);
//     }

//     private class DisposableToken : IDisposable
//     {
//         private readonly Action _onDispose;

//         public DisposableToken(Action onDispose) => _onDispose = onDispose;
//         public void Dispose() => _onDispose?.Invoke();
//     }
// }

// // Asynchronous EventBus for background operations
// public static class AsyncEventBus
// {
//     private static readonly Dictionary<Type, List<ISubscription>> _subscribers = new();
//     private static readonly object _lock = new();

//     public static IDisposable Subscribe<T>(Func<T, Task> handler) where T : IGameEvent
//     {
//         if (handler == null) throw new ArgumentNullException(nameof(handler));

//         var subscription = new Subscription<T>(handler);

//         lock (_lock)
//         {
//             if (!_subscribers.TryGetValue(typeof(T), out var subscriptions))
//             {
//                 subscriptions = new List<ISubscription>();
//                 _subscribers[typeof(T)] = subscriptions;
//             }
//             subscriptions.Add(subscription);
//         }

//         return new DisposableToken(() => Unsubscribe(subscription));
//     }

//     public static async Task Publish<T>(T gameEvent) where T : IGameEvent
//     {
//         if (gameEvent == null) throw new ArgumentNullException(nameof(gameEvent));

//         ISubscription[] subscriptionsCopy;
//         lock (_lock)
//         {
//             if (!_subscribers.TryGetValue(typeof(T), out var subscriptions))
//                 return;

//             subscriptionsCopy = subscriptions.ToArray();
//         }

//         var tasks = new List<Task>();
//         foreach (var subscription in subscriptionsCopy)
//         {
//             try
//             {
//                 tasks.Add(subscription.HandleAsync(gameEvent));
//             }
//             catch (Exception ex)
//             {
//                 Debug.LogError($"[AsyncEventBus] handler error: {ex}");
//             }
//         }

//         await Task.WhenAll(tasks);
//     }

//     public static void PublishFireAndForget<T>(T gameEvent) where T : IGameEvent
//     {
//         if (gameEvent == null) throw new ArgumentNullException(nameof(gameEvent));

//         ISubscription[] subscriptionsCopy;
//         lock (_lock)
//         {
//             if (!_subscribers.TryGetValue(typeof(T), out var subscriptions))
//                 return;

//             subscriptionsCopy = subscriptions.ToArray();
//         }

//         foreach (var subscription in subscriptionsCopy)
//         {
//             Task.Run(async () =>
//             {
//                 try
//                 {
//                     await subscription.HandleAsync(gameEvent);
//                 }
//                 catch (Exception ex)
//                 {
//                     Debug.LogError($"[AsyncEventBus] handler error: {ex}");
//                 }
//             });
//         }
//     }

//     private static void Unsubscribe(ISubscription subscription)
//     {
//         lock (_lock)
//         {
//             if (_subscribers.TryGetValue(subscription.EventType, out var subscriptions))
//             {
//                 subscriptions.Remove(subscription);
//                 if (subscriptions.Count == 0)
//                 {
//                     _subscribers.Remove(subscription.EventType);
//                 }
//             }
//         }
//     }

//     public static void Clear()
//     {
//         lock (_lock)
//         {
//             _subscribers.Clear();
//         }
//     }

//     private interface ISubscription
//     {
//         Type EventType { get; }
//         Task HandleAsync(IGameEvent gameEvent);
//     }

//     private class Subscription<T> : ISubscription where T : IGameEvent
//     {
//         private readonly Func<T, Task> _handler;
//         public Type EventType => typeof(T);

//         public Subscription(Func<T, Task> handler) => _handler = handler;

//         public Task HandleAsync(IGameEvent gameEvent) => _handler((T)gameEvent);
//     }

//     private class DisposableToken : IDisposable
//     {
//         private readonly Action _onDispose;

//         public DisposableToken(Action onDispose) => _onDispose = onDispose;
//         public void Dispose() => _onDispose?.Invoke();
//     }
// }

