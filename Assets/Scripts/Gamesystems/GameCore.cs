using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(IGameSystem))]
public class GameCore : MonoBehaviour
{
    private HashSet<IGameSystem> _systems;

    private void Start()
    {
        _systems = new HashSet<IGameSystem>(GetComponents<IGameSystem>());
        foreach (var system in _systems)
        {
            system.Initialize();
        }
    }

    public void RegisterSystem(IGameSystem system)
    {
        if (system == null || !_systems.Add(system)) return;
        system.Initialize();
    }

    public void UnregisterSystem(IGameSystem system)
    {
        if (system == null || !_systems.Remove(system)) return;
        system.Shutdown();
    }

    private void OnDestroy()
    {
        foreach (var system in _systems)
        {
            system?.Shutdown();
        }
        _systems.Clear();
    }

    public T GetSystem<T>() where T : class, IGameSystem
    {
        foreach (var system in _systems)
        {
            if (system is T typedSystem) return typedSystem;
        }
        return null;
    }

    public bool TryGetSystem<T>(out T system) where T : class, IGameSystem
    {
        foreach (var s in _systems)
        {
            if (s is T typedSystem)
            {
                system = typedSystem;
                return true;
            }
        }
        system = null;
        return false;
    }
}