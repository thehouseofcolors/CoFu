using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;



[RequireComponent(typeof(CanvasGroup))]
public abstract class BasePanelController : MonoBehaviour, IPanel
{
    [Header("Base Panel Settings")]
    [SerializeField] protected float fadeDuration = 0.3f;
    [SerializeField] protected float scaleDuration = 0.5f;
    [SerializeField] protected Ease showEase = Ease.OutBack;
    [SerializeField] protected Ease hideEase = Ease.InBack;
    [SerializeField] protected float contentScaleMultiplier = 1.1f;
    [SerializeField] protected float buttonAppearDelay = 0.1f;
    [SerializeField] protected float buttonClickFeedbackIntensity = 0.1f;
    
    [Header("Base Panel References")]
    [SerializeField] protected RectTransform panelContent;
    [SerializeField] protected CanvasGroup canvasGroup;

    protected Vector3 _originalScale;
    protected Sequence _currentAnimation;
    protected bool _isInitialized;

    protected virtual void Awake()
    {
        InitializeComponents();
    }

    protected virtual void InitializeComponents()
    {
        if (canvasGroup == null) canvasGroup = GetComponent<CanvasGroup>();
        if (panelContent == null) panelContent = GetComponent<RectTransform>();
        
        _originalScale = panelContent.localScale;
        _isInitialized = true;
    }

    public virtual async Task ShowAsync(object transitionData = null)
    {
        if (!_isInitialized) InitializeComponents();
        
        gameObject.SetActive(true);
        canvasGroup.alpha = 0;
        panelContent.localScale = Vector3.zero;

        // Reset animation if already playing
        _currentAnimation?.Kill();

        // Create show animation sequence
        _currentAnimation = DOTween.Sequence()
            .Append(canvasGroup.DOFade(1, fadeDuration))
            .Join(panelContent.DOScale(_originalScale * contentScaleMultiplier, scaleDuration * 0.7f).SetEase(Ease.OutQuad))
            .Append(panelContent.DOScale(_originalScale, scaleDuration * 0.3f).SetEase(showEase))
            .OnComplete(() => _currentAnimation = null);

        await _currentAnimation.AsyncWaitForCompletion();
    }

    public virtual async Task HideAsync()
    {
        if (!gameObject.activeSelf) return;

        // Reset animation if already playing
        _currentAnimation?.Kill();

        // Create hide animation
        _currentAnimation = DOTween.Sequence()
            .Append(canvasGroup.DOFade(0, fadeDuration * 0.7f))
            .Join(panelContent.DOScale(Vector3.zero, scaleDuration * 0.5f).SetEase(hideEase))
            .OnComplete(() =>
            {
                gameObject.SetActive(false);
                _currentAnimation = null;
                OnPanelHidden();
            });

        await _currentAnimation.AsyncWaitForCompletion();
    }

    protected virtual void OnPanelHidden()
    {
        // Override in child classes for custom behavior
    }

    protected void PlayButtonAppearAnimation(Transform buttonTransform, float delayMultiplier = 1f)
    {
        buttonTransform.localScale = Vector3.zero;
        buttonTransform.DOScale(Vector3.one, 0.4f)
            .SetEase(Ease.OutBack)
            .SetDelay(buttonAppearDelay * delayMultiplier);
    }

    protected void PlayButtonClickFeedback(Transform buttonTransform)
    {
        buttonTransform.DOKill();
        buttonTransform.DOPunchScale(
            Vector3.one * buttonClickFeedbackIntensity, 
            0.3f, 
            2, 
            0.5f
        ).OnComplete(() => buttonTransform.localScale = Vector3.one);
    }

    protected virtual void OnDestroy()
    {
        // Clean up all tweens
        _currentAnimation?.Kill();
        DOTween.Kill(transform);
    }

    protected void SafeAddButtonListener(Button button, Action action)
    {
        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => 
            {
                action?.Invoke();
                PlayButtonClickFeedback(button.transform);
            });
        }
    }
}