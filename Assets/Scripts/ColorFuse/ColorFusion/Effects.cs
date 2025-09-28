/// <summary>
/// Centralized static manager for UI, gameplay, camera, tile, and text animation effects.
/// 
/// 📌 Modules:
/// - PanelTransition: Handles panel show/hide transitions (fade, slide, scale, etc.)
/// - Buttons: Manages button click/bounce feedback
/// - Camera: Triggers camera shake
/// - Gameplay: Controls movement, merge, and error feedback effects
/// - Tiles: Manages tile highlight, select, pop, and push animations
/// - Texts: Applies bounce effect to texts
/// 
/// 🛠 Uses:
/// - DOTween for tweening/animations
/// - Task-based async methods for awaitable effects
/// - EffectParams for runtime effect configuration
/// 
/// ⚠️ This class is static and finalized. It will not be extended or modified.
/// may add camera bounce
/// 
/// </summary>

using DG.Tweening;
using System.Threading.Tasks;
using UnityEngine;

public static class Effects
{
    public static class PanelTransition
    {
        public static Task Fade(CanvasGroup target, bool fadeIn, float duration = 0.5f)
            => TransitionEffectManager.Fade(target, fadeIn, duration);

        public static Task Slide(RectTransform rect, Vector2 direction, bool slideIn, float duration = 0.5f)
            => TransitionEffectManager.Slide(rect, direction, slideIn, duration);

        public static Task SlidePanelSwap(
            RectTransform currentPanel,
            RectTransform nextPanel,
            Vector2 direction,
            float duration = 0.5f)
            => TransitionEffectManager.SlidePanelSwap(currentPanel, nextPanel, direction, duration);
        public static Task FadePanelSwap(
            CanvasGroup currentPanel,
            CanvasGroup nextPanel,
            float duration = 0.5f)
            => TransitionEffectManager.FadePanelSwap(currentPanel, nextPanel, duration);

        public static Task FadeScale(CanvasGroup panel, bool fadeIn, float duration = 0.3f)
            => TransitionEffectManager.FadeScale(panel, fadeIn, duration);
        public static Task Scale(RectTransform panel, bool scaleIn, float duration = 0.4f)
            => TransitionEffectManager.Scale(panel, scaleIn, duration);
    }
    public static class Buttons
    {
        public static Task PlayClick(Transform buttonTransform, float duration = 0.2f)
            => ButtonEffectManager.PlayClick(buttonTransform, duration);

        public static Tween PlayBounce(Transform buttonTransform, float scaleAmount = 1.1f, float duration = 0.6f)
            => ButtonEffectManager.PlayBounce(buttonTransform, scaleAmount, duration);
    }
    public static class Camera
    {
        public static void Shake(Transform cameraTransform,float duration = 0.3f, float strength = 0.3f) 
            => CameraEffectManager.Shake(cameraTransform, duration, strength);
    }
    public static class Gameplay
    {
        public static Task PlayMergeEffect(Transform transform, float punchScale = 0.2f, float duration = 0.3f)
            => GameplayEffectManager.PlayMergeEffect(transform, punchScale, duration);
        public static async Task PlayMoveEffectAsync(EffectParams effectParams)
        => await GameplayEffectManager.PlayMoveEffectAsync(effectParams);

        public static async Task<GameObject> PlayMergeEffectAsync(EffectParams effectParams)
        => await GameplayEffectManager.PlayMergeEffectAsync(effectParams);


        public static async Task PlayMergeFailEffectAsync(GameObject mergeEffect)
        => await GameplayEffectManager.PlayMergeFailEffectAsync(mergeEffect);


        public static async Task PlayMoveEffectAsync(GameObject effectGO, Vector3 target)
        => await GameplayEffectManager.PlayMoveEffectAsync(effectGO, target);



    }
    public static class Tiles
    {
        public static void Highlight(Transform tileTransform, bool on, float scale = 1.2f, float duration = 0.15f)
            => TileEffectManager.SetHighlight(tileTransform, on, scale, duration);

