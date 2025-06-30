using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using GameEvents;
using System;



public class ReverseJokerUI : AsyncJokerUIBase
{
    protected override int JokerCount
    {
        get => PlayerPrefsService.RemainingReverse;
        set => PlayerPrefsService.RemainingReverse = value;
    }

    protected override string AdPlacementId => "reverse_joker";

    protected override async Task ExecuteJokerActionAsync()
    {
        // Reverse işlemini buraya yaz
        Debug.Log("Reverse action executed.");
        await Task.CompletedTask;  // Asenkron yapı olduğu için await tamamlandı olarak bırakıyoruz
    }

    // protected override async void OnUseClicked()
    // {
    //     if (isProcessing) return;

    //     isProcessing = true;
    //     UpdateUI();
    //     loadingIndicator.SetActive(true);

    //     try
    //     {
    //         bool rewarded = await AdManager.Instance.ShowRewardedAdAsync(AdPlacementId);
    //         if (rewarded)
    //         {
    //             loadingIndicator.SetActive(false);
    //             await ExecuteJokerActionAsync();
    //         }
    //         else
    //         {
    //             Debug.Log("Player didn't complete the ad");
    //             loadingIndicator.SetActive(false);
    //         }
    //     }
    //     catch (Exception e)
    //     {
    //         Debug.LogError($"Ad failed: {e.Message}");
    //         loadingIndicator.SetActive(false);
    //     }
    //     finally
    //     {
    //         isProcessing = false;
    //         UpdateUI();
    //     }
    // }

    protected override void UpdateUI()
    {
        // Stok olmadığı için buton durumu sadece isProcessing'e göre kontrol edilecek
        useButton.interactable = !isProcessing;
        earnButton.interactable = false;

        useButton.image.color = useButton.interactable ? Color.white : Color.gray;
        earnButton.image.color = Color.gray;

        countText.text = "∞"; // Sonsuz gösterimi
        countText.color = Color.green;
        loadingIndicator.SetActive(isProcessing);
    }
}
