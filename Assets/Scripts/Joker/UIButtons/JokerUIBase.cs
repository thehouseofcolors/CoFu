using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using GameEvents;
using System;

public abstract class AsyncJokerUIBase : MonoBehaviour
{
    // Required UI Elements
    [SerializeField] protected Button useButton;
    [SerializeField] protected Button earnButton;
    [SerializeField] protected TextMeshProUGUI countText;
    [SerializeField] protected GameObject loadingIndicator;
    
    // Configuration
    [SerializeField] protected int adRewardAmount = 3;
    [SerializeField] protected float minActionDuration = 0.3f; // Minimum visual feedback time

    protected bool isProcessing;
    protected abstract int JokerCount { get; set; }
    protected abstract string AdPlacementId { get; }

    protected virtual void OnEnable()
    {
        useButton.onClick.AddListener(OnUseClicked);
        earnButton.onClick.AddListener(OnEarnClicked);
        UpdateUI();
    }

    protected virtual void OnDisable()
    {
        useButton.onClick.RemoveListener(OnUseClicked);
        earnButton.onClick.RemoveListener(OnEarnClicked);
    }

    private async void OnUseClicked()
    {
        if (isProcessing || JokerCount <= 0) return;
        
        isProcessing = true;
        UpdateUI();
        
        try
        {
            // Start timing for minimum visual feedback
            var actionTask = ExecuteJokerActionAsync();
            var delayTask = Task.Delay((int)(minActionDuration * 1000));
            
            await Task.WhenAll(actionTask, delayTask);
            
            JokerCount--;
            UpdateUI();
        }
        catch (Exception e)
        {
            Debug.LogError($"Joker action failed: {e.Message}");
            // Consider returning the joker count if failure should be recoverable
        }
        finally
        {
            isProcessing = false;
            UpdateUI();
        }
    }

    private async void OnEarnClicked()
    {
        if (isProcessing) return;
        
        isProcessing = true;
        UpdateUI();
        
        try
        {
            earnButton.interactable = false;
            loadingIndicator.SetActive(true);
            
            bool rewarded = await AdManager.Instance.ShowRewardedAdAsync(AdPlacementId);
            if (rewarded)
            {
                JokerCount += adRewardAmount;
                // Optional: Add quick visual pulse effect here
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Ad failed: {e.Message}");
            // Optionally show error to player
        }
        finally
        {
            loadingIndicator.SetActive(false);
            isProcessing = false;
            UpdateUI();
        }
    }

    protected abstract Task ExecuteJokerActionAsync();

    protected virtual void UpdateUI()
    {
        countText.text = JokerCount.ToString();
        
        // Basic button state management
        useButton.interactable = !isProcessing && JokerCount > 0;
        earnButton.interactable = !isProcessing && JokerCount <= 0;
    }
}

