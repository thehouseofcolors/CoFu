//panele ekle

using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using GameEvents;

public class MenuPanelController : BasePanelController
{
    [Header("UI Elements")]
    [SerializeField] private Button startButton;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private float buttonPressScale = 0.95f;
    [SerializeField] private float buttonAnimationDuration = 0.2f;

    private bool _isProcessingClick;
    private Tween _buttonTween;

    protected override void Awake()
    {
        base.Awake();

        if (startButton == null)
        {
            Debug.LogError("Start button reference missing!", this);
            return;
        }

        AddButtonListenerWithFeedback(startButton, OnStartClicked);
    }

    public override async Task ShowAsync(object transitionData = null)
    {
        await base.ShowAsync();

        ResetButtonState();

        if (transitionData is int level)
        {
            UpdateLevelText(level);
        }
        
        await Effects.PanelTransition.Fade(canvasGroup, true);
    }
    public override async Task HideAsync(object transitionData = null)
    {

        await Effects.PanelTransition.Slide(contentRoot, Vector2.right, false);
        await base.HideAsync();

    }
    private void ResetButtonState()
    {
        _isProcessingClick = false;
        startButton.interactable = true;

        // Başlat bounce loop
        _buttonTween = Effects.Buttons.PlayBounce(startButton.transform);

    }


    // private void ResetButtonState()
    // {
    //     _isProcessingClick = false;
    //     startButton.interactable = true;

    //     if (_buttonTween != null && _buttonTween.IsActive())
    //     {
    //         _buttonTween.Kill();
    //     }
    //     startButton.transform.localScale = Vector3.one;
    // }

    private async void OnStartClicked()
    {
        if (_isProcessingClick) return;
        _isProcessingClick = true;
        // Stop bounce anim
        
        

        // Visual feedback
        startButton.interactable = false;
        await AnimateButtonPress();

        // Trigger game load
        await EventBus.PublishAuto(new GameLoadEvent());
    }

    private async Task AnimateButtonPress()
    {
        _buttonTween = startButton.transform.DOScale(
            Vector3.one * buttonPressScale,
            buttonAnimationDuration
        ).SetLoops(2, LoopType.Yoyo);

        await _buttonTween.AsyncWaitForCompletion();
    }

    private void UpdateLevelText(int levelNumber)
    {
        if (levelText == null) return;

        levelText.text = $"Level {levelNumber}";
        levelText.transform.DOScale(Vector3.one * 1.1f, 0.3f)
            .SetEase(Ease.OutBack);
    }

    protected override void CleanUpAfterHide()
    {
        if (_buttonTween != null)
        {
            _buttonTween.Kill();
            _buttonTween = null;
        }
        base.CleanUpAfterHide();
    }

    private void OnDestroy()
    {
        if (startButton != null)
        {
            startButton.onClick.RemoveAllListeners();
        }
    }
}

