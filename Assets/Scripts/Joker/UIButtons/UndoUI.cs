using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using GameEvents;

// // public class UndoJokerUI : MonoBehaviour
// // {
// //     [Header("UI References")]
// //     [SerializeField] private Button _useButton;
// //     [SerializeField] private Button _earnButton;
// //     [SerializeField] private TextMeshProUGUI _countText;

// //     [Header("Settings")]
// //     [SerializeField] private int _adRewardAmount = 5;

// //     public void OnEnable()
// //     {
// //         _useButton.onClick.AddListener(OnUse);
// //         _earnButton.onClick.AddListener(OnEarn);
// //         UpdateUI();
// //     }

// //     public void OnDisable()
// //     {
// //         _useButton.onClick.RemoveAllListeners();
// //         _earnButton.onClick.RemoveAllListeners();

// //     }




// //     private async void OnUse()
// //     {
// //         if (PlayerPrefsService.RemainingUndo > 0)
// //         {
// //             PlayerPrefsService.RemainingUndo--;
// //             //undo yap
// //             UpdateUI();
// //         }
// //         await Task.CompletedTask;
// //     }

// //     private async void OnEarn()
// //     {
// //         _earnButton.interactable = false;

// //         bool rewarded = await AdManager.Instance.ShowRewardedAdAsync("undo_joker");
// //         if (rewarded)
// //         {
// //             PlayerPrefsService.RemainingUndo += _adRewardAmount;
// //             UpdateUI();
// //         }

// //         _earnButton.interactable = true;
// //     }

// //     private void UpdateUI()
// //     {
// //         int count = PlayerPrefsService.RemainingUndo;
// //         _countText.text = count.ToString();
// //         Debug.Log($"undo: {count}");
// //         UpdateButtonStates();
// //     }

// //     private void UpdateButtonStates()
// //     {
// //         int count = PlayerPrefsService.RemainingUndo;
// //         _useButton.interactable = count > 0;
// //         _earnButton.interactable = count == 0;
// //     }
// // }

// public class UndoJokerUI : MonoBehaviour
// {
//     [Header("UI References")]
//     [SerializeField] private Button _useButton;
//     [SerializeField] private Button _earnButton;
//     [SerializeField] private TextMeshProUGUI _countText;
//     [SerializeField] private GameObject _loadingIndicator;

//     [Header("Settings")]
//     [SerializeField] private int _adRewardAmount = 5;

//     private void OnEnable()
//     {
//         _useButton.onClick.AddListener(OnUse);
//         _earnButton.onClick.AddListener(OnEarn);
//         UpdateUI();
//     }

//     private void OnDisable()
//     {
//         _useButton.onClick.RemoveAllListeners();
//         _earnButton.onClick.RemoveAllListeners();
//     }

//     private async void OnUse()
//     {
//         if (PlayerPrefsService.RemainingUndo <= 0) return;

//         PlayerPrefsService.RemainingUndo--;
//         // Add undo logic here
//         UpdateUI();
//     }

//     private async void OnEarn()
//     {
//         _earnButton.interactable = false;
//         _loadingIndicator.SetActive(true);

//         try
//         {
//             bool rewarded = await AdManager.Instance.ShowRewardedAdAsync("undo_joker");
//             if (rewarded)
//             {
//                 PlayerPrefsService.RemainingUndo += _adRewardAmount;
//                 UpdateUI();
//                 // Play earned effect
//             }
//         }
//         catch (System.Exception e)
//         {
//             Debug.LogError($"Ad failed: {e.Message}");
//             // Show error message to player
//         }
//         finally
//         {
//             _earnButton.interactable = true;
//             _loadingIndicator.SetActive(false);
//         }
//     }

//     private void UpdateUI()
//     {
//         int count = PlayerPrefsService.RemainingUndo;
//         _countText.text = count.ToString();
//         UpdateButtonStates();
//     }

//     private void UpdateButtonStates()
//     {
//         bool hasTokens = PlayerPrefsService.RemainingUndo > 0;
//         _useButton.interactable = hasTokens;
//         _earnButton.interactable = !hasTokens;
//     }
// }



public class UndoJokerUI : AsyncJokerUIBase
{
    protected override int JokerCount
    {
        get => PlayerPrefsService.RemainingUndo;
        set => PlayerPrefsService.RemainingUndo = value;
    }

    protected override string AdPlacementId => "undo_joker";

    protected override async Task ExecuteJokerActionAsync()
    {
        // Buraya undo işlemi mantığını ekle
        // Örneğin:
        await Task.Yield(); // Simule etmek için, kendi undo işlemini async yapabilirsin
        Debug.Log("Undo action executed.");
    }
    
    // protected override async Task ExecuteJokerActionAsync()
    // {
    //     try
    //     {
    //         // Execute the undo operation through your game service
    //         bool success = await _undoService.ExecuteUndoAsync();
            
    //         if (!success)
    //         {
    //             // Return the joker if undo failed
    //             JokerCount++;
    //             UpdateUI();
    //             Debug.LogWarning("Undo operation failed - joker returned");
    //         }
    //     }
    //     catch (System.Exception ex)
    //     {
    //         Debug.LogError($"Undo operation error: {ex.Message}");
    //         throw; // Re-throw to be caught by base class
    //     }
    // }

}

