using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using GameEvents;
using DG.Tweening;
using TMPro;
using System;

[RequireComponent(typeof(CanvasGroup))]
public class WinLayOverController : BaseLayOverController
{
    [Header("Win Overlay References")]
    [SerializeField] private Button continueButton;
    [SerializeField] private Button menuButton;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private ParticleSystem celebrationParticles;
    
    [Header("Animation Settings")]
    [SerializeField, Range(0.05f, 0.5f)] private float buttonStaggerDelay = 0.15f;
    [SerializeField, Range(0.5f, 3f)] private float scoreCountDuration = 1.5f;
    [SerializeField, Min(0)] private int minScoreDisplay = 0;
    // [SerializeField] private AudioClip victorySound;

    private bool _isAnimating;
    private int _finalScore;

    protected override void Initialize()
    {
        base.Initialize();
        
        // Validate and initialize components
        if (continueButton == null || menuButton == null)
        {
            Debug.LogError("WinLayOverController: Required buttons not assigned!");
            enabled = false;
            return;
        }

        InitializeButtons();
        InitializeScoreDisplay();
    }

    private void InitializeButtons()
    {
        continueButton.transform.localScale = Vector3.zero;
        menuButton.transform.localScale = Vector3.zero;
        
        AddButtonListenerWithFeedback(continueButton, OnContinueClicked);
        AddButtonListenerWithFeedback(menuButton, OnMenuClicked);
    }

    private void InitializeScoreDisplay()
    {
        if (scoreText != null)
        {
            scoreText.text = minScoreDisplay.ToString();
        }
    }

    public override async Task ShowLayOverAsync(object transitionData=null)
    {
        if (_isAnimating) return;
        _isAnimating = true;

        try
        {
            // // Get score from transition data if available
            // _finalScore = transitionData is int score ? Mathf.Max(score, minScoreDisplay) : minScoreDisplay;
            
            await base.ShowLayOverAsync();
            
            // Play celebration effects
            PlayCelebrationEffects();
            
            // Run parallel animations
            await Task.WhenAll(
                AnimateScoreDisplay(),
                AnimateButtons()
            );
        }
        finally
        {
            _isAnimating = false;
        }
    }

    private void PlayCelebrationEffects()
    {
        // Play sound effect if available
        // AudioManager.Play(victorySound);
        
        // Play particles if available
        if (celebrationParticles != null)
        {
            celebrationParticles.Play();
        }
    }

    private async Task AnimateScoreDisplay()
    {
        if (scoreText == null) return;
        
        float currentScore = minScoreDisplay;
        scoreText.text = currentScore.ToString();
        
        await DOTween.To(
            () => currentScore,
            x => {
                currentScore = x;
                scoreText.text = Mathf.FloorToInt(currentScore).ToString();
            },
            _finalScore,
            scoreCountDuration
        ).SetEase(Ease.OutQuad)
         .AsyncWaitForCompletion();
    }

    private async Task AnimateButtons()
    {
        var sequence = DOTween.Sequence();
        
        // Continue button animation
        if (continueButton != null)
        {
            sequence.Append(continueButton.transform.DOScale(1f, 0.3f)
                .SetEase(Ease.OutBack));
        }
            
        // Restart button animation with delay
        if (menuButton != null)
        {
            sequence.Append(menuButton.transform.DOScale(1f, 0.3f)
                .SetEase(Ease.OutBack)
                .SetDelay(buttonStaggerDelay));
        }
            
        await sequence.AsyncWaitForCompletion();
    }

    private void OnContinueClicked()
    {
        if (_isAnimating) return;
        
        PlayButtonClickFeedback(continueButton.transform);
        Debug.Log("Continue to next level");
        
        // Example event publication
        EventBus.PublishAuto(new NextLevelRequestedEvent());
        _ = HideCurrentLayOverAsync();
    }

    private async void OnMenuClicked()
    {
        if (_isAnimating) return;
        
        PlayButtonClickFeedback(menuButton.transform);
        
        try
        {
            _isAnimating = true;
            // await GameStateMachine.ChangeStateAsync(new Game());
            await HideCurrentLayOverAsync();
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to restart game: {e}");
            // Consider showing error feedback to player
        }
        finally
        {
            _isAnimating = false;
        }
    }

    protected override void OnLayOverHidden()
    {
        // Stop any ongoing celebration effects
        if (celebrationParticles != null && celebrationParticles.isPlaying)
        {
            celebrationParticles.Stop();
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        // Clean up button listeners
        if (continueButton != null) continueButton.onClick.RemoveAllListeners();
        if (menuButton != null) menuButton.onClick.RemoveAllListeners();
    }
}