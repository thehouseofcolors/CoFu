//panele ekle

using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Threading;
using DG.Tweening;

public class LoadingPanelController : BasePanelController
{
    [Header("Loading Settings")]
    [SerializeField] private Slider loadingBar;
    [SerializeField] private TextMeshProUGUI loadingText;
    [SerializeField] private TextMeshProUGUI CoFuText;
    [SerializeField, Min(0.1f)] private float loadingDuration = 5f;
    [SerializeField, Min(0)] private float completionDelay = 3f;

    private CancellationTokenSource _loadingCTS;
    private Tween _bounceTween;

    protected override void Awake()
    {
        base.Awake();
        ResetLoadingUI();
        // await ShowAsync();
        // await HideAsync();
    }

    private void ResetLoadingUI()
    {
        if (loadingBar != null) loadingBar.value = 0;
        if (loadingText != null) loadingText.text = "Loading... 0%";
    }

    public override async Task ShowAsync(object transitionData = null)
    {
        await base.ShowAsync();
        _loadingCTS = new CancellationTokenSource();

        StartBounceAnimation();
        await Effects.PanelTransition.Fade(canvasGroup, true);

        await RunLoadingOperation(_loadingCTS.Token);
    }

    public override async Task HideAsync(object transitionData = null)
    {
        StopBounceAnimation();
        await Effects.PanelTransition.Slide(contentRoot, Vector2.right, false);
        CancelLoadingOperation();
        await base.HideAsync();
    }

    private void StartBounceAnimation()
    {
        if (CoFuText != null)
        {
            _bounceTween = Effects.Texts.Bounce(CoFuText.transform);
            
        }
    }

    
    private void StopBounceAnimation()
    {
        if (_bounceTween != null && _bounceTween.IsActive())
        {
            _bounceTween.Kill();
            _bounceTween = null;
        }
    }

    private async Task RunLoadingOperation(CancellationToken token)
    {
        try
        {
            await AnimateLoadingProgress(token);
            await FinalizeLoading(token);
        }
        catch (TaskCanceledException)
        {
            Debug.Log("Loading operation cancelled");
        }
    }

    private async Task AnimateLoadingProgress(CancellationToken token)
    {
        float elapsed = 0f;
        while (elapsed < loadingDuration && !token.IsCancellationRequested)
        {
            elapsed += Time.deltaTime;
            UpdateProgressDisplay(elapsed / loadingDuration);
            await Task.Yield();
        }
    }

    private async Task FinalizeLoading(CancellationToken token)
    {
        if (token.IsCancellationRequested) return;

        UpdateProgressDisplay(1f);
        await Task.Delay((int)(completionDelay * 1000), token);
    }

    private void UpdateProgressDisplay(float progress)
    {
        progress = Mathf.Clamp01(progress);
        if (loadingBar != null) loadingBar.value = progress;
        if (loadingText != null) loadingText.text = $"Loading... {Mathf.FloorToInt(progress * 100)}%";
    }

    private void CancelLoadingOperation()
    {
        _loadingCTS?.Cancel();
        _loadingCTS?.Dispose();
        _loadingCTS = null;
    }

    protected override void CleanUpAfterHide()
    {
        StopBounceAnimation();
        CancelLoadingOperation();
        base.CleanUpAfterHide();
    }

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();
        loadingDuration = Mathf.Max(0.1f, loadingDuration);
        completionDelay = Mathf.Max(0, completionDelay);
    }
#endif
}