        public static async Task PlaySelected(Transform tileTransform, SpriteRenderer spriteRenderer, float scale = 1.1f, float duration = 0.15f)
            => await TileEffectManager.PlaySelectedEffect(tileTransform, spriteRenderer, scale, duration);

        public static Tween PlayPop(Transform tileTransform, float scale = 1.2f, float duration = 0.15f)
            => TileEffectManager.PlayPopAnimation(tileTransform, scale, duration);

        public static Tween PlayPush(Transform tileTransform, float scale = 0.8f, float duration = 0.15f)
            => TileEffectManager.PlayPushAnimation(tileTransform, scale, duration);
    }
    public static class Texts
    {
        public static Tween Bounce(Transform target)
        => TextEffectManager.Bounce(target);
        
    }
}
public static class TransitionEffectManager
{
    private const float DefaultSlideDistance = 1000f;
    private const float DefaultScaleAmount = 1.2f;

    public static async Task Fade(CanvasGroup target, bool fadeIn, float duration = 0.5f)
    {
        if (target == null) return;
        
        target.alpha = fadeIn ? 0f : 1f;
        target.gameObject.SetActive(true);
        
        await target.DOFade(fadeIn ? 1f : 0f, duration)
                   .SetEase(fadeIn ? Ease.OutQuad : Ease.InQuad)
                   .AsyncWaitForCompletion();

        // if (!fadeIn) target.gameObject.SetActive(false);
    }

    public static async Task Slide(RectTransform rect, Vector2 direction, bool slideIn, float duration = 0.5f)
    {
        if (rect == null) return;

        Vector2 targetPos = rect.anchoredPosition;
        Vector2 offscreenPos = targetPos - (direction.normalized * DefaultSlideDistance);

        if (slideIn)
        {
            rect.anchoredPosition = offscreenPos;
            rect.gameObject.SetActive(true);
        }

        await rect.DOAnchorPos(slideIn ? targetPos : offscreenPos, duration)
                 .SetEase(slideIn ? Ease.OutBack : Ease.InBack)
                 .AsyncWaitForCompletion();

        // if (!slideIn) rect.gameObject.SetActive(false);
    }

    public static async Task SlidePanelSwap(
        RectTransform currentPanel,
        RectTransform nextPanel,
        Vector2 direction,
        float duration = 0.5f)
    {
        if (currentPanel == null || nextPanel == null) return;

        Vector2 offset = direction.normalized * DefaultSlideDistance;
        nextPanel.anchoredPosition = currentPanel.anchoredPosition + offset;
        nextPanel.gameObject.SetActive(true);

        var outTween = currentPanel.DOAnchorPos(currentPanel.anchoredPosition - offset, duration)
                      .SetEase(Ease.InOutCubic);
        
        var inTween = nextPanel.DOAnchorPos(currentPanel.anchoredPosition, duration)
                     .SetEase(Ease.InOutCubic);

        await Task.WhenAll(outTween.AsyncWaitForCompletion(), inTween.AsyncWaitForCompletion());
        
        // currentPanel.gameObject.SetActive(false);
    }

    public static async Task Scale(RectTransform panel, bool scaleIn, float duration = 0.4f)
    {
        if (panel == null) return;

        if (scaleIn)
        {
            panel.localScale = Vector3.zero;
            panel.gameObject.SetActive(true);
        }

        await panel.DOScale(scaleIn ? Vector3.one : Vector3.zero, duration)
                  .SetEase(scaleIn ? Ease.OutBack : Ease.InBack)
                  .AsyncWaitForCompletion();

        // if (!scaleIn) panel.gameObject.SetActive(false);
    }

    public static async Task FadePanelSwap(
        CanvasGroup currentPanel,
        CanvasGroup nextPanel,
        float duration = 0.5f)
    {
        if (currentPanel == null || nextPanel == null) return;

        nextPanel.alpha = 0f;
        nextPanel.gameObject.SetActive(true);
        nextPanel.blocksRaycasts = false;

        var fadeOut = currentPanel.DOFade(0f, duration).SetEase(Ease.OutQuad);
        var fadeIn = nextPanel.DOFade(1f, duration).SetEase(Ease.OutQuad);

        await Task.WhenAll(fadeOut.AsyncWaitForCompletion(), fadeIn.AsyncWaitForCompletion());

        currentPanel.gameObject.SetActive(false);
        nextPanel.blocksRaycasts = true;
    }

