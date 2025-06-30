// using UnityEngine;
// using DG.Tweening;
// using System.Threading.Tasks;
// using UnityEngine.UI;

// // // public class EffectManager : MonoBehaviour
// // // {
// // //     [SerializeField] private GameObject transferEffectPrefab;
// // //     [SerializeField] private GameObject whiteEffectPrefab;
// // //     [SerializeField] private RectTransform uiCanvas;
// // //     [SerializeField] private RectTransform scoreTarget;

// // //     public void PlayTransferEffect(Vector3 from, Vector3 to, Color color, float duration)
// // //     {
// // //         var effect = Instantiate(transferEffectPrefab, from, Quaternion.identity);
// // //         var spriteRenderer = effect.GetComponent<SpriteRenderer>();
// // //         spriteRenderer.color = color;
// // //         spriteRenderer.sortingOrder = 10;

// // //         DOTween.Sequence()
// // //             .Append(effect.transform.DOMove(to, duration))
// // //             .Join(effect.transform.DOScale(1.2f, duration / 2))
// // //             .Append(effect.transform.DOScale(0f, duration / 2))
// // //             .SetEase(Ease.InOutSine)
// // //             .OnComplete(() => Destroy(effect));
// // //     }

// // //     public void PlayWhiteFusionUIEffect(Vector3 worldPos)
// // //     {
// // //         Vector2 screenPos = Camera.main.WorldToScreenPoint(worldPos);
// // //         var uiEffect = Instantiate(whiteEffectPrefab, uiCanvas);
// // //         uiEffect.GetComponent<RectTransform>().position = screenPos;

// // //         var script = uiEffect.GetComponent<WhiteFusionEffectUI>();
// // //         script.SetTarget(scoreTarget);
// // //     }

// // //     public void PlayWhiteEffect(Vector3 position)
// // //     {
// // //         var effect = Instantiate(whiteEffectPrefab, position, Quaternion.identity);
// // //         Destroy(effect, 1f);
// // //     }
// // // }



// // public class EffectManager : MonoBehaviour
// // {
// //     [SerializeField] private GameObject worldcolorEffectPrefab;
// //     [SerializeField] private GameObject UIEffectPrefab;
// //     [SerializeField] private RectTransform uiCanvas;


// //     private const float moveDuration = 0.5f;
// //     private const float mergeDuration = 0.3f;
// //     private const float finalMoveDuration = 0.5f;

// //     // Tek bir renk efektini spawn edip baştan sona hareket ettirir
// //     public async Task SpawnAndMoveColorEffect(ColorVector color, Vector3 startPos)
// //     {
// //         var effectGO = Instantiate(worldcolorEffectPrefab, startPos, Quaternion.identity);
// //         var sr = effectGO.GetComponent<SpriteRenderer>();
// //         sr.color = color.ToUnityColor();
// //         sr.sortingOrder = 10;
// //         Vector3 endPos = Vector3.zero;
// //         await effectGO.transform.DOMove(endPos, moveDuration).SetEase(Ease.InOutSine).AsyncWaitForCompletion();

// //         // Efekt burada kalabilir veya yok edilebilir
// //         Destroy(effectGO);
// //     }
// //     public async Task SpawnAndMoveUIEffect(ColorVector color, Vector3 startPos)
// //     {
// //         var uiEffect = Instantiate(UIEffectPrefab, uiCanvas);
// //         var rt = uiEffect.GetComponent<RectTransform>();
// //         var img = uiEffect.GetComponent<Image>();
// //         img.color = color.ToUnityColor();

// //         rt.position = startPos; // Bu zaten screen space
// //         Vector3 endPos = Vector3.zero; // Merge pozisyonunu dışarıdan ver istersen

// //         await rt.DOMove(endPos, moveDuration).SetEase(Ease.InOutSine).AsyncWaitForCompletion();

// //         Destroy(uiEffect);
// //     }

