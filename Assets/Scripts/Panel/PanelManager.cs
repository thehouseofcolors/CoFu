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
    [SerializeField] private GameObject fail;

    [SerializeField] private GameObject pause;

    [Header("Settings")]
    [SerializeField] private float minLoadingTime = 2f; // Minimum loading screen duration
    [SerializeField] private float panelTransitionDelay = 3f;
    private GameObject _currentPanel;
    private IDisposable _eventSubscription;
    private Dictionary<ScreenType, GameObject> _panelDictionary;
    private bool _isTransitioning;


    public async void Initialize()
    {

        CreatePanelDictionary();

        _eventSubscription = EventBus.Subscribe<ScreenChangeEvent>(OnScreenChanged);

        // Start with loading screen
        await ShowInitialLoading();
    }
    private async Task ShowInitialLoading()
    {
        Debug.Log("ShowInitialLoading start");

        _currentPanel = loadingPanel;   // Burada direkt current paneli loading yapıyoruz
        loadingPanel.SetActive(true);   // Ve loading panelini aktif ediyoruz

        var loadingPanelInterface = loadingPanel.GetComponent<IPanel>();
        if (loadingPanelInterface != null)
        {
            await loadingPanelInterface.ShowAsync(null);  // Animasyon varsa çalıştır
        }

        // Debug.Log("Initial loading panel shown");

        // await Task.Delay((int)(minLoadingTime * 1000));

        // Debug.Log("Going to menu");

        await GameStateMachine.SetInitialStateAsync(new MenuState());
    }


    public void Shutdown()
    {
        _eventSubscription?.Dispose();
        _eventSubscription = null;
    }

    private void CreatePanelDictionary()
    {
        _panelDictionary = new Dictionary<ScreenType, GameObject>
        {
            {ScreenType.Loading, loadingPanel},
            {ScreenType.Menu, menuPanel},
            {ScreenType.Game, gamePanel},
            {ScreenType.Win, winPanel},
            {ScreenType.Fail, fail},
            { ScreenType.Pause_NoMoves, pause},
            {ScreenType.Pause_TimeOver, pause}
        };

        // Initialize all panels
        foreach (var panel in _panelDictionary.Values)
        {
            if (panel != null)
            {
                panel.SetActive(false);

                // Ensure each panel has an IPanel component
                var panelInterface = panel.GetComponent<IPanel>();
                if (panelInterface == null)
                {
                    Debug.LogWarning($"[PanelManager] Panel {panel.name} doesn't implement IPanel interface");
                }
            }
        }
    }

    private async Task OnScreenChanged(ScreenChangeEvent e)
    {
        if (_isTransitioning) return;

        _isTransitioning = true;
        try
        {
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

        Debug.Log($"[PanelManager] ShowPanelAsync called for {screenType}");
        if (!_panelDictionary.TryGetValue(screenType, out var newPanel))
        {
            Debug.LogError($"[PanelManager] No panel registered for {screenType}");
            return;
        }

        if (newPanel == null)
        {
            Debug.LogError($"[PanelManager] Panel reference for {screenType} is null");
            return;
        }

        // Skip if already showing this panel
        if (newPanel == _currentPanel)
        {
            Debug.Log($"[PanelManager] {screenType} already active. Forcing ShowAsync again.");

            var currentPanelInterface = _currentPanel.GetComponent<IPanel>();
            if (currentPanelInterface != null)
            {
                await currentPanelInterface.ShowAsync(transitionData); // Yeniden başlat
            }

            await EventBus.PublishAuto(new PanelShownEvent(screenType, transitionData));
            return;
        }


        // Hide current panel
        if (_currentPanel != null)
        {
            var currentPanelInterface = _currentPanel.GetComponent<IPanel>();
            if (currentPanelInterface != null)
            {
                await currentPanelInterface.HideAsync();
            }
            else
            {
                await DefaultHideAsync(_currentPanel);
            }
        }

        // Show new panel
        var newPanelInterface = newPanel.GetComponent<IPanel>();
        if (newPanelInterface != null)
        {
            
            await newPanelInterface.ShowAsync(transitionData);
        }
        else
        {
            await DefaultShowAsync(newPanel, transitionData);
        }

        _currentPanel = newPanel;
        await EventBus.PublishAuto(new PanelShownEvent(screenType, transitionData));
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