    public static async Task FadeScale(CanvasGroup panel, bool fadeIn, float duration = 0.3f)
    {
        if (panel == null) return;

        panel.alpha = fadeIn ? 0f : 1f;
        panel.transform.localScale = fadeIn ? Vector3.one * DefaultScaleAmount : Vector3.one;
        panel.gameObject.SetActive(true);
        panel.blocksRaycasts = false;

        var fade = panel.DOFade(fadeIn ? 1f : 0f, duration).SetEase(Ease.OutQuad);
        var scale = panel.transform.DOScale(fadeIn ? Vector3.one : Vector3.one * DefaultScaleAmount, duration)
                   .SetEase(fadeIn ? Ease.OutBack : Ease.InBack);

        await Task.WhenAll(fade.AsyncWaitForCompletion(), scale.AsyncWaitForCompletion());

        panel.blocksRaycasts = fadeIn;
        // if (!fadeIn) panel.gameObject.SetActive(false);
    }
}
public static class ButtonEffectManager
{
    private const string BounceTweenId = "ButtonBounceTween";

    public static async Task PlayClick(Transform buttonTransform, float duration = 0.2f)
    {
        if (buttonTransform == null) return;

        DOTween.Kill(BounceTweenId);
        buttonTransform.DOKill();
        buttonTransform.localScale = Vector3.one;

        await buttonTransform.DOPunchScale(Vector3.one * 0.1f, duration)
                            .AsyncWaitForCompletion();
    }

    // public static void PlayBounce(Transform buttonTransform, float scaleAmount = 1.1f, float duration = 0.6f)
    // {
    //     if (buttonTransform == null) return;

    //     buttonTransform.DOKill();
    //     buttonTransform.localScale = Vector3.one;

    //     buttonTransform.DOScale(scaleAmount, duration)
    //                   .SetLoops(-1, LoopType.Yoyo)
    //                   .SetEase(Ease.InOutQuad)
    //                   .SetId(BounceTweenId);
    // }
    public static Tween PlayBounce(Transform target, float scaleAmount = 1.1f, float duration = 0.6f)
    {
        if (target == null) return null;

        target.DOKill();
        target.localScale = Vector3.one;

        return target.DOScale(scaleAmount, duration)
                     .SetLoops(-1, LoopType.Yoyo)
                     .SetEase(Ease.InOutQuad)
                     .SetId(BounceTweenId);
    }


    // public static Tween PlayBounceLoop(Transform target, float scaleAmount = 1.05f, float duration = 0.5f)
    // {
    //     if (target == null) return null;

    //     target.localScale = Vector3.one;

    //     return target.DOScale(Vector3.one * scaleAmount, duration)
    //                  .SetEase(Ease.InOutSine)
    //                  .SetLoops(-1, LoopType.Yoyo);
    // }

    // public static void StopBounce(Tween bounceTween, Transform target)
    // {
    //     if (bounceTween != null && bounceTween.IsActive())
    //     {
    //         bounceTween.Kill();
    //     }