// //     public async Task ReverseSpawnAndMoveColorEffect(ColorVector color, Vector3 startPos)
// //     {
// //         Vector3 endPos = Vector3.zero;
// //         var effectGO = Instantiate(worldcolorEffectPrefab, endPos, Quaternion.identity);
// //         var sr = effectGO.GetComponent<SpriteRenderer>();
// //         sr.color = color.ToUnityColor();
// //         sr.sortingOrder = 10;
// //         await effectGO.transform.DOMove(startPos, moveDuration).SetEase(Ease.InOutSine).AsyncWaitForCompletion();

// //         // Efekt burada kalabilir veya yok edilebilir
// //         Destroy(effectGO);
// //     }



// //     public async Task MoveEffectToUI(ColorVector color, Vector2 uiAnchorPos)
// //     {
// //         var effectGO = Instantiate(UIEffectPrefab, uiCanvas);
// //         var rt = effectGO.GetComponent<RectTransform>();
// //         var img = effectGO.GetComponentInChildren<Image>();
// //         img.color = color.ToUnityColor();

// //         rt.anchoredPosition = Vector2.zero;

// //         await rt.DOAnchorPos(uiAnchorPos, finalMoveDuration).SetEase(Ease.OutBack).AsyncWaitForCompletion();

// //         Destroy(effectGO);
// //     }

// //     public enum EffectSpace
// //     {
// //         World,
// //         UI
// //     }

// //     public class EffectParams
// //     {
// //         public ColorVector Color;
// //         public Vector3 StartPos;
// //         public Vector3 EndPos;
// //         public EffectSpace Space = EffectSpace.World;
// //         public RectTransform ParentUI; // sadece UI için
// //     }

// //     public async Task PlayEffectAsync(EffectParams effectParams)
// //     {
// //         GameObject effectGO;

// //         if (effectParams.Space == EffectSpace.World)
// //         {
// //             effectGO = Instantiate(worldcolorEffectPrefab, effectParams.StartPos, Quaternion.identity);
// //             var sr = effectGO.GetComponent<SpriteRenderer>();
// //             sr.color = effectParams.Color.ToUnityColor();
// //             sr.sortingOrder = 10;

// //             await effectGO.transform.DOMove(effectParams.EndPos, moveDuration).SetEase(Ease.InOutSine).AsyncWaitForCompletion();
// //         }
// //         else // UI
// //         {
// //             var canvas = effectParams.ParentUI ?? uiCanvas;
// //             effectGO = Instantiate(UIEffectPrefab, canvas);
// //             var rt = effectGO.GetComponent<RectTransform>();
// //             var img = effectGO.GetComponentInChildren<Image>();
// //             img.color = effectParams.Color.ToUnityColor();

// //             rt.position = effectParams.StartPos;

// //             await rt.DOMove(effectParams.EndPos, moveDuration).SetEase(Ease.OutBack).AsyncWaitForCompletion();
// //         }

// //         Destroy(effectGO);
// //     }


// // }



// // public class EffectManager : Singleton<EffectManager>
// // {    private const float moveDuration = 0.5f;

// //     public async Task PlayMoveEffectAsync(EffectParams effectParams)
// //     {
// //         GameObject effectGO;
// //         effectGO = Instantiate(effectParams.Prefab, effectParams.StartPos, Quaternion.identity);
// //         var sr = effectGO.GetComponent<SpriteRenderer>();
// //         sr.color = effectParams.Color.ToUnityColor();
// //         var t = effectGO.transform;

// //         t.localScale = Vector3.one;

// //         await Task.WhenAll(
// //             t.DOMove(effectParams.EndPos, moveDuration).SetEase(Ease.InOutSine).AsyncWaitForCompletion(),
// //             t.DOScale(0.5f, moveDuration).SetEase(Ease.OutQuad).AsyncWaitForCompletion()
// //         );



// //         // Destroy(effectGO);
// //     }

// //     public async Task PlayMergeEffectAsync(EffectParams effectParams)
// //     {
// //         GameObject effectGO;
// //         effectGO = Instantiate(effectParams.Prefab, Vector3.zero, Quaternion.identity);
// //         var sr = effectGO.GetComponent<SpriteRenderer>();
// //         sr.color = effectParams.Color.ToUnityColor();
// //         var t = effectGO.transform;

// //         t.localScale = Vector3.zero;

// //         await t.DOScale(1f, 0.3f).SetEase(Ease.OutQuad).AsyncWaitForCompletion();



