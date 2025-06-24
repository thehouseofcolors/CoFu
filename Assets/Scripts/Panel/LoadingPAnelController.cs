using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class LoadingPanelController : MonoBehaviour, IPanel
{
    [Header("References")]
    [SerializeField] private Slider loadingBar;
    [SerializeField] private TextMeshProUGUI loadingText;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private RectTransform panelContent;
    [SerializeField] private TextMeshProUGUI CoFuText;

    [Header("Animation Settings")]
    [SerializeField] private float fadeDuration = 0.5f;
    [SerializeField] private float scaleDuration = 0.7f;
    [SerializeField] private Ease scaleEase = Ease.OutBack;
    [SerializeField] private float minLoadingTime = 5f; // Minimum loading duration
    [SerializeField] private float maxLoadingTime = 6f; // Maximum loading duration

    private Vector3 _originalScale;
    private Sequence _pulseSequence;

    private void Awake()
    {
        if (canvasGroup == null) canvasGroup = GetComponent<CanvasGroup>();
        _originalScale = panelContent.localScale;


        // Setup loading bar
        loadingBar.minValue = 0;
        loadingBar.maxValue = 1;
        loadingBar.value = 0;
    }

    public async Task ShowAsync(object transitionData)
    {
        Debug.Log("LoadingPanel: ShowAsync started"); // EKLE

        StopAllAnimations();

        // Show animation
        var showSequence = DOTween.Sequence()
            .Append(canvasGroup.DOFade(1, fadeDuration))
            .Join(panelContent.DOScale(_originalScale, scaleDuration).SetEase(scaleEase));

        await showSequence.AsyncWaitForCompletion();

        // Start loading process
        await ShowRealLoadingProgress();
    }
    private async Task ShowRealLoadingProgress()
    {
        float fakeProgress = 0;
        while (fakeProgress < 1f)
        {
            fakeProgress += Time.deltaTime / minLoadingTime;
            loadingBar.value = fakeProgress;
            loadingText.text = $"Loading... {Mathf.FloorToInt(fakeProgress * 100)}%";
            await Task.Yield();
        }

        loadingBar.value = 1f;
        loadingText.text = "Loading... 100%";

        // await Task.Delay(300); // Ufak bir bekleme

    }
    private async Task SimulateLoadingProgress()
    {
        float elapsedTime = 0f;
        float randomLoadingTime = Random.Range(minLoadingTime, maxLoadingTime);

        // Text animation (optional)
        DOTween.To(() => loadingText.text, x => loadingText.text = x, "Loading...", 0.5f)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutQuad);

        // Smooth progress bar fill
        while (elapsedTime < randomLoadingTime)
        {
            elapsedTime += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsedTime / randomLoadingTime);

            // Animate the slider smoothly
            loadingBar.DOValue(progress, 0.2f).SetEase(Ease.OutQuad);

            // Update percentage text
            loadingText.text = $"Loading... {Mathf.FloorToInt(progress * 100)}%";

            await Task.Yield(); // Wait for next frame
        }

        // Ensure we reach 100%
        loadingBar.DOValue(1f, 0.3f);
        loadingText.text = "Loading... 100%";

        // Optional: Add a small delay at full load
        await Task.Delay(300);
    }

    public async Task HideAsync()
    {
        StopAllAnimations();

        var hideSequence = DOTween.Sequence()
            .Append(canvasGroup.DOFade(0, fadeDuration * 0.7f))
            .Join(panelContent.DOScale(Vector3.zero, scaleDuration * 0.5f).SetEase(Ease.InBack))
            .OnComplete(() => gameObject.SetActive(false));

        await hideSequence.AsyncWaitForCompletion();
    }

    private void StopAllAnimations()
    {
        _pulseSequence?.Kill();
        canvasGroup.DOKill();
        panelContent.DOKill();
        loadingBar.DOKill();
        DOTween.Kill(loadingText); // Stop text animation
    }

    private void OnDestroy()
    {
        StopAllAnimations();
    }

}

