using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using GameEvents;
using DG.Tweening;
using TMPro;
using System;

[RequireComponent(typeof(CanvasGroup))]
public class FailLayOverController : BaseLayOverController
{
    [Header("Fail Overlay References")]
    [SerializeField] private Button restartButton;
    [SerializeField] private Button menuButton;
    [SerializeField] private TextMeshProUGUI failText;
    [SerializeField] private ParticleSystem failParticles;
    
    [Header("Animation Settings")]
    [SerializeField, Range(0.1f, 1f)] private float buttonStaggerDelay = 0.3f;
    [SerializeField, Range(0.5f, 3f)] private float textRevealDuration = 1f;
    // [SerializeField] private AudioClip failSound;

    private bool _isTransitioning;

    protected override void Initialize()
    {
        base.Initialize();
        
        // Validate critical components
        if (restartButton == null || menuButton == null)
        {
            Debug.LogError("FailLayOverController: Required buttons not assigned!");
            enabled = false;
            return;
        }

        InitializeButtons();
        InitializeText();
    }

    private void InitializeButtons()
    {
        restartButton.transform.localScale = Vector3.zero;
        menuButton.transform.localScale = Vector3.zero;
        
        AddButtonListenerWithFeedback(restartButton, OnRestartClicked);
        AddButtonListenerWithFeedback(menuButton, OnMenuClicked);
    }

    private void InitializeText()
    {
        if (failText != null)
        {
            failText.alpha = 0f;
            failText.gameObject.SetActive(true);
        }
    }

    public override async Task ShowLayOverAsync(object transitionData = null)
    {
        if (_isTransitioning) return;
        _isTransitioning = true;

        try
        {
            // Play fail effects
            PlayFailEffects();
            
            await base.ShowLayOverAsync(transitionData);
            
            // Run parallel animations
            await Task.WhenAll(
                AnimateText(),
                AnimateButtons()
            );
        }
        finally
        {
            _isTransitioning = false;
        }
    }

    private void PlayFailEffects()
    {
        // AudioManager.Play(failSound);
        if (failParticles != null) failParticles.Play();
    }

    private async Task AnimateText()
    {
        if (failText == null) return;
        
        await failText.DOFade(1f, textRevealDuration)
            .SetEase(Ease.OutQuad)
            .AsyncWaitForCompletion();
    }

    private async Task AnimateButtons()
    {
        var sequence = DOTween.Sequence();
        
        sequence.Append(restartButton.transform.DOScale(1f, 0.4f))
            .SetEase(Ease.OutBack);
            
        sequence.Append(menuButton.transform.DOScale(1f, 0.4f))
            .SetEase(Ease.OutBack)
            .SetDelay(buttonStaggerDelay);
            
        await sequence.AsyncWaitForCompletion();
        
        // Enable interaction only after animations complete
        restartButton.interactable = true;
        menuButton.interactable = true;
    }

    private void OnRestartClicked()
    {
        if (_isTransitioning) return;
        
        PlayButtonClickFeedback(restartButton.transform);
        EventBus.PublishAuto(new LevelRestartRequestedEvent());
        _ = HideCurrentLayOverAsync();
    }

    private async void OnMenuClicked()
    {
        if (_isTransitioning) return;
        
        PlayButtonClickFeedback(menuButton.transform);
        
        try
        {
            _isTransitioning = true;
            await GameStateMachine.ChangeStateAsync(new MenuState());
            await HideCurrentLayOverAsync();
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to return to menu: {e}");
        }
        finally
        {
            _isTransitioning = false;
        }
    }

    protected override void OnLayOverHidden()
    {
        // Reset button states
        restartButton.interactable = false;
        menuButton.interactable = false;
        
        // Stop particles if playing
        if (failParticles != null && failParticles.isPlaying)
        {
            failParticles.Stop();
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        // Clean up button listeners
        if (restartButton != null) restartButton.onClick.RemoveAllListeners();
        if (menuButton != null) menuButton.onClick.RemoveAllListeners();
    }
}