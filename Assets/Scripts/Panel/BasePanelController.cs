
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Threading;
using System.Collections.Generic;
using DG.Tweening;
using GameEvents;

[RequireComponent(typeof(CanvasGroup))]
public abstract class BasePanelController : MonoBehaviour, IPanel
{
    [Header("UI References")]
    [SerializeField] protected RectTransform contentRoot;
    [SerializeField] protected CanvasGroup canvasGroup;
    [SerializeField] protected PanelType panelType;
    [SerializeField] protected AudioEntry panelAudio;

    #region Protected Fields
    protected Vector2 originalPosition;
    protected bool isInitialized;
    protected CancellationTokenSource _cts;
    protected readonly List<Tween> activeTweens = new();
    public PanelType PanelType => panelType;
    #endregion

    #region Unity Lifecycle
    protected virtual void Awake() => Setup();

    protected virtual void OnDestroy()
    {
        _cts?.Cancel();
        _cts?.Dispose();
        KillAllTweens();
    }
    #endregion

    #region Initialization
    protected virtual void Setup()
    {
        if (isInitialized) return;

        canvasGroup = GetComponent<CanvasGroup>();
        contentRoot = GetComponent<RectTransform>();

        if (contentRoot != null)
            originalPosition = contentRoot.anchoredPosition;

        isInitialized = true;
        InitializeButtons();
        InitializeText();
    }

    #endregion

    #region Panel State Management
    public virtual async Task ShowAsync(object transitionData = null)
    {
        if (!isInitialized) Setup();
        PrepareForShow();
        await EventBus.PublishAuto(new PlayAudioEvent(panelAudio));
        await Task.CompletedTask;
    }

    public virtual async Task HideAsync(object transitionData = null)
    {
        // await EventBus.PublishAuto(new StopPlayingAudioEvent());
        CleanUpAfterHide();
        await Task.CompletedTask;
    }

    protected virtual void PrepareForShow()
    {
        _cts = new CancellationTokenSource();
        gameObject.SetActive(true);
        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        contentRoot.anchoredPosition = originalPosition;
    }

    protected virtual void CleanUpAfterHide()
    {
        _cts?.Cancel();
        _cts?.Dispose();
        _cts = null;
        gameObject.SetActive(false);
    }

    #endregion

    #region State Callbacks
    public virtual void OnPause()
    {
        KillAllTweens();
    }

    public virtual void OnResume()
    {

    }

    public virtual void OnQuit()
    {
        KillAllTweens();
    }
    #endregion

    #region UI Initialization
    protected virtual void InitializeButtons() { }
    protected virtual void InitializeText() { }
    #endregion

    #region Button Interaction
    protected virtual void PlayButtonClickFeedback(Transform buttonTransform)
    {
        if (buttonTransform == null) return;
        Effects.Buttons.PlayClick(buttonTransform);
    }

    protected virtual void AddButtonListenerWithFeedback(Button button, Action action, bool includeClickSound = true)
    {
        if (button == null) return;

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() =>
        {
            PlayButtonClickFeedback(button.transform);
            action?.Invoke();
        });
    }

    protected virtual void AddButtonListenerWithFeedback(Button button, Func<Task> asyncAction, bool includeClickSound = true)
    {
        if (button == null) return;

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(async () =>
        {
            PlayButtonClickFeedback(button.transform);
            try
            {
                if (asyncAction != null)
                    await asyncAction.Invoke();
            }
            catch (Exception ex)
            {
                Debug.LogError($"Button action failed: {ex}");
            }
        });
    }
    #endregion

    #region Tween Management
    protected Tween RegisterTween(Tween tween)
    {
        if (tween != null && tween.IsActive())
            activeTweens.Add(tween);
        return tween;
    }

    protected void KillAllTweens()
    {
        foreach (var tween in activeTweens)
        {
            if (tween.IsActive())
                tween.Kill();
        }
        activeTweens.Clear();
    }
    #endregion

    #region Editor Utilities
#if UNITY_EDITOR
    protected virtual void OnValidate()
    {
        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();
        if (contentRoot == null)
            contentRoot = GetComponent<RectTransform>();
            
    }
#endif
    #endregion
}

