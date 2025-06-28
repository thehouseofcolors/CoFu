// using System.Threading.Tasks;
// using UnityEngine;
// using UnityEngine.UI;
// using DG.Tweening;
// using System;

// [RequireComponent(typeof(CanvasGroup))]
// public abstract class BaseLayOverController : MonoBehaviour, ILayOver
// {
//     [Header("Animation Settings")]
//     [SerializeField] protected float fadeDuration = 0.3f;
//     [SerializeField] protected float moveDuration = 0.4f;
//     [SerializeField] protected Ease showEase = Ease.OutBack;
//     [SerializeField] protected Ease hideEase = Ease.InBack;
//     [SerializeField] protected float verticalOffset = 100f;

//     [Header("UI References")]
//     [SerializeField] protected RectTransform contentRoot;
//     [SerializeField] protected CanvasGroup canvasGroup;
//     [SerializeField] protected Button dimmerButton;
    
//     [SerializeField] private float buttonClickFeedbackIntensity = 0.1f;

//     protected Vector2 originalPosition;
//     protected Sequence activeSequence;
//     protected bool isInitialized;

//     protected virtual void Awake() => Initialize();

//     protected virtual void Initialize()
//     {
//         if (canvasGroup == null) canvasGroup = GetComponent<CanvasGroup>();
//         if (contentRoot == null) contentRoot = GetComponent<RectTransform>();
        
//         originalPosition = contentRoot.anchoredPosition;
//         SetupDimmerButton();
//         isInitialized = true;
//     }

//     protected virtual void SetupDimmerButton()
//     {
//         if (dimmerButton != null)
//         {
//             dimmerButton.onClick.RemoveAllListeners();
//             dimmerButton.onClick.AddListener(() => _ = HideCurrentLayOverAsync());
//         }
//     }

//     public virtual async Task ShowLayOverAsync()
//     {
//         if (!isInitialized) Initialize();
        
//         PrepareForShow();
//         activeSequence?.Kill();
        
//         activeSequence = DOTween.Sequence()
//             .Append(canvasGroup.DOFade(1f, fadeDuration))
//             .Join(contentRoot.DOAnchorPos(originalPosition, moveDuration).SetEase(showEase))
//             .OnComplete(() => activeSequence = null);

//         await activeSequence.AsyncWaitForCompletion();
//     }

//     public virtual async Task HideCurrentLayOverAsync()
//     {
//         if (!gameObject.activeSelf) return;

//         activeSequence?.Kill();
        
//         activeSequence = DOTween.Sequence()
//             .Append(canvasGroup.DOFade(0f, fadeDuration))
//             .Join(contentRoot.DOAnchorPos(originalPosition + new Vector2(0, verticalOffset), moveDuration).SetEase(hideEase))
//             .OnComplete(() => {
//                 CleanUpAfterHide();
//                 activeSequence = null;
//             });

//         await activeSequence.AsyncWaitForCompletion();
//     }

//     protected virtual void PrepareForShow()
//     {
//         gameObject.SetActive(true);
//         canvasGroup.alpha = 0f;
//         contentRoot.anchoredPosition = originalPosition + new Vector2(0, verticalOffset);
//     }

//     protected virtual void CleanUpAfterHide()
//     {
//         gameObject.SetActive(false);
//         OnLayOverHidden();
//     }
//     // New method to handle button clicks with visual feedback
//     protected virtual void AddButtonListenerWithFeedback(Button button, Action action)
//     {
//         if (button != null)
//         {
//             button.onClick.RemoveAllListeners();
//             button.onClick.AddListener(() => 
//             {
//                 action?.Invoke();
//                 PlayButtonClickFeedback(button.transform);
//             });
//         }
//     }

//     // Button click feedback animation
//     protected virtual void PlayButtonClickFeedback(Transform buttonTransform)
//     {
//         if (buttonTransform == null) return;
        
//         buttonTransform.DOKill();
//         buttonTransform.DOPunchScale(
//             Vector3.one * buttonClickFeedbackIntensity, 
//             0.3f, 
//             2, 
//             0.5f
//         ).OnComplete(() => buttonTransform.localScale = Vector3.one);
//     }

//     protected virtual void OnLayOverHidden()
//     {
//         // Optional override point for child classes
//     }

//     protected virtual void OnDestroy()
//     {
//         activeSequence?.Kill();
//         if (dimmerButton != null)
//         {
//             dimmerButton.onClick.RemoveAllListeners();
//         }
//     }
// }

using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

[RequireComponent(typeof(CanvasGroup))]
public abstract class BaseLayOverController : MonoBehaviour, ILayOver
{
    [Header("Animation Settings")]
    [SerializeField, Range(0.1f, 2f)] protected float fadeDuration = 0.3f;
    [SerializeField, Range(0.1f, 2f)] protected float moveDuration = 0.4f;
    [SerializeField] protected Ease showEase = Ease.OutBack;
    [SerializeField] protected Ease hideEase = Ease.InBack;
    [SerializeField, Min(0)] protected float verticalOffset = 100f;
    [SerializeField, Range(0.05f, 0.5f)] protected float buttonClickFeedbackIntensity = 0.1f;
    [SerializeField] protected float buttonClickDuration = 0.2f;