// //         // Destroy(effectGO);
// //     }

// // }

// public static class EffectManager
// {
//     private const float moveDuration = 0.5f;

//     public static async Task PlayMoveEffectAsync(EffectParams effectParams)
//     {
//         GameObject effectGO = Object.Instantiate(effectParams.Prefab, effectParams.StartPos, Quaternion.identity);
//         var sr = effectGO.GetComponent<SpriteRenderer>();
//         sr.color = effectParams.Color.ToUnityColor();
//         var t = effectGO.transform;

//         t.localScale = Vector3.one;

//         await Task.WhenAll(
//             t.DOMove(effectParams.EndPos, moveDuration).SetEase(Ease.InOutSine).AsyncWaitForCompletion(),
//             t.DOScale(0.5f, moveDuration).SetEase(Ease.OutQuad).AsyncWaitForCompletion()
//         );

//         // Object.Destroy(effectGO);
//     }

//     public static async Task PlayMergeEffectAsync(EffectParams effectParams)
//     {
//         GameObject effectGO = Object.Instantiate(effectParams.Prefab, Vector3.zero, Quaternion.identity);
//         var sr = effectGO.GetComponent<SpriteRenderer>();
//         sr.color = effectParams.Color.ToUnityColor();
//         var t = effectGO.transform;

//         t.localScale = Vector3.zero;

//         await t.DOScale(1f, 0.3f).SetEase(Ease.OutQuad).AsyncWaitForCompletion();

//         // Object.Destroy(effectGO);
//     }
//     public static async Task PlayMergeFailEffectAsync(GameObject mergeEffect)
//     {
//         if (mergeEffect == null) return;

//         var sr = mergeEffect.GetComponent<SpriteRenderer>();
//         var t = mergeEffect.transform;

//         // Emin olmak için tam opak yap
//         var color = sr.color;
//         sr.color = new Color(color.r, color.g, color.b, 1f);

//         // Shake ve Fade Out paralel oynasın
//         var shake = t.DOShakePosition(
//             duration: 0.4f,
//             strength: new Vector3(0.2f, 0.2f, 0f),
//             vibrato: 8,
//             randomness: 60,
//             fadeOut: true
//         ).AsyncWaitForCompletion();

//         var fade = sr.DOFade(0f, 0.4f).SetEase(Ease.OutSine).AsyncWaitForCompletion();

//         await Task.WhenAll(shake, fade);

//         // Object.Destroy(mergeEffect);
//     }
//     public static async Task PlayMoveEffectAsync(GameObject effectGO, Vector3 target)
//     {
//         var sr = effectGO.GetComponent<SpriteRenderer>();
//         var t = effectGO.transform;

//         t.localScale = Vector3.one;

//         await Task.WhenAll(
//             t.DOMove(target, moveDuration).SetEase(Ease.InOutSine).AsyncWaitForCompletion(),
//             t.DOScale(0.5f, moveDuration).SetEase(Ease.OutQuad).AsyncWaitForCompletion()
//         );

//         // Object.Destroy(effectGO);
//     }
// }


using UnityEngine;
using DG.Tweening;
using System.Threading.Tasks;

public static class EffectManager
{
    private const float DefaultMoveDuration = 2f;
    private const float DefaultMergeDuration = 1f;
    private const float DefaultFailDuration = 0.4f;

    public static async Task PlayMoveEffectAsync(EffectParams effectParams)
    {
        GameObject effectGO = Object.Instantiate(effectParams.Prefab, effectParams.StartPos, Quaternion.identity);
        var sr = effectGO.GetComponent<SpriteRenderer>();
        sr.color = effectParams.Color.ToUnityColor();

        float distance = Vector3.Distance(effectParams.StartPos, effectParams.EndPos);
        float duration = Mathf.Clamp(distance , 0.15f, DefaultMoveDuration);


        // LineRenderer objesi oluşturuyoruz
        GameObject lineGO = new GameObject("EffectPathLine");
        var lineRenderer = lineGO.AddComponent<LineRenderer>();

        // LineRenderer ayarları (kendi ihtiyaçlarına göre düzenleyebilirsin)
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, effectParams.StartPos);
        lineRenderer.SetPosition(1, effectParams.EndPos);

        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;

