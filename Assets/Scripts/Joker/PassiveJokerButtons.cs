
using UnityEngine;
using UnityEngine.UI;
using GameEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class PassiveJokerButtons : MonoBehaviour, IGameSystem, IQuittable, IPausable
{
    [Header("Configuration")]
    [SerializeField] private List<JokerButtonData> jokerButtons;

    [Header("Feedback")]
    [SerializeField] private AudioClip successSound;
    [SerializeField] private AudioClip errorSound;
    [SerializeField] private float globalCooldown = 0.5f;

    private bool isProcessing;
    private AudioSource audioSource;
    private List<Button> validButtons;
    private float lastClickTime;

    #region Initialization and Lifecycle

    public async Task Initialize()
    {
        TryGetComponent(out audioSource);
        CacheValidButtons();
        ValidateConfiguration();

        foreach (var button in validButtons)
        {
            button.onClick.AddListener(() => OnButtonClicked(button));
        }
        await Task.CompletedTask;
    }

    public async Task Shutdown()
    {
        RemoveAllButtonListeners();
        await Task.CompletedTask;
    }

    public void OnPause()
    {
        RemoveAllButtonListeners();
    }

    public void OnResume()
    {
        TryGetComponent(out audioSource);
        foreach (var button in validButtons)
        {
            button.onClick.AddListener(() => OnButtonClicked(button));
        }
    }

    public void OnQuit()
    {
        RemoveAllButtonListeners();
    }

    #endregion

    #region Core Functionality

    private async void OnButtonClicked(Button clickedButton)
    {
        if (isProcessing || Time.time < lastClickTime + globalCooldown) return;

        lastClickTime = Time.time;
        isProcessing = true;

        var buttonData = jokerButtons.FirstOrDefault(b => b.EarnButton == clickedButton);
        if (buttonData == null) return;

        clickedButton.interactable = false;
        var startTime = Time.time;

        try
        {
            await ProcessRewardRequest(buttonData);
            await EnsureMinimumProcessingTime(startTime, buttonData.MinProcessingTime);
            PlayFeedbackSound(success: true);
        }
        catch (Exception e)
        {
            Debug.LogError($"Reward process failed: {e.Message}");
            PlayFeedbackSound(success: false);
            // Additional visual feedback could be added here
        }
        finally
        {
            clickedButton.interactable = true;
            isProcessing = false;
        }
    }

    private async Task ProcessRewardRequest(JokerButtonData buttonData)
    {
        await Effects.Buttons.PlayClick(buttonData.EarnButton.transform).ConfigureAwait(false);
        await EventBus.PublishAuto(new RewardRequestedEvent(buttonData.RewardType)).ConfigureAwait(false);
    }

    #endregion

    #region Helper Methods

    private async Task EnsureMinimumProcessingTime(float startTime, float minDuration)
    {
        var elapsedTime = Time.time - startTime;
        if (elapsedTime < minDuration)
        {
            await Task.Delay((int)((minDuration - elapsedTime) * 1000)).ConfigureAwait(false);
        }
    }

    private void PlayFeedbackSound(bool success)
    {
        if (audioSource == null) return;

        var clip = success ? successSound : errorSound;
        if (clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    private void CacheValidButtons()
    {
        validButtons = jokerButtons?
            .Where(b => b.EarnButton != null)
            .Select(b => b.EarnButton)
            .ToList() ?? new List<Button>();
    }

    private void ValidateConfiguration()
    {
        if (jokerButtons == null || jokerButtons.Count == 0)
        {
            Debug.LogWarning("No joker buttons configured!");
        }
    }

    private void RemoveAllButtonListeners()
    {
        foreach (var button in validButtons)
        {
            button.onClick.RemoveAllListeners();
        }
    }

    #endregion

    #region Editor Helpers

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (globalCooldown < 0) globalCooldown = 0;
    }
#endif

    #endregion
}



// using UnityEngine;
// using UnityEngine.UI;
// using GameEvents;
// using System;
// using System.Collections.Generic;
// using System.Threading.Tasks;

// public class PassiveJokerButtons : MonoBehaviour, IGameSystem, IQuittable,IPausable
// {
//     [Header("Configuration")]
//     [SerializeField] private List<JokerButtonData> jokerButtons;

//     [Header("Feedback")]
//     [SerializeField] private AudioClip successSound;
//     [SerializeField] private AudioClip errorSound;
//     private AudioSource audioSource;


//     #region IGameSystem-IPausable-IQuittable Implementation

//     public async Task Initialize()
//     {
//         TryGetComponent(out audioSource);
//         foreach (var buttonData in jokerButtons)
//         {
//             if (buttonData.EarnButton != null)
//             {
//                 buttonData.EarnButton.onClick.AddListener(() => OnEarnClicked(buttonData));
//             }
//         }
//     }

//     public async Task Shutdown()
//     {
//         foreach (var buttonData in jokerButtons)
//         {
//             if (buttonData.EarnButton != null)
//             {
//                 buttonData.EarnButton.onClick.RemoveAllListeners();
//             }
//         }
//     }

//     public void OnPause()
//     {
//         foreach (var buttonData in jokerButtons)
//         {
//             if (buttonData.EarnButton != null)
//             {
//                 buttonData.EarnButton.onClick.RemoveAllListeners();
//             }
//         }
//     }

//     public void OnResume()
//     {
//         TryGetComponent(out audioSource);
//         foreach (var buttonData in jokerButtons)
//         {
//             if (buttonData.EarnButton != null)
//             {
//                 buttonData.EarnButton.onClick.AddListener(() => OnEarnClicked(buttonData));
//             }
//         }

//     }

//     public void OnQuit()
//     {
//         foreach (var buttonData in jokerButtons)
//         {
//             if (buttonData.EarnButton != null)
//             {
//                 buttonData.EarnButton.onClick.RemoveAllListeners();
//             }
//         }

//     }

//     #endregion


//     private async void OnEarnClicked(JokerButtonData buttonData)
//     {
//         if ( buttonData.EarnButton == null) return;

//         var startTime = Time.time;
//         buttonData.EarnButton.interactable = false;

//         try
//         {
//             await ProcessRewardRequest(buttonData);

//             // Ensure minimum processing time for better UX
//             await EnsureMinimumProcessingTime(startTime, buttonData.MinProcessingTime);

//             PlayFeedbackSound(success: true);
//         }
//         catch (Exception e)
//         {
//             Debug.LogError($"Reward process failed: {e.Message}");
//             PlayFeedbackSound(success: false);
//             // Consider adding visual feedback here as well
//         }
//         finally
//         {
//             buttonData.EarnButton.interactable = true;
//         }
//     }

//     private async Task ProcessRewardRequest(JokerButtonData buttonData)
//     {
//         await Effects.Buttons.PlayClick(buttonData.EarnButton.transform);
//         await EventBus.PublishAuto(new RewardRequestedEvent(buttonData.RewardType));
//         // Add any additional reward processing logic here
//     }

//     private async Task EnsureMinimumProcessingTime(float startTime, float minDuration)
//     {
//         var elapsedTime = Time.time - startTime;
//         if (elapsedTime < minDuration)
//         {
//             await Task.Delay((int)((minDuration - elapsedTime) * 1000));
//         }
//     }

//     private void PlayFeedbackSound(bool success)
//     {
//         if (audioSource == null) return;

//         var clip = success ? successSound : errorSound;
//         if (clip != null)
//         {
//             audioSource.PlayOneShot(clip);
//         }
//     }
// }