    [Header("UI References")]
    [SerializeField] protected RectTransform contentRoot;
    [SerializeField] protected CanvasGroup canvasGroup;
    // [SerializeField] protected Button dimmerButton;

    protected Vector2 originalPosition;
    protected Sequence activeSequence;
    protected bool isInitialized;

    protected virtual void Awake()
    {
        // Ensure initialization happens even if script is disabled
        Initialize();
    }

    protected virtual void Initialize()
    {
        if (isInitialized) return;

        TryGetComponent(out canvasGroup);
        TryGetComponent(out contentRoot);
        
        if (contentRoot != null)
        {
            originalPosition = contentRoot.anchoredPosition;
        }

        SetupDimmerButton();
        isInitialized = true;
    }

    protected virtual void SetupDimmerButton()
    {
        // if (dimmerButton == null) return;
        
        // dimmerButton.onClick.RemoveAllListeners();
        // dimmerButton.onClick.AddListener(OnDimmerClicked);
    }

    protected virtual void OnDimmerClicked()
    {
        // Added virtual modifier for potential override
        _ = HideCurrentLayOverAsync();
    }

    public virtual async Task ShowLayOverAsync(object transitionData=null)
    {
        if (!isInitialized) Initialize();
        if (this == null) return;

        PrepareForShow();
        activeSequence?.Kill(complete: true);
        
        try
        {
            activeSequence = DOTween.Sequence()
                .Append(canvasGroup.DOFade(1f, fadeDuration))
                .Join(contentRoot.DOAnchorPos(originalPosition, moveDuration).SetEase(showEase))
                .SetUpdate(true) // Works during pause
                .OnComplete(() => activeSequence = null);

            await activeSequence.AsyncWaitForCompletion();
        }
        catch (Exception e)
        {
            Debug.LogError($"Show animation failed: {e}");
            activeSequence = null;
        }
    }

    public virtual async Task HideCurrentLayOverAsync()
    {
        if (!gameObject.activeSelf || this == null) return;

        activeSequence?.Kill(complete: true);
        
        try
        {
            activeSequence = DOTween.Sequence()
                .Append(canvasGroup.DOFade(0f, fadeDuration))
                .Join(contentRoot.DOAnchorPos(originalPosition + new Vector2(0, verticalOffset), moveDuration).SetEase(hideEase))
                .SetUpdate(true)
                .OnComplete(() =>
                {
                    CleanUpAfterHide();
                    activeSequence = null;
                });

            await activeSequence.AsyncWaitForCompletion();
        }
        catch (Exception e)
        {
            Debug.LogError($"Hide animation failed: {e}");
            CleanUpAfterHide();
            activeSequence = null;
        }
    }

    protected virtual void PrepareForShow()
    {
        gameObject.SetActive(true);
        canvasGroup.alpha = 0f;
        contentRoot.anchoredPosition = originalPosition + new Vector2(0, verticalOffset);
    }

    protected virtual void CleanUpAfterHide()
    {
        gameObject.SetActive(false);
        OnLayOverHidden();
    }

    protected virtual void AddButtonListenerWithFeedback(Button button, Action action, bool includeClickSound = true)
    {
        if (button == null) return;

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() =>
        {
            PlayButtonClickFeedback(button.transform);
            action?.Invoke();
            
            // Example for sound - implement your audio system
            // if (includeClickSound) AudioManager.PlayButtonClick(); 
        });
    }

    protected virtual void PlayButtonClickFeedback(Transform buttonTransform)
    {
        if (buttonTransform == null) return;

        buttonTransform.DOKill();
        var sequence = DOTween.Sequence()
            .Append(buttonTransform.DOScale(Vector3.one * (1f - buttonClickFeedbackIntensity), buttonClickDuration/2))
            .Append(buttonTransform.DOScale(Vector3.one, buttonClickDuration/2))
            .SetEase(Ease.OutQuad)
            .SetUpdate(true);
    }

    protected virtual void OnLayOverHidden()
    {
        // Optional override point
    }

    protected virtual void OnDestroy()
    {
        activeSequence?.Kill();
        // if (dimmerButton != null)
        // {
        //     dimmerButton.onClick.RemoveAllListeners();
        // }
        
        // Cleanup all tweens on this transform
        DOTween.Kill(transform);
    }

    #if UNITY_EDITOR
    protected virtual void OnValidate()
    {
        // Auto-get references in editor but not at runtime
        if (!Application.isPlaying)
        {
            if (canvasGroup == null) canvasGroup = GetComponent<CanvasGroup>();
            if (contentRoot == null) contentRoot = GetComponent<RectTransform>();
        }
    }
    #endif
}