    //     if (target != null)
    //     {
    //         target.localScale = Vector3.one;
    //     }
    // }

}
public static class TextEffectManager
{
    public static Tween Bounce(Transform target, 
                              float scaleMultiplier = 1.2f, 
                              float duration = 0.3f, 
                              Ease easeType = Ease.OutBounce)
    {
        if (target == null)
        {
            Debug.LogWarning("Bounce target is null!");
            return null;
        }

        // Kill existing animations on this transform
        target.DOKill(true);
        
        // Reset scale before starting new animation
        target.localScale = Vector3.one;

        // Create and return the bounce tween
        return target.DOScale(Vector3.one * scaleMultiplier, duration)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(easeType)
            .OnKill(() => {
                // Reset scale when animation is killed
                if (target != null)
                    target.localScale = Vector3.one;
            });
    }
}
public static class CameraEffectManager
{
    public static void Shake(Transform cameraTransform,float duration = 0.3f, float strength = 0.3f)
    {
        if (cameraTransform == null)
        {
            Debug.LogWarning("Camera not initialized. Call Initialize() first.");
            return;
        }

        cameraTransform.DOShakePosition(duration, strength, 10, 90, true);
    }
}
public static class GameplayEffectManager
{
    // Animation configuration
    private const float DefaultMoveDuration = 2f;
    private const float DefaultMergeDuration = 1f;
    private const float DefaultFailDuration = 0.4f;
    private const float MinMoveDuration = 0.15f;
    private const float DefaultPunchScale = 0.2f;
    private const float DefaultEffectDuration = 0.3f;
    private const float MergeScaleMultiplier = 1.5f;
    private const float ColorIntensity = 1.2f;

    public static async Task PlayMergeEffect(Transform transform, float punchScale = DefaultPunchScale, float duration = DefaultEffectDuration)
    {
        if (transform == null) return;

        await transform.DOPunchScale(Vector3.one * punchScale, duration)
                      .AsyncWaitForCompletion();
    }

    public static async Task PlayMoveEffect(Transform transform, float punchScale = DefaultPunchScale, float duration = DefaultEffectDuration)
    {
        if (transform == null) return;

        await transform.DOPunchScale(Vector3.one * punchScale, duration)
                      .AsyncWaitForCompletion();
    }

    public static async Task PlayMoveEffectAsync(EffectParams effectParams)
    {
        if (effectParams == null || effectParams.Prefab == null) return;

        GameObject effectGO = CreateEffectObject(effectParams);
        float duration = CalculateMoveDuration(effectParams.StartPos, effectParams.EndPos, effectParams.Duration);

        var sequence = BuildMoveSequence(effectGO, effectParams.EndPos, duration);
        await CompleteAndCleanup(sequence, effectGO);
    }

    public static async Task<GameObject> PlayMergeEffectAsync(EffectParams effectParams)
    {
        if (effectParams == null || effectParams.Prefab == null) return null;

        GameObject effectGO = CreateEffectObject(effectParams);
        float duration = effectParams.Duration > 0 ? effectParams.Duration : DefaultMergeDuration;

        var sequence = BuildMergeSequence(effectGO, effectParams.Color.ToUnityColor(), duration);
        await sequence.AsyncWaitForCompletion();
        return effectGO;
    }

    public static async Task PlayMergeFailEffectAsync(GameObject mergeEffect)
    {
        if (mergeEffect == null) return;

        var sequence = BuildFailSequence(mergeEffect);
        await sequence.AsyncWaitForCompletion();
        SafeDestroy(mergeEffect);
    }

    public static async Task PlayMoveEffectAsync(GameObject effectGO, Vector3 target)
    {
        if (effectGO == null) return;

        float duration = CalculateMoveDuration(effectGO.transform.position, target);
        var sequence = BuildMoveSequence(effectGO, target, duration);
        await sequence.AsyncWaitForCompletion();
    }

