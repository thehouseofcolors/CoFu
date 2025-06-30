using UnityEngine;
using DG.Tweening;
using System.Threading.Tasks;

public class TileEffectController : MonoBehaviour
{
    [Header("Animation Settings")]
    [SerializeField] private float highlightScale = 1.2f;
    [SerializeField] private float selectScale = 1.1f;
    [SerializeField] private float animationDuration = 0.15f;
    [SerializeField] private float pushScale = 0.8f;
    [SerializeField] private float popScale = 1.2f;


    public void SetHighlight(bool on)
    {
        transform.DOKill(); // Kill any ongoing animations
        transform.DOScale(on ? highlightScale : 1f, animationDuration)
            .SetEase(Ease.OutBack);
    }

    public async Task PlaySelectedEffect(SpriteRenderer spriteRenderer)
    {
        if (spriteRenderer == null) return;

        transform.DOKill();
        spriteRenderer.DOKill();

        var sequence = DOTween.Sequence()
            .Append(transform.DOScale(selectScale, animationDuration / 2))
            .Join(spriteRenderer.DOColor(
                Color.Lerp(spriteRenderer.color, Color.white, 0.3f), 
                animationDuration / 2))
            .SetLoops(2, LoopType.Yoyo)
            .SetEase(Ease.OutQuad);

        await sequence.AsyncWaitForCompletion();
    }

    public Tween PlayPopAnimation()
    {
        transform.DOKill();
        var sequence = DOTween.Sequence();
        sequence.Append(transform.DOScale(popScale, animationDuration).SetEase(Ease.OutBack));
        sequence.Append(transform.DOScale(1f, animationDuration).SetEase(Ease.InBack));
        return sequence;
    }

    public Tween PlayPushAnimation()
    {
        transform.DOKill();
        var sequence = DOTween.Sequence();
        sequence.Append(transform.DOScale(pushScale, animationDuration).SetEase(Ease.OutBack));
        sequence.Append(transform.DOScale(1f, animationDuration).SetEase(Ease.InBack));
        return sequence;
    }

}