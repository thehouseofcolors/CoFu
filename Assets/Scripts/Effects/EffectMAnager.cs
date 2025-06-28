using UnityEngine;
using DG.Tweening;
using System.Threading.Tasks;
using UnityEngine.UI;

// public class EffectManager : MonoBehaviour
// {
//     [SerializeField] private GameObject transferEffectPrefab;
//     [SerializeField] private GameObject whiteEffectPrefab;
//     [SerializeField] private RectTransform uiCanvas;
//     [SerializeField] private RectTransform scoreTarget;

//     public void PlayTransferEffect(Vector3 from, Vector3 to, Color color, float duration)
//     {
//         var effect = Instantiate(transferEffectPrefab, from, Quaternion.identity);
//         var spriteRenderer = effect.GetComponent<SpriteRenderer>();
//         spriteRenderer.color = color;
//         spriteRenderer.sortingOrder = 10;

//         DOTween.Sequence()
//             .Append(effect.transform.DOMove(to, duration))
//             .Join(effect.transform.DOScale(1.2f, duration / 2))
//             .Append(effect.transform.DOScale(0f, duration / 2))
//             .SetEase(Ease.InOutSine)
//             .OnComplete(() => Destroy(effect));
//     }

//     public void PlayWhiteFusionUIEffect(Vector3 worldPos)
//     {
//         Vector2 screenPos = Camera.main.WorldToScreenPoint(worldPos);
//         var uiEffect = Instantiate(whiteEffectPrefab, uiCanvas);
//         uiEffect.GetComponent<RectTransform>().position = screenPos;

//         var script = uiEffect.GetComponent<WhiteFusionEffectUI>();
//         script.SetTarget(scoreTarget);
//     }

//     public void PlayWhiteEffect(Vector3 position)
//     {
//         var effect = Instantiate(whiteEffectPrefab, position, Quaternion.identity);
//         Destroy(effect, 1f);
//     }
// }



public class EffectManager : MonoBehaviour
{
    [SerializeField] private GameObject worldcolorEffectPrefab;
    [SerializeField] private GameObject UIEffectPrefab;
    [SerializeField] private RectTransform uiCanvas;
    

    private const float moveDuration = 0.5f;
    private const float mergeDuration = 0.3f;
    private const float finalMoveDuration = 0.5f;

    // Tek bir renk efektini spawn edip baştan sona hareket ettirir
    public async Task SpawnAndMoveColorEffect(ColorVector color, Vector3 startPos)
    {
        var effectGO = Instantiate(worldcolorEffectPrefab, startPos, Quaternion.identity);
        var sr = effectGO.GetComponent<SpriteRenderer>();
        sr.color = color.ToUnityColor();
        sr.sortingOrder = 10;
        Vector3 endPos = Vector3.zero;
        await effectGO.transform.DOMove(endPos, moveDuration).SetEase(Ease.InOutSine).AsyncWaitForCompletion();

        // Efekt burada kalabilir veya yok edilebilir
        Destroy(effectGO);
    }
    public async Task ReverseSpawnAndMoveColorEffect(ColorVector color, Vector3 startPos)
    {
        Vector3 endPos = Vector3.zero;
        var effectGO = Instantiate(worldcolorEffectPrefab, endPos, Quaternion.identity);
        var sr = effectGO.GetComponent<SpriteRenderer>();
        sr.color = color.ToUnityColor();
        sr.sortingOrder = 10;
        await effectGO.transform.DOMove(startPos, moveDuration).SetEase(Ease.InOutSine).AsyncWaitForCompletion();

        // Efekt burada kalabilir veya yok edilebilir
        Destroy(effectGO);
    }


    public async Task MoveEffectToUI(ColorVector color, Vector2 uiScreenPos)
    {
        var effectGO = Instantiate(UIEffectPrefab, uiCanvas); 
        var rt = effectGO.GetComponent<RectTransform>();
        var img = effectGO.GetComponentInChildren<Image>();
        img.color = color.ToUnityColor();

        rt.anchoredPosition = Vector2.zero;

        await rt.DOMove(uiScreenPos, finalMoveDuration).SetEase(Ease.OutBack).AsyncWaitForCompletion();

        Destroy(effectGO);
    }


    
}
