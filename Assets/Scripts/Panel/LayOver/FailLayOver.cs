using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using GameEvents;
using TMPro;
using System;

[RequireComponent(typeof(CanvasGroup))]
public class FailLayOverController : BasePanelController
{
    [Header("Fail Overlay References")]
    [SerializeField] private Button restartButton;
    [SerializeField] private Button menuButton;
    [SerializeField] private TextMeshProUGUI failText;
    private bool _isTransitioning;



    protected override void InitializeButtons()
    {
        restartButton.transform.localScale = Vector3.zero;
        menuButton.transform.localScale = Vector3.zero;

        AddButtonListenerWithFeedback(restartButton, OnRestartClicked);
        AddButtonListenerWithFeedback(menuButton, OnMenuClicked);
    }

    protected override void InitializeText()
    {
        if (failText != null)
        {
            failText.alpha = 0f;
            failText.gameObject.SetActive(true);
        }
    }



    private void OnRestartClicked()
    {
        if (_isTransitioning) return;

        PlayButtonClickFeedback(restartButton.transform);
        EventBus.PublishAuto(new LevelRestartRequestedEvent());
        _ = HideAsync();
    }

    private async void OnMenuClicked()
    {
        if (_isTransitioning) return;

        PlayButtonClickFeedback(menuButton.transform);

        try
        {
            _isTransitioning = true;
            await GameStateMachine.SetStateAsync(new NonPlayingState());
            await HideAsync();
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to return to menu: {e}");
        }
        finally
        {
            _isTransitioning = false;
        }
    }

    private void OnOverlayHidden()
    {
        // Reset button states
        restartButton.interactable = false;
        menuButton.interactable = false;
    }


}