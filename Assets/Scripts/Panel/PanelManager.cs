using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using GameEvents;

public class PanelManager : MonoBehaviour, IGameSystem
{
    [Header("Panel References")]
    [SerializeField] private GameObject loadingPanel;
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private GameObject gamePanel;
    [SerializeField] private GameObject winPanel;
    [SerializeField] private GameObject failPanel;
    [SerializeField] private GameObject pausePanel;

    [Header("Settings")]
    [SerializeField] private float minLoadingTime = 2f;
    [SerializeField] private float panelTransitionDelay = 0.3f;

    private GameObject _currentPanel;
    private GameObject _currentLayOver;
    private readonly List<IDisposable> _eventSubscriptions = new List<IDisposable>();
    private Dictionary<ScreenType, GameObject> _panelDictionary;
    private Dictionary<LayOverType, GameObject> _layOverDictionary;
    private bool _isTransitioning;

    public async void Initialize()
    {
        CreatePanelDictionary();
        CreateLayOverDictionary();

        _eventSubscriptions.Add(EventBus.Subscribe<ScreenChangeEvent>(OnScreenChanged));
        _eventSubscriptions.Add(EventBus.Subscribe<LayOverChangeEvent>(OnLayOverChanged));

        await ShowInitialLoading();
    }

    private async Task ShowInitialLoading()
    {
        if (this == null) return;

        Debug.Log("[PanelManager] Showing initial loading screen");

        _currentPanel = loadingPanel;
        loadingPanel.SetActive(true);

        try
        {
            var loadingPanelInterface = loadingPanel.GetComponent<IPanel>();
            if (loadingPanelInterface != null)
            {
                await loadingPanelInterface.ShowAsync(null);
            }

            // Enforce minimum loading time if needed
            // float elapsed = 0f;
            // while (elapsed < minLoadingTime)
            // {
            //     elapsed += Time.deltaTime;
            //     await Task.Yield();
            // }

            await GameStateMachine.SetInitialStateAsync(new MenuState());
        }
        catch (Exception e)
        {
            Debug.LogError($"[PanelManager] Error during initial loading: {e}");
        }
    }

    public void Shutdown()
    {
        foreach (var subscription in _eventSubscriptions)
        {
            subscription?.Dispose();
        }
        _eventSubscriptions.Clear();
    }

    private void CreatePanelDictionary()
    {
        _panelDictionary = new Dictionary<ScreenType, GameObject>
        {
            {ScreenType.Loading, loadingPanel},
            {ScreenType.Menu, menuPanel},
            {ScreenType.Game, gamePanel}
            
        };

        InitializePanels(_panelDictionary, typeof(IPanel));
    }

    private void CreateLayOverDictionary()
    {
        _layOverDictionary = new Dictionary<LayOverType, GameObject>
        {
            {LayOverType.Win, winPanel},
            // {LayOverType.Fail, failPanel},
            {LayOverType.Pause_NoMoves, pausePanel},
            {LayOverType.Pause_TimeOver, pausePanel}
        };

        InitializePanels(_layOverDictionary, typeof(ILayOver));
    }
    private void InitializePanels<T>(Dictionary<T, GameObject> panelDict, Type interfaceType) where T : Enum
    {
        foreach (var kvp in panelDict)
        {
            if (kvp.Value == null)
            {
                Debug.LogError($"[PanelManager] Panel reference for {kvp.Key} is null");
                continue;
            }

            kvp.Value.SetActive(false);

            if (interfaceType == typeof(IPanel))
            {
                if (kvp.Value.GetComponent<IPanel>() == null)
                {
                    Debug.LogWarning($"[PanelManager] Panel {kvp.Value.name} doesn't implement IPanel interface");
                }
            }
            else if (interfaceType == typeof(ILayOver))
            {
                if (kvp.Value.GetComponent<ILayOver>() == null)
                {
                    Debug.LogWarning($"[PanelManager] Panel {kvp.Value.name} doesn't implement ILayOver interface");
                }
            }
        }
    }

    private async Task OnScreenChanged(ScreenChangeEvent e)
    {
        if (_isTransitioning || this == null) return;

        try
        {
            _isTransitioning = true;
            await ShowPanelAsync(e.Screen, e.TransitionData);
        }
        catch (Exception ex)
        {
            Debug.LogError($"[PanelManager] Error during screen transition: {ex}");
        }
        finally
        {
            _isTransitioning = false;
        }
    }

