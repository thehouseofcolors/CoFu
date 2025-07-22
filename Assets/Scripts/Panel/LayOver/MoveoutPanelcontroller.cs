using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using GameEvents;
using TMPro;
using System;

[RequireComponent(typeof(CanvasGroup))]
public class MoveOverLayOverController : BasePanelController
{
    [Header("Fail Overlay References")]
    [SerializeField] private Button extraMoveButton;
    [SerializeField] private Button failButton;
    [SerializeField] private TextMeshProUGUI failText;
    private bool _isTransitioning;



    protected override void InitializeButtons()
    {
        extraMoveButton.transform.localScale = Vector3.zero;
        failButton.transform.localScale = Vector3.zero;

        AddButtonListenerWithFeedback(extraMoveButton, OnExtraMoveClicked);
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



    private async void OnExtraMoveClicked()
    {
        if (_isTransitioning) return;

        PlayButtonClickFeedback(extraMoveButton.transform);
        await EventBus.PublishAuto(new ExtraMovesRequestedEvent());
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
        extraMoveButton.interactable = false;
        failButton.interactable = false;
    }


}