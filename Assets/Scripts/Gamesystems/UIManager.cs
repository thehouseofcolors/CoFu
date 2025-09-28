/// <summary>
/// ✅ Geliştirme Durumu: TAMAMLANDI (Değişiklik planlanmıyor)
///
/// 📌 Sorumluluk:
/// - Oyundaki tüm ekran (Screen) ve overlay (Overlay) UI geçişlerini yönetir.
///
/// 🎧 Dinlediği Eventler:
/// - UIChangeEvent (EventBus aracılığıyla)
///
/// 📡 Yayınladığı Eventler:
/// - Yok
///
/// 🧩 Bağlı Olduğu Sistemler:
/// - EventBus
/// - ScreenConfig / OverlayConfig
/// - IGameSystem, IPausable, IQuittable
///
/// 📝 Notlar:
/// - Tüm UI prefab’ları sahnede tanımlanmıştır, runtime’da ekleme yapılmaz.
/// - UIManager dışarıdan destroy edilmemelidir. Shutdown() temizliği yapar.
/// 
/// 
/// ✅ Geliştirme Durumu: TAMAMLANDI (Değişiklik planlanmıyor)
/// </summary>

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using GameEvents;

public class UIManager : MonoBehaviour, IGameSystem, IPausable, IQuittable
{
    [Header("Screen Configuration")]
    [SerializeField] private ScreenConfig[] screenConfigs;

    [Header("Overlay Configuration")]
    [SerializeField] private OverlayConfig[] overlayConfigs;

    [Header("Mobile Settings")]
    
    private ScreenType resumeScreen = ScreenType.Pause;
    private bool handleMobilePause = true;

    private Dictionary<ScreenType, ScreenConfig> _screens;
    private Dictionary<OverlayType, OverlayConfig> _overlays;
    private ScreenConfig _currentScreen;
    private OverlayConfig _currentOverlay;
    private List<IDisposable> _uiEventSubscription = new List<IDisposable>();
    private bool _isTransitioning;
    private ScreenType? _lastScreenBeforePause;

    #region IGameSystem-IPausable-IQuittable Implementation

    public async Task Initialize()
    {
        try
        {
            _screens = InitializePanelDictionary<ScreenType, ScreenConfig>(screenConfigs);
            _overlays = InitializePanelDictionary<OverlayType, OverlayConfig>(overlayConfigs);

            _uiEventSubscription.Add(EventBus.Subscribe<UIChangeAsyncEvent>(OnUIChanged));

            await SafeChangeScreen(ScreenType.Loading);
            await SafeChangeScreen(ScreenType.Menu);
        }
        catch (Exception ex)
        {
            Debug.LogError($"UI Manager initialization failed: {ex}");
            throw;
        }
    }

    public async Task Shutdown()
    {
        try
        {
            foreach (var sub in _uiEventSubscription)
            {
                sub?.Dispose();
                
            }
            _uiEventSubscription.Clear();
            // await CleanupCurrentUI();
            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            Debug.LogError($"UI Manager shutdown error: {ex}");
            throw;
        }
    }

    public async void OnPause()
    {
        if (!handleMobilePause) return;

        _lastScreenBeforePause = _currentScreen?.panelType;

        if (_currentScreen == null || _currentScreen.panelType != resumeScreen)
        {
            await SafeChangeScreen(ScreenType.Pause);
        }
    }

    public async void OnResume()
    {
        if (!handleMobilePause) return;

        if (_currentScreen?.panelType == resumeScreen)
        {
            await ReturnFromPause();
        }
    }

    public void OnQuit()
    {

    }

    #endregion



    #region Core Functionality

    private Dictionary<TPanelType, TConfig> InitializePanelDictionary<TPanelType, TConfig>(
    TConfig[] configs
    ) where TConfig : PanelConfigBase<TPanelType>
    {
        var dict = new Dictionary<TPanelType, TConfig>();

        foreach (var config in configs)
        {
            if (config.TryInitialize())
            {
                dict[config.panelType] = config;
                
                config.gameObject.SetActive(false);
            }
        }

        return dict;
    }

    private async Task OnUIChanged(UIChangeAsyncEvent e)
    {
        if (_isTransitioning) return;

        _isTransitioning = true;
        try
        {
            if (e.IsOverlay)
            {
                await SafeChangeOverlay(e.OverlayType, e.TransitionData);
            }
            else
            {
                await SafeChangeScreen(e.ScreenType, e.TransitionData);
            }
            Debug.Log("event listed");
        }
        catch (Exception ex)
        {
            Debug.LogError($"UI transition failed: {ex}");
        }
        finally
        {
            _isTransitioning = false;
        }
    }
    private async Task SafeChangeScreen(ScreenType screenType, object transitionData = null)
    {
        if (!_screens.TryGetValue(screenType, out var newScreen))
        {
            Debug.LogWarning($"Screen {screenType} not found");
            return;
        }

        if (_currentScreen?.panelType == screenType) return;

        if (_currentScreen == null)
        {
            // Önce panel görünür yapılır, sonra güncellenir
            await newScreen.panel.ShowAsync(transitionData);
        }
        else
        {
            // Aynı anda hem eski panel gizlenir, hem yeni panel gösterilir
            await TransitionBetweenPanels(_currentScreen.panel, newScreen.panel);
        }

        _currentScreen = newScreen;
    }
    private async Task SafeChangeOverlay(OverlayType overlayType, object transitionData = null)
    {
        if (_currentOverlay != null)
        {
            try
            {
                await _currentOverlay.panel.HideAsync(transitionData);
                _currentOverlay.gameObject.SetActive(false);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to hide panel {_currentOverlay.gameObject.name}: {ex}");
                _currentOverlay.gameObject.SetActive(false);
                throw;
            }
        }
        if (overlayType == OverlayType.None) return;

        if (!_overlays.TryGetValue(overlayType, out var newOverlay))
        {
            Debug.LogWarning($"Overlay {overlayType} not found");
            return;
        }

        if (_currentOverlay?.panelType == overlayType) return;

        // Show new overlay
        _currentOverlay = newOverlay;
        try
        {
            _currentOverlay.gameObject.SetActive(true);
            await _currentOverlay.panel.ShowAsync(transitionData);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to show panel {_currentOverlay.gameObject.name}: {ex}");
            _currentOverlay.gameObject.SetActive(false);
            throw;
        }
    }
    private async Task TransitionBetweenPanels(IPanel from, IPanel to)
    {
        var hideTask = from.HideAsync();
        var showTask = to.ShowAsync();

        await Task.WhenAll(hideTask, showTask);
    }
    private async Task ReturnFromPause()
    {
        await SafeChangeOverlay(OverlayType.None);

        var targetScreen = _lastScreenBeforePause ?? resumeScreen;
        if (_screens.ContainsKey(targetScreen))
        {
            await SafeChangeScreen(targetScreen);
        }

        _lastScreenBeforePause = null;
    }

    #endregion
   



}


