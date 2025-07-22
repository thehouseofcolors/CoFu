
using System.Threading.Tasks;
using GameEvents;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;



public class SlotController : MonoBehaviour, IColorSource
{
    [Header("Components")]
    [SerializeField] private Image slotImage;
    [SerializeField] private Button slotButton;
    [SerializeField] private GameObject lockedSlot;
    [SerializeField] private Button unlockeButton; //for ad

    [Header("Settings")]
    [SerializeField] private Color emptyColor = new Color(0.2f, 0.2f, 0.2f, 0.5f);
    [SerializeField] private float highlightScale = 1.2f;
    [SerializeField] private float highlightDuration = 0.15f;
    [SerializeField] private float clickCooldown = 0.3f;

    private ColorVector? storedColor;
    private bool isInteractable = true;
    private Sequence highlightSequence;
    public bool hasColor;

    // IColorSource implementation
    public bool IsEmpty() => !storedColor.HasValue;
    public bool IsInteractable() => isInteractable;
    public Vector3 GetWorldPosition()
    {
        Vector2 uiPos = GetComponent<RectTransform>().position;
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(uiPos);
        worldPos.z = 0;
        Debug.Log($"ui {uiPos} to world {worldPos}");
        return worldPos;
    }
    public ColorVector PeekColor() => storedColor ?? ColorVector.Null;

    private void Awake()
    {
        ValidateComponents();
        slotButton.onClick.AddListener(OnSlotClicked);
        unlockeButton.onClick.AddListener(OnUnlockClicked);
    }
    
    private void OnDestroy()
    {
        if (slotButton != null)
            slotButton.onClick.RemoveListener(OnSlotClicked);

        highlightSequence?.Kill();
    }

    private void ValidateComponents()
    {
        if (slotImage == null) slotImage = GetComponentInChildren<Image>();
        if (slotButton == null) slotButton = GetComponentInChildren<Button>();
    }

    public void Init(bool isOpen)
    {
        if (isOpen)
        {
            lockedSlot.SetActive(false);
            isInteractable = true;
            hasColor = false;
            
            slotButton.onClick.AddListener(OnSlotClicked);
        }
        if (!isOpen)
        {
            lockedSlot.SetActive(true);
            isInteractable = false;
            
            unlockeButton.onClick.AddListener(OnUnlockClicked);
        }

        UpdateVisual();
    }

    public async void OnSlotClicked()
    {
        if (!isInteractable || IsEmpty()) return;

        isInteractable = false;
        try
        {
            await HandleSlotClick();
        }
        finally
        {
            await Task.Delay((int)(clickCooldown * 1000));
            isInteractable = true;
        }
    }
    public async void OnUnlockClicked()
    {
        if (!isInteractable || IsEmpty()) return;

        isInteractable = false;
        try
        {
            await HandleUnlockClick();
        }
        finally
        {
            await Task.Delay((int)(clickCooldown * 1000));
            isInteractable = true;
        }
    }
    private async Task HandleSlotClick()
    {
        PlayClickEffects();
        await EventBus.PublishAuto(new TileSelectionEvent(this));
    }
    private async Task HandleUnlockClick()
    {
        lockedSlot.SetActive(false);
        isInteractable = true;
        hasColor = false;
        slotButton.onClick.AddListener(OnSlotClicked);
        await Task.CompletedTask;
    }
    private void PlayClickEffects()
    {
        // Visual feedback
        highlightSequence?.Kill();
        highlightSequence = DOTween.Sequence()
            .Append(transform.DOScale(highlightScale, highlightDuration).SetEase(Ease.OutBack))
            .Append(transform.DOScale(1f, highlightDuration).SetEase(Ease.InBack));

        // // Particle effect
        // if (slotParticles != null)
        //     slotParticles.Play();
    }

    public ColorVector PopTopColor()
    {
        if (IsEmpty()) return ColorVector.Null;

        var color = storedColor.Value;
        Clear();
        return color;
    }

    public void SetColor(ColorVector color)
    {
        storedColor = color;
        UpdateVisual();
        PlayColorSetEffects();
    }

    private void PlayColorSetEffects()
    {
        // if (slotParticles != null)
        //     slotParticles.Play();
    }

    public void Clear()
    {
        storedColor = null;
        UpdateVisual();
    }

    private void UpdateVisual()
    {
        if (slotImage == null) return;

        slotImage.color = storedColor?.ToUnityColor() ?? emptyColor;
        slotImage.enabled = true;
    }

    public void SetHighlight(bool highlight)
    {
        highlightSequence?.Kill();
        transform.localScale = highlight ? Vector3.one * highlightScale : Vector3.one;
    }
    public void SetTemporarilyDisabled(bool disabled)
    {
        if (disabled)
            slotImage.color = Color.grey;
        else
            UpdateVisual(); // gerçek rengi tekrar uygular
    }

}

