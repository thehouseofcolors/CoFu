
using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameEvents;

[RequireComponent(typeof(SpriteRenderer))]
public class Tile : MonoBehaviour, IColorSource
{
    // Serialized Fields
    [SerializeField] private SpriteRenderer spriteRenderer;


    // Properties
    private Stack<ColorVector> colorStack = new Stack<ColorVector>();
    public bool IsEmpty() => colorStack.Count == 0;
    public bool CanSelectable { get; set; }
    public int X { get; private set; }
    public int Y { get; private set; }


    private void Awake()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
        UpdateVisual();
    }

    public void SetCoordinates(int x, int y)
    {
        X = x;
        Y = y;
    }

    public ColorVector PopTopColor()
    {
        if (IsEmpty()) return ColorVector.Null;
        var color = colorStack.Pop();
        Effects.Tiles.PlayPop(transform);
        UpdateVisual();
        return color;
    }
    public Vector3 GetWorldPosition()
    {
        return transform.position;
    }

    public void PushColor(ColorVector color)
    {
        colorStack.Push(color);
        Effects.Tiles.PlayPush(transform);
        UpdateVisual();
    }

    public ColorVector PeekColor()
    {
        if (IsEmpty())
        {
            Debug.LogWarning("Attempted to peek empty tile");
            return ColorVector.Null;
        }
        return colorStack.Peek();
    }
    public void UpdateVisual()
    {
        if (spriteRenderer == null) return;
        spriteRenderer.color = IsEmpty() ? Color.clear : PeekColor().ToUnityColor();
    }

    public void SetHighlight(bool on)
    {
        Effects.Tiles.Highlight(transform, on);
    }
    public void SetTemporarilyDisabled(bool disabled)
    {
        if (disabled)
            spriteRenderer.color = Color.gray; // veya daha soft, yarı saydam bir renk
        else
            UpdateVisual(); // gerçek rengi tekrar uygular
    }

    private bool CanBeClicked()
    {
        // Add your conditions here
        bool canClick = CanSelectable;
        Debug.Log($"CanBeClicked: {canClick}");
        return canClick;
    }

    public async void ReverseStack()
    {
        if (IsEmpty()) return;

        List<ColorVector> tempList = new List<ColorVector>();

        // Tüm renkleri tek tek Pop (ve animasyon oynat)
        while (!IsEmpty())
        {
            var color = PopTopColor(); // Animasyon içeriyor zaten
            tempList.Add(color);
            await Task.Delay(100); // pop animasyonu süresi kadar bekle (opsiyonel ayarla)
        }

        // Tersten geri Push (yani stack'i terslemiş olursun)
        for (int i = 0; i < tempList.Count; i++)
        {
            PushColor(tempList[i]); // Push animasyonu da var
            await Task.Delay(100); // push animasyonu süresi kadar bekle (opsiyonel ayarla)
        }
    }


    public async Task OnClicked()
    {
        Debug.Log("tile cliicked");
        if (!CanBeClicked()) return;

        try
        {
            CanSelectable = false; // Prevent double clicks
            await Effects.Tiles.PlaySelected(transform, spriteRenderer);
            await EventBus.PublishAuto(new TileSelectionEvent(this));
        }
        finally
        {
            CanSelectable = true;
        }
    }

}