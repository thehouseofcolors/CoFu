
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using GameEvents;

public class MenuPanelController : BasePanelController
{
    #region UI Elements
    [Header("UI Elements")]
    [SerializeField] private Button startButton;
    [SerializeField] private TextMeshProUGUI levelText;
    #endregion

    #region Private Fields
    private bool _isProcessingClick;
    private Tween _buttonTween;
    private RectTransform _startButtonTransform;
    #endregion

    #region Unity Lifecycle

    protected override void InitializeButtons()
    {
        startButton.transform.localScale = Vector3.zero;

        _startButtonTransform = startButton.GetComponent<RectTransform>();
        AddButtonListenerWithFeedback(startButton, OnStartClicked);
    }
    protected override void OnDestroy()
    {
        if (startButton != null)
        {
            startButton.onClick.RemoveAllListeners();
        }

        CleanUpTweens();
        base.OnDestroy();
    }
    #endregion

    #region Panel State Methods
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

    public override void OnPause()
    {
        base.OnPause();
        StopButtonTween();
    }

    public override void OnResume()
    {
        base.OnResume();
        RestartButtonTween();
    }

    public override void OnQuit()
    {
        base.OnQuit();
        CleanUpTweens();
    }
    #endregion

    #region Button Methods
    private void ResetButtonState()
    {
        _isProcessingClick = false;
        startButton.interactable = true;
        RestartButtonTween();
    }

    private async void OnStartClicked()
    {
        if (_isProcessingClick) return;

        try
        {
            _isProcessingClick = true;
            StopButtonTween();
            startButton.interactable = false;

            await EventBus.PublishAuto(new ButtonClickedEvent(ButtonType.Start, startButton));
            await GameStateMachine.SetStateAsync(new PlayingState());
        }
        finally
        {
            ResetButtonState();
        }
    }

    private void StopButtonTween()
    {
        if (_buttonTween != null && _buttonTween.IsActive())
        {
            _buttonTween.Kill();
            _buttonTween = null;
        }
    }

    private void RestartButtonTween()
    {
        if (startButton != null && startButton.gameObject.activeInHierarchy)
        {
            _buttonTween = Effects.Buttons.PlayBounce(_startButtonTransform);
        }
    }
    #endregion

    #region Helper Methods
    private void UpdateLevelText(int levelNumber)
    {
        if (levelText == null)
        {
            Debug.LogWarning("Level text reference missing!", this);
            return;
        }

        levelText.text = $"Level {levelNumber}";
        levelText.transform.DOScale(Vector3.one * 1.1f, 0.3f)
            .SetEase(Ease.OutBack);
    }

    private void CleanUpTweens()
    {
        StopButtonTween();

        if (levelText != null)
        {
            levelText.transform.DOKill();
        }
    }
    #endregion
}


