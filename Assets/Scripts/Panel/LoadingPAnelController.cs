using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;


public class LoadingPanelController : BasePanelController
{
    [Header("Loading Specifics")]
    [SerializeField] private Slider loadingBar;
    [SerializeField] private TextMeshProUGUI loadingText;
    [SerializeField] private TextMeshProUGUI CoFuText;
    [SerializeField] private float minLoadingTime = 2f;
    [SerializeField] private float maxLoadingTime = 3f;

    protected override void Awake()
    {
        base.Awake();
        loadingBar.value = 0;
    }

    public override async Task ShowAsync(object transitionData = null)
    {
        loadingBar.value = 0;
        loadingText.text = "Loading... 0%";

        await base.ShowAsync(transitionData);
        await SimulateLoadingProgress();
    }

    private async Task SimulateLoadingProgress()
    {
        float elapsedTime = 0f;
        float randomLoadingTime = Random.Range(minLoadingTime, maxLoadingTime);

        while (elapsedTime < randomLoadingTime)
        {
            elapsedTime += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsedTime / randomLoadingTime);

            loadingBar.DOValue(progress, 0.2f);
            loadingText.text = $"Loading... {Mathf.FloorToInt(progress * 100)}%";

            await Task.Yield();
        }

        loadingBar.value = 1f;
        loadingText.text = "Loading... 100%";
        await Task.Delay(300);
    }

}