        // Materyal ve renk ayarı (varsa kendi materyalini kullanabilirsin)
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = sr.color;
        lineRenderer.endColor = sr.color;


        var sequence = DOTween.Sequence()
            .Append(effectGO.transform.DOMove(effectParams.EndPos, duration).SetEase(Ease.InOutSine))
            .Join(effectGO.transform.DOScale(0.1f, duration).SetEase(Ease.OutQuad))
            .Join(sr.DOFade(0.8f, duration * 0.5f).SetLoops(2, LoopType.Yoyo));

        await sequence.AsyncWaitForCompletion();

        Object.Destroy(effectGO);
    }

    public static async Task<GameObject> PlayMergeEffectAsync(EffectParams effectParams)
    {
        GameObject effectGO = Object.Instantiate(effectParams.Prefab, Vector3.zero, Quaternion.identity);
        var sr = effectGO.GetComponent<SpriteRenderer>();
        sr.color = effectParams.Color.ToUnityColor();

        var sequence = DOTween.Sequence()
            .Append(effectGO.transform.DOScale(1.5f, DefaultMergeDuration).SetEase(Ease.OutBack))
            .Append(effectGO.transform.DOScale(1f, DefaultMergeDuration * 0.5f))
            .Join(sr.DOColor(effectParams.Color.ToUnityColor() * 1.2f, DefaultMergeDuration).SetLoops(2, LoopType.Yoyo));

        await sequence.AsyncWaitForCompletion();
        return effectGO;
    }

    public static async Task PlayMergeFailEffectAsync(GameObject mergeEffect)
    {
        if (mergeEffect == null) return;

        var sr = mergeEffect.GetComponent<SpriteRenderer>();
        var originalColor = sr.color;
        var t = mergeEffect.transform;

        var sequence = DOTween.Sequence()
            .Append(t.DOShakePosition(
                duration: DefaultFailDuration,
                strength: new Vector3(0.3f, 0.3f, 0f),
                vibrato: 10,
                randomness: 90,
                fadeOut: true))
            .Join(sr.DOColor(Color.red, DefaultFailDuration * 0.5f).SetLoops(2, LoopType.Yoyo))
            .Join(sr.DOFade(0f, DefaultFailDuration).SetEase(Ease.OutSine));

        await sequence.AsyncWaitForCompletion();
    }

    public static async Task PlayMoveEffectAsync(GameObject effectGO, Vector3 target)
    {
        var sr = effectGO.GetComponent<SpriteRenderer>();
        var originalColor = sr.color;
        var t = effectGO.transform;
        
        float distance = Vector3.Distance(Vector3.zero, target);
        float duration = Mathf.Clamp(distance, 0.15f, DefaultMoveDuration);


        // LineRenderer objesi oluşturuyoruz
        GameObject lineGO = new GameObject("EffectPathLine");
        var lineRenderer = lineGO.AddComponent<LineRenderer>();

        // LineRenderer ayarları (isteğine göre özelleştirilebilir)
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, t.position);

        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;

        // Materyal ve renk ayarı
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = originalColor;
        lineRenderer.endColor = originalColor;


        var sequence = DOTween.Sequence()
            .Append(effectGO.transform.DOMove(target, duration).SetEase(Ease.InOutSine))
            .Join(effectGO.transform.DOScale(0.5f, duration).SetEase(Ease.OutQuad))
            .Join(sr.DOColor(originalColor * 1.3f, duration * 0.5f).SetLoops(2, LoopType.Yoyo));

        await sequence.AsyncWaitForCompletion();
    }

    // public static async Task PlayPopEffect(GameObject target, float duration = 0.2f)
    // {
    //     var originalScale = target.transform.localScale;
    //     await target.transform.DOPunchScale(originalScale * 0.2f, duration, 2, 0.5f)
    //         .AsyncWaitForCompletion();
    // }
}

public class EffectParams
{
    public GameObject Prefab;
    public ColorVector Color;
    public Vector3 StartPos;
    public Vector3 EndPos;
    public float Duration = -1f; // Use default if -1
}