    private static GameObject CreateEffectObject(EffectParams effectParams)
    {
        GameObject effectGO = Object.Instantiate(effectParams.Prefab, effectParams.StartPos, Quaternion.identity);
        var sr = effectGO.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.color = effectParams.Color.ToUnityColor();
        }
        return effectGO;
    }

    private static float CalculateMoveDuration(Vector3 start, Vector3 end, float overrideDuration = -1)
    {
        if (overrideDuration > 0) return overrideDuration;
        float distance = Vector3.Distance(start, end);
        return Mathf.Clamp(distance, MinMoveDuration, DefaultMoveDuration);
    }

    private static Sequence BuildMergeSequence(GameObject effectGO, Color color, float duration)
    {
        var sr = effectGO.GetComponent<SpriteRenderer>();
        var t = effectGO.transform;

        return DOTween.Sequence()
            .Append(t.DOScale(MergeScaleMultiplier, duration).SetEase(Ease.OutBack))
            .Append(t.DOScale(1f, duration * 0.5f))
            .Join(sr.DOColor(color * ColorIntensity, duration).SetLoops(2, LoopType.Yoyo));
    }

    private static Sequence BuildFailSequence(GameObject effectGO)
    {
        var sr = effectGO.GetComponent<SpriteRenderer>();
        var t = effectGO.transform;

        return DOTween.Sequence()
            .Append(t.DOShakePosition(
                duration: DefaultFailDuration,
                strength: new Vector3(0.3f, 0.3f, 0f),
                vibrato: 10,
                randomness: 90,
                fadeOut: true))
            .Join(sr.DOColor(Color.red, DefaultFailDuration * 0.5f).SetLoops(2, LoopType.Yoyo))
            .Join(sr.DOFade(0f, DefaultFailDuration).SetEase(Ease.OutSine));
    }

    private static Sequence BuildMoveSequence(GameObject effectGO, Vector3 endPos, float duration)
    {
        var sr = effectGO.GetComponent<SpriteRenderer>();
        var t = effectGO.transform;

        return DOTween.Sequence()
            .Append(t.DOMove(endPos, duration).SetEase(Ease.InOutSine))
            .Join(t.DOScale(0.5f, duration).SetEase(Ease.OutQuad))
            .Join(sr.DOFade(0.8f, duration * 0.5f).SetLoops(2, LoopType.Yoyo));
    }

    private static async Task CompleteAndCleanup(Sequence sequence, GameObject effectGO)
    {
        await sequence.AsyncWaitForCompletion();
        SafeDestroy(effectGO);
    }

    private static void SafeDestroy(GameObject obj)
    {
        if (obj != null)
        {
            Object.Destroy(obj);
        }
    }
}
public static class TileEffectManager
{
    public static void SetHighlight(Transform tileTransform, bool on, float scale, float duration)
    {
        if (tileTransform == null) return;

        tileTransform.DOKill();
        tileTransform.DOScale(on ? scale : 1f, duration)
            .SetEase(Ease.OutBack);
    }

    public static async Task PlaySelectedEffect(Transform tileTransform, SpriteRenderer spriteRenderer, float scale, float duration)
    {
        if (tileTransform == null || spriteRenderer == null) return;

        tileTransform.DOKill();
        spriteRenderer.DOKill();

        var sequence = DOTween.Sequence()
            .Append(tileTransform.DOScale(scale, duration / 2))
            .Join(spriteRenderer.DOColor(
                Color.Lerp(spriteRenderer.color, Color.white, 0.3f),
                duration / 2))
            .SetLoops(2, LoopType.Yoyo)
            .SetEase(Ease.OutQuad);

        await sequence.AsyncWaitForCompletion();
    }

    public static Tween PlayPopAnimation(Transform tileTransform, float scale, float duration)
    {
        tileTransform.DOKill();
        var sequence = DOTween.Sequence().SetTarget(tileTransform); 
        sequence.Append(tileTransform.DOScale(scale, duration).SetEase(Ease.OutBack));
        sequence.Append(tileTransform.DOScale(1f, duration).SetEase(Ease.InBack));
        return sequence;
    }

    public static Tween PlayPushAnimation(Transform tileTransform, float scale, float duration)
    {
        tileTransform.DOKill();
        var sequence = DOTween.Sequence();
        sequence.Append(tileTransform.DOScale(scale, duration).SetEase(Ease.OutBack));
        sequence.Append(tileTransform.DOScale(1f, duration).SetEase(Ease.InBack));
        return sequence;
    }
}

public class EffectParams
{
    public GameObject Prefab { get; set; }
    public ColorVector Color { get; set; }
    public Vector3 StartPos { get; set; }
    public Vector3 EndPos { get; set; }
    public float Duration { get; set; } = -1f;

    public EffectParams() { }

    public EffectParams(GameObject prefab, ColorVector color, Vector3 startPos, Vector3 endPos, float duration = -1f)
    {
        Prefab = prefab;
        Color = color;
        StartPos = startPos;
        EndPos = endPos;
        Duration = duration;
    }
}