    public async Task ShowPanelAsync(ScreenType screenType, object transitionData = null)
    {
        if (this == null) return;

        Debug.Log($"[PanelManager] Transitioning to {screenType}");

        if (!_panelDictionary.TryGetValue(screenType, out var newPanel) || newPanel == null)
        {
            Debug.LogError($"[PanelManager] Invalid panel for {screenType}");
            return;
        }

        // Skip if already showing this panel
        if (newPanel == _currentPanel)
        {
            Debug.Log($"[PanelManager] {screenType} already active. Refreshing...");
            await RefreshCurrentPanel(transitionData, screenType);
            return;
        }

        // Hide current panel if different
        if (_currentPanel != null)
        {
            await HidePanelAsync(_currentPanel);
        }

        // Show new panel
        await ShowPanelInternalAsync(newPanel, transitionData);
        _currentPanel = newPanel;

    }

    private async Task RefreshCurrentPanel(object transitionData, ScreenType screenType)
    {
        var currentPanelInterface = _currentPanel.GetComponent<IPanel>();
        if (currentPanelInterface != null)
        {
            await currentPanelInterface.ShowAsync(transitionData);
        }
    }

    private async Task OnLayOverChanged(LayOverChangeEvent e)
    {
        try
        {
            await ShowLayOverAsync(e.Screen, e.TransitionData);
        }
        catch (Exception ex)
        {
            Debug.LogError($"[PanelManager] Error during overlay transition: {ex}");
        }
    }

    public async Task ShowLayOverAsync(LayOverType layOverType, object transitionData = null)
    {
        if (this == null) return;

        // Handle null overlay case (hiding current overlay)
        if (layOverType == LayOverType.None || !_layOverDictionary.TryGetValue(layOverType, out var newLayover))
        {
            Debug.Log($"[PanelManager] Hiding current overlay");
            if (_currentLayOver != null)
            {
                await HideLayOverAsync(_currentLayOver);
                _currentLayOver = null;
            }
            return;
        }

        if (newLayover == null)
        {
            Debug.LogError($"[PanelManager] Overlay reference for {layOverType} is null");
            return;
        }

        // Skip if already showing this overlay
        if (newLayover == _currentLayOver)
        {
            Debug.Log($"[PanelManager] {layOverType} already active. Refreshing...");
            await RefreshCurrentLayOver(transitionData);
            return;
        }

        // Hide current overlay if exists
        if (_currentLayOver != null)
        {
            await HideLayOverAsync(_currentLayOver);
        }

        // Show new overlay
        await ShowLayOverInternalAsync(newLayover, transitionData);
        _currentLayOver = newLayover;
    }
    private async Task RefreshCurrentLayOver(object transitionData)
    {
        var currentLayOverInterface = _currentLayOver.GetComponent<ILayOver>();
        if (currentLayOverInterface != null)
        {
            await currentLayOverInterface.ShowLayOverAsync(null);
        }
    }

    private async Task HidePanelAsync(GameObject panel)
    {
        var panelInterface = panel.GetComponent<IPanel>();
        if (panelInterface != null)
        {
            await panelInterface.HideAsync();
        }
        else
        {
            await DefaultHideAsync(panel);
        }
    }

    private async Task HideLayOverAsync(GameObject layOver)
    {
        var layOverInterface = layOver.GetComponent<ILayOver>();
        if (layOverInterface != null)
        {
            await layOverInterface.HideCurrentLayOverAsync();
        }
        else
        {
            await DefaultHideAsync(layOver);
        }
    }

    private async Task ShowPanelInternalAsync(GameObject panel, object transitionData)
    {
        panel.SetActive(true);
        var panelInterface = panel.GetComponent<IPanel>();
        if (panelInterface != null)
        {
            await panelInterface.ShowAsync(transitionData);
        }
        else
        {
            await DefaultShowAsync(panel, transitionData);
        }
    }

    private async Task ShowLayOverInternalAsync(GameObject layOver, object transitionData)
    {
        layOver.SetActive(true);
        var layOverInterface = layOver.GetComponent<ILayOver>();
        if (layOverInterface != null)
        {
            await layOverInterface.ShowLayOverAsync(null);
        }
        else
        {
            await DefaultShowAsync(layOver, transitionData);
        }
    }



    private async Task DefaultShowAsync(GameObject panel, object transitionData)
    {
        panel.SetActive(true);
        if (panelTransitionDelay > 0)
        {
            await Task.Delay((int)(panelTransitionDelay * 1000));
        }
    }

    private async Task DefaultHideAsync(GameObject panel)
    {
        panel.SetActive(false);
        await Task.CompletedTask;
    }

}

