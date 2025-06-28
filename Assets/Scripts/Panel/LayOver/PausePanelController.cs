using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using GameEvents;
using DG.Tweening;
using TMPro;
using System;
using System.Collections.Generic;

public class PausePanelController : BaseLayOverController
{
    [Header("Pause Panel References")]
    [SerializeField] private Button extraTimeButton;
    [SerializeField] private Button failButton;
    [SerializeField] private TextMeshProUGUI pauseText;
    [SerializeField] private Button[] additionalButtons;
    
    [Header("Animation Settings")]
    [SerializeField, Range(0.05f, 0.5f)] private float buttonStaggerDelay = 0.1f;
    [SerializeField, Range(0.1f, 1f)] private float textFadeDuration = 0.5f;

    private List<Button> _allButtons = new List<Button>();
    private bool _isTransitioning;

    protected override void Initialize()
    {
        base.Initialize();
        CacheAllButtons();
        InitializeUIElements();
    }

    private void CacheAllButtons()
    {
        _allButtons.Clear();
        if (extraTimeButton != null) _allButtons.Add(extraTimeButton);
        if (failButton != null) _allButtons.Add(failButton);
        if (additionalButtons != null) _allButtons.AddRange(additionalButtons);
    }

    private void InitializeUIElements()
    {
        InitializeText();
        InitializeAllButtons();
    }

    private void InitializeText()
    {
        if (pauseText != null)
        {
            pauseText.alpha = 0f;
            pauseText.gameObject.SetActive(true);
        }
    }

    private void InitializeAllButtons()
    {
        AddButtonListenerWithFeedback(extraTimeButton, OnExtraTimeClicked);
        AddButtonListenerWithFeedback(failButton, OnFailClicked);
        
        foreach (var button in _allButtons)
        {
            if (button != null)
            {
                button.transform.localScale = Vector3.zero;
                button.gameObject.SetActive(true);
                button.interactable = false;
            }
        }
    }

    public override async Task ShowLayOverAsync(object transitionData=null)
    {
        if (_isTransitioning) return;
        _isTransitioning = true;

        try
        {
            ResetAllElements();
            await base.ShowLayOverAsync();
            await AnimateAllElements();
        }
        finally
        {
            _isTransitioning = false;
        }
    }

    private void ResetAllElements()
    {
        if (pauseText != null) pauseText.alpha = 0f;
        foreach (var button in _allButtons)
        {
            ResetButton(button);
        }
    }

    private void ResetButton(Button button)
    {
        if (button != null)
        {
            button.transform.localScale = Vector3.zero;
            button.interactable = false;
        }
    }

    private async Task AnimateAllElements()
    {
        var animationTasks = new List<Task>
        {
            AnimateText(),
            AnimateButtons()
        };
        
        await Task.WhenAll(animationTasks);
        SetButtonsInteractable(true);
    }

    private async Task AnimateText()
    {
        if (pauseText == null) return;
        
        await pauseText.DOFade(1f, textFadeDuration)
            .SetEase(Ease.OutQuad)
            .AsyncWaitForCompletion();
    }

    private async Task AnimateButtons()
    {
        var buttonAnimationTasks = new List<Task>();
        float delay = 0;

        foreach (var button in _allButtons)
        {
            if (button != null)
            {
                buttonAnimationTasks.Add(AnimateButton(button, delay));
                delay += buttonStaggerDelay;
            }
        }

        await Task.WhenAll(buttonAnimationTasks);
    }

    private async Task AnimateButton(Button button, float delay)
    {
        await button.transform.DOScale(1f, 0.3f)
            .SetEase(Ease.OutBack)
            .SetDelay(delay)
            .AsyncWaitForCompletion();
    }

    private void SetButtonsInteractable(bool interactable)
    {
        foreach (var button in _allButtons)
        {
            if (button != null)
            {
                button.interactable = interactable;
            }
        }
    }

    private void OnExtraTimeClicked()
    {
        if (_isTransitioning) return;
        
        Debug.Log("Extra time requested");
        EventBus.PublishAuto(new ExtraTimeRequestedEvent());
        _ = HideCurrentLayOverAsync();
    }

    private async void OnFailClicked()
    {
        if (_isTransitioning) return;
        
        try 
        {
            _isTransitioning = true;
            await GameStateMachine.ChangeStateAsync(new GameFailState());
            await HideCurrentLayOverAsync();
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to transition to fail state: {e}");
        }
        finally
        {
            _isTransitioning = false;
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        CleanUpAllButtons();
    }

    private void CleanUpAllButtons()
    {
        foreach (var button in _allButtons)
        {
            CleanUpButton(button);
        }
        _allButtons.Clear();
    }

    private void CleanUpButton(Button button)
    {
        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.transform.DOKill();
        }
    }
}