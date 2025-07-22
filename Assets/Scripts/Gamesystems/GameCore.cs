
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;


[DefaultExecutionOrder(-1000)] // Ensures earliest possible execution
public class GameCore : MonoBehaviour
{
    private readonly HashSet<IGameSystem> _systems = new();
    private bool _isShuttingDown;
    private bool _isPaused;

    [Header("Mobile Settings")]
    [Tooltip("Whether to automatically handle pause/resume on mobile devices")]
    [SerializeField] private bool _handleMobilePause = true;
    private bool _wasPausedBySystem;

    #region Lifecycle Management

    private async void Awake()
    {
        DontDestroyOnLoad(gameObject);
        await InitializeCoreSystems();
    }

    private async Task InitializeCoreSystems()
    {
        if (_handleMobilePause && Application.isMobilePlatform)
        {
            SetupMobilePauseHandling();
        }

        // Initialize all systems in order of their execution order
        var orderedSystems = GetComponents<IGameSystem>();

        foreach (var system in orderedSystems)
        {
            await RegisterSystemAsync(system);
        }
    }

    // private async void OnDestroy()
    // {
    //     await ShutdownAllSystems();
    // }

    // private async Task ShutdownAllSystems()
    // {
    //     if (_isShuttingDown) return;
    //     _isShuttingDown = true;

    //     CleanupMobilePauseHandling();

    //     // Create shutdown tasks for all systems
    //     var shutdownTasks = _systems.Select(SafeShutdownSystem).ToList();

    //     try
    //     {
    //         await Task.WhenAll(shutdownTasks);
    //     }
    //     catch (Exception ex)
    //     {
    //         Debug.LogError($"Core shutdown error: {ex}");
    //     }
    //     finally
    //     {
    //         _systems.Clear();
    //     }
    // }

    private async void OnDestroy()
    {
        if (_isShuttingDown) return;
        _isShuttingDown = true;

        CleanupMobilePauseHandling();

        // Create a copy of the systems list to avoid modification during iteration
        var systemsToShutdown = _systems.ToList(); // This is where we fix the enumeration issue

        var shutdownTasks = new List<Task>();
        foreach (var system in systemsToShutdown)
        {
            shutdownTasks.Add(SafeShutdownSystem(system));
        }

        try
        {
            await Task.WhenAll(shutdownTasks);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Core shutdown error: {ex}");
        }
        finally
        {
            _systems.Clear(); // Now safe to clear since we're done with the copy
        }
    }



    #endregion

    #region System Registration

    public async Task RegisterSystemAsync(IGameSystem system)
    {
        if (ShouldSkipRegistration(system)) return;

        _systems.Add(system);
        try
        {
            await system.Initialize();
            
            // If we're paused, notify the new system immediately
            if (_isPaused && system is IPausable pausable)
            {
                pausable.OnPause();
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"System init failed: {system?.GetType().Name}\n{ex}");
            _systems.Remove(system);
            throw;
        }
    }

    private bool ShouldSkipRegistration(IGameSystem system)
    {
        return system == null || 
               _systems.Contains(system) || 
               _isShuttingDown || 
               (system is MonoBehaviour mono && mono == null);
    }

    public async Task UnregisterSystemAsync(IGameSystem system)
    {
        if (ShouldSkipUnregistration(system)) return;

        try
        {
            await system.Shutdown();
            _systems.Remove(system);
        }
        catch (Exception ex)
        {
            Debug.LogError($"System shutdown failed: {system?.GetType().Name}\n{ex}");
        }
    }

    private bool ShouldSkipUnregistration(IGameSystem system)
    {
        return system == null || 
               !_systems.Contains(system) || 
               (system is MonoBehaviour mono && mono == null);
    }

    #endregion

    #region Mobile Pause Handling

    private void SetupMobilePauseHandling()
    {
        Application.focusChanged += OnApplicationFocusChanged;
        Application.onBeforeRender += OnBeforeRenderPauseCheck;
    }

    private void CleanupMobilePauseHandling()
    {
        Application.focusChanged -= OnApplicationFocusChanged;
        Application.onBeforeRender -= OnBeforeRenderPauseCheck;
    }

    private void OnApplicationFocusChanged(bool hasFocus)
    {
        if (!_handleMobilePause) return;

        if (!hasFocus)
        {
            SetPausedState(true);
            _wasPausedBySystem = true;
        }
        else if (_wasPausedBySystem)
        {
            SetPausedState(false);
            _wasPausedBySystem = false;
        }
    }

