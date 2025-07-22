using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using GameEvents;
using System;

public abstract class AsyncPassiveJokerUIBase : MonoBehaviour
{
    // Required UI Elements
    [SerializeField] protected Button earnButton;
    [SerializeField] protected TextMeshProUGUI countText;
    protected float minActionDuration = 0.3f; // Minimum visual feedback time
    protected bool isProcessing;
    protected abstract int JokerCount { get; set; }

    protected virtual void Initialize()
    {
        earnButton.onClick.AddListener(OnEarnClicked);
    }

    protected virtual void Shutdown()
    {
        earnButton.onClick.RemoveListener(OnEarnClicked);
    }


    private async void OnEarnClicked()
    {
        if (isProcessing) return;

        isProcessing = true;

        try
        {
            earnButton.interactable = false;

        }
        catch (Exception e)
        {
            Debug.LogError($"Ad failed: {e.Message}");
            // Optionally show error to player
        }
        finally
        {
            isProcessing = false;
        }
        await Task.CompletedTask;
    }


}

