
using System.Threading.Tasks;
using GameEvents;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;



[RequireComponent(typeof(Image), typeof(Button))]
public class SlotController : MonoBehaviour, IColorSource
{
    [Header("Components")]
    [SerializeField] private Image slotImage;
    [SerializeField] private Button slotButton;
    // [SerializeField] private ParticleSystem slotParticles;

    [Header("Settings")]
    [SerializeField] private Color emptyColor = new Color(0.2f, 0.2f, 0.2f, 0.5f);
    [SerializeField] private float highlightScale = 1.2f;
    [SerializeField] private float highlightDuration = 0.15f;
    [SerializeField] private float clickCooldown = 0.3f;

    private ColorVector? storedColor;
    private int slotIndex;
    private IntermediateSlotManager slotManager;
    private bool isInteractable = true;
    private Sequence highlightSequence;

    private void Awake()
    {
        ValidateComponents();
        slotButton.onClick.AddListener(OnSlotClicked);
    }

    private void OnDestroy()
    {
        if (slotButton != null)
            slotButton.onClick.RemoveListener(OnSlotClicked);

        highlightSequence?.Kill();
    }

    private void ValidateComponents()
    {
        if (slotImage == null) slotImage = GetComponent<Image>();
        if (slotButton == null) slotButton = GetComponent<Button>();
    }

    public void Init(int index, IntermediateSlotManager manager)
    {
        slotIndex = index;
        slotManager = manager;
        name = $"Slot_{index}";
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

    private async Task HandleSlotClick()
    {
        PlayClickEffects();
        await EventBus.PublishAuto(new TileSelectionEvent(this));
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

    public ColorVector GetColor()
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

    // IColorSource implementation
    public bool IsEmpty() => !storedColor.HasValue;
    public Vector3 GetPosition() => transform.position;
    public ColorVector PeekColor() => storedColor ?? ColorVector.Null;
}

