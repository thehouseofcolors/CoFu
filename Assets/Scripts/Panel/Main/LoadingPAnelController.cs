/// <summary>
/// loading panel objesine eklenecek
/// oyun ilk açıldığında veya tamamen kapandıktan sonra açıldığında gösterilecek
/// sahte bir loading ekranı var
/// 
/// geliştirmesi tamamlandı değişiklik yapma
/// </summary>

using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Threading;
using DG.Tweening;//bu tween kullanmak için var silme.

/// geliştirmesi tamamlandı değişiklik yapma
public class LoadingPanelController : BasePanelController
{
    [Header("Loading Settings")]
    [SerializeField] private Slider loadingBar;
    [SerializeField] private TextMeshProUGUI loadingText;
    [SerializeField] private TextMeshProUGUI CoFuText;
    private float loadingDuration = 1f;
    private float completionDelay = 0.2f;

    private CancellationTokenSource _loadingCTS;
    private Tween _bounceTween;

    public override async Task ShowAsync(object transitionData = null)
    {
        await base.ShowAsync();
        _loadingCTS = new CancellationTokenSource();
        await Effects.PanelTransition.Fade(canvasGroup, true);
        StartBounceAnimation();
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


    public override void OnPause()
    {
        base.OnPause(); // panel sesini durdurur
        StopBounceAnimation();
        CancelLoadingOperation(); 
    }

    public override void OnResume()
    {
        base.OnResume(); // panel sesini başlatır
        StartBounceAnimation();
    }

    public override void OnQuit()
    {
        base.OnQuit();
        StopBounceAnimation();
        CancelLoadingOperation();
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

