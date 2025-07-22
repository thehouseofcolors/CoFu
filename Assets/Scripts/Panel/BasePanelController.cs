/// <summary>
///
/// 📌 Sorumluluklar:
/// - UI panellerinin temel görünme/gizlenme geçişlerini ve buton kurulumlarını yönetir.
/// - `Awake()` sırasında veya `ShowAsync()` öncesinde güvenli şekilde `Setup()` çağrısı ile başlatılır.
///
/// 🧩 Etkileşimde Bulunduğu Sistemler:
/// - Effects.PanelTransition (geçiş animasyonları)
/// - Effects.Buttons (buton tıklama animasyonları)
///
/// 🧩 Uyguladığı Interface:
/// - IPanel: ShowAsync(), HideAsync(), OnPause(), OnResume(), OnQuit()
///
/// 📡 Yayınladığı Eventler:
/// - Yok
///
/// 📝 Notlar:
/// - Tüm paneller bu sınıftan türemelidir.
/// - `InitializeButtons()` ve `InitializeText()` override edilerek özelleştirilmelidir.
/// - `ShowAsync()` paneli hazırlar ama geçiş animasyonu içermez. Gerekirse override edilmelidir.
/// - `CancellationTokenSource` ile animasyon/işlemler gerektiğinde iptal edilebilir.
/// 
/// 
/// ✅ Geliştirme Durumu: TAMAMLANMADI (Soyut temel panel sınıfı)
/// </summary>

using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Threading;
//bu her panele eklenen scriptin abstractı dolayısıyla awake vs lazım yada initializeı dışardan yapıcam ama gereksiz gibi 


[RequireComponent(typeof(CanvasGroup))]
public abstract class BasePanelController : MonoBehaviour, IPanel
{

    [Header("UI References")]
    [SerializeField] protected RectTransform contentRoot;
    [SerializeField] protected CanvasGroup canvasGroup;
    [SerializeField] PanelType panelType;

    protected Vector2 originalPosition;
    protected bool isInitialized;

    protected CancellationTokenSource _cts;
    protected virtual void Awake()
    {
        // Ensure initialization happens even if script is disabled
        Setup();

    }

    protected virtual void Setup()
    {        
        if (isInitialized) return;

        canvasGroup = gameObject.GetComponent<CanvasGroup>();
        contentRoot = gameObject.GetComponent<RectTransform>();
        if (contentRoot != null)
        {
            originalPosition = contentRoot.anchoredPosition;
        }

        isInitialized = true;
        InitializeButtons();
        InitializeText();
    }

    protected virtual void AddButtonListenerWithFeedback(Button button, Action action, bool includeClickSound = true)
    {
        if (button == null) return;

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() =>
        {
            PlayButtonClickFeedback(button.transform);
            action?.Invoke();

        });
    }

    #region Ipanel Implementation

    public virtual async Task ShowAsync(object transitionData = null)
    {
        if (!isInitialized) Setup();
        PrepareForShow();

        await Task.CompletedTask;
    }
    public virtual async Task HideAsync(object transitionData = null)
    {
        CleanUpAfterHide();
        await Task.CompletedTask;

    }

    public virtual void OnPause() { }

    public virtual void OnResume() {}

    public virtual void OnQuit() {}

    #endregion


    protected virtual void PrepareForShow()
    {
        _cts = new CancellationTokenSource(); // << Bunu ekle
        gameObject.SetActive(true);
        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        contentRoot.anchoredPosition = originalPosition;
    }

    protected virtual void CleanUpAfterHide()
    {
        _cts?.Cancel();
        gameObject.SetActive(false);
    }


    protected virtual void PlayButtonClickFeedback(Transform buttonTransform)
    {
        if (buttonTransform == null) return;
        Effects.Buttons.PlayClick(buttonTransform);
    }
    protected virtual void InitializeButtons() {}

    protected virtual void InitializeText() {}



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


