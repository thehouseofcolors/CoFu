using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using GameEvents;
using DG.Tweening;
using TMPro;
using System;


public class StartPanelController : BasePanelController
{
    [Header("Start Panel Specifics")]
    [SerializeField] private Button startButton;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI levelTitleText;
    [SerializeField] private float buttonPulseAmount = 0.1f;
    [SerializeField] private float buttonPulseDuration = 1f;

    private Sequence _pulseSequence;
    private IDisposable _levelUpdateSub;

    protected override void Awake()
    {
        base.Awake();
        
        SafeAddButtonListener(startButton, OnStartClicked);
        _levelUpdateSub = EventBus.Subscribe<LevelInfoUpdateEvent>(OnLevelInfoUpdated);
    }

    public override async Task ShowAsync(object transitionData = null)
    {
        await base.ShowAsync(transitionData);
        StartButtonPulseAnimation();
        
        if (levelTitleText != null)
        {
            levelTitleText.transform.localScale = Vector3.zero;
            levelTitleText.transform.DOScale(Vector3.one, scaleDuration * 0.7f)
                .SetEase(showEase)
                .SetDelay(buttonAppearDelay * 2f);
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        _levelUpdateSub?.Dispose();
        _pulseSequence?.Kill();
    }

    private void OnStartClicked()
    {
        _pulseSequence?.Kill();
        EventBus.PublishAuto(new GameStartRequestedEvent());
    }

    private void OnLevelInfoUpdated(LevelInfoUpdateEvent evt)
    {
        levelText.text = $"Level {evt.LevelNumber}";
    }

    private void StartButtonPulseAnimation()
    {
        _pulseSequence = DOTween.Sequence()
            .Append(startButton.transform.DOScale(Vector3.one * (1 + buttonPulseAmount), buttonPulseDuration/2))
            .Append(startButton.transform.DOScale(Vector3.one, buttonPulseDuration/2))
            .SetLoops(-1);
    }
}