    private void OnBeforeRenderPauseCheck()
    {
        if (!_handleMobilePause || !Application.isMobilePlatform) return;

        // Secondary check for cases where focusChanged might have been missed
        if (!Application.isFocused && !_wasPausedBySystem)
        {
            SetPausedState(true);
            _wasPausedBySystem = true;
        }
    }

    #endregion

    #region Pause/Quit Handling

    private void OnApplicationPause(bool pauseStatus)
    {
        SetPausedState(pauseStatus);
    }

    private void SetPausedState(bool pauseStatus)
    {
        if (_isPaused == pauseStatus) return;
        _isPaused = pauseStatus;

        foreach (var system in _systems)
        {
            if (system is IPausable pausable)
            {
                try
                {
                    if (pauseStatus) pausable.OnPause();
                    else pausable.OnResume();
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Pause error in {system?.GetType().Name}: {ex}");
                }
            }
        }
    }

    private void OnApplicationQuit()
    {
        foreach (var system in _systems.OfType<IQuittable>())
        {
            try
            {
                system.OnQuit();
            }
            catch (Exception ex)
            {
                Debug.LogError($"Quit error in {system?.GetType().Name}: {ex}");
            }
        }
    }

    #endregion

    #region Utility Methods

    private async Task SafeShutdownSystem(IGameSystem system)
    {
        try
        {
            // Skip already destroyed Unity objects
            if (system is MonoBehaviour mono && mono == null)
                return;

            await UnregisterSystemAsync(system);
        }
        catch (MissingReferenceException)
        {
            // Silently handle already destroyed objects
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error shutting down system: {ex}");
        }
    }
    public T GetSystem<T>() where T : class, IGameSystem =>
        _systems.FirstOrDefault(s => s is T) as T;

    public bool TryGetSystem<T>(out T system) where T : class, IGameSystem
    {
        system = GetSystem<T>();
        return system != null;
    }

    #endregion
}

// [AttributeUsage(AttributeTargets.Class)]
// public class ExecutionOrderAttribute : Attribute
// {
//     public int order;
//     public ExecutionOrderAttribute(int order) => this.order = order;
// }




// using UnityEngine;
// using System.Collections.Generic;
// using System.Threading.Tasks;

// [RequireComponent(typeof(IGameSystem))]
// public class GameCore : MonoBehaviour
// {
//     private HashSet<IGameSystem> _systems = new HashSet<IGameSystem>();
//     private bool _isShuttingDown;

//     private async void Start()
//     {
//         // Initialize all attached systems
//         var systems = GetComponents<IGameSystem>();
//         foreach (var system in systems)
//         {
//             await RegisterSystemAsync(system);
//         }
//     }

//     public async Task RegisterSystemAsync(IGameSystem system)
//     {
//         if (system == null || _systems.Contains(system) || _isShuttingDown)
//             return;

//         _systems.Add(system);
//         try
//         {
//             await system.Initialize();
//         }
//         catch (System.Exception ex)
//         {
//             Debug.LogError($"Failed to initialize system {system.GetType().Name}: {ex}");
//             _systems.Remove(system);
//         }
//     }

//     public async Task UnregisterSystemAsync(IGameSystem system)
//     {
//         if (system == null || !_systems.Contains(system) || _isShuttingDown)
//             return;

//         _systems.Remove(system);
//         try
//         {
//             await system.Shutdown();
//         }
//         catch (System.Exception ex)
//         {
//             Debug.LogError($"Failed to shutdown system {system.GetType().Name}: {ex}");
//         }
//     }

//     private async void OnDestroy()
//     {
//         _isShuttingDown = true;

//         var shutdownTasks = new List<Task>();
//         foreach (var system in _systems)
//         {
//             if (system != null)
//             {
//                 shutdownTasks.Add(system.Shutdown());
//             }
//         }

//         try
//         {
//             await Task.WhenAll(shutdownTasks);
//         }
//         catch (System.Exception ex)
//         {
//             Debug.LogError($"Error during system shutdown: {ex}");
//         }

//         _systems.Clear();
//     }

//     public T GetSystem<T>() where T : class, IGameSystem
//     {
//         foreach (var system in _systems)
//         {
//             if (system is T typedSystem)
//                 return typedSystem;
//         }
//         return null;
//     }

//     public bool TryGetSystem<T>(out T system) where T : class, IGameSystem
//     {
//         system = GetSystem<T>();
//         return system != null;
//     }

//     private void OnApplicationPause(bool pauseStatus)
//     {
        
        
//     }
//     private void OApplicationQuit()
//     {
        
//     }

// }

