using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using GameEvents;
using TMPro;
using System;

[RequireComponent(typeof(CanvasGroup))]
public class TimeOverLayOverController : BasePanelController
{
    [Header("Fail Overlay References")]
    [SerializeField] private Button extraTimeButton;
    [SerializeField] private Button failButton;
    [SerializeField] private TextMeshProUGUI failText;
    private bool _isTransitioning;



    protected override void InitializeButtons()
    {
        extraTimeButton.transform.localScale = Vector3.zero;
        failButton.transform.localScale = Vector3.zero;

        AddButtonListenerWithFeedback(extraTimeButton, OnExtraTimeClicked);
        AddButtonListenerWithFeedback(failButton, OnFailClicked);
    }

    protected override void InitializeText()
    {
        if (failText != null)
        {
            failText.alpha = 0f;
            failText.gameObject.SetActive(true);
        }
    }



    private async void OnExtraTimeClicked()
    {
        if (_isTransitioning) return;

        PlayButtonClickFeedback(extraTimeButton.transform);
        await EventBus.PublishAuto(new ExtraTimeRequestedEvent());
        _ = HideAsync();
    }

    private async void OnFailClicked()
    {
        if (_isTransitioning) return;

        PlayButtonClickFeedback(failButton.transform);

        await EventBus.PublishAuto(new GameFailEvent());
    }

    private void OnOverlayHidden()
    {
        // Reset button states
        extraTimeButton.interactable = false;
        failButton.interactable = false;
    }


}