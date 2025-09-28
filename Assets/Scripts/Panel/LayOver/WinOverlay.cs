using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using GameEvents;

[RequireComponent(typeof(CanvasGroup))]
public class WinLayOverController : BasePanelController
{
    [Header("Win Overlay References")]
    [SerializeField] private Button continueButton;
    [SerializeField] private TextMeshProUGUI scoreText;


    protected override void InitializeButtons()
    {
        continueButton.transform.localScale = Vector3.zero;

        AddButtonListenerWithFeedback(continueButton, OnContinueClicked);
    }

    protected override void InitializeText()
    {
        if (scoreText != null)
        {
            scoreText.text = "0";
        }
    }

    public override async Task ShowAsync(object transitionData = null)
    {

        await base.ShowAsync();
        await Effects.PanelTransition.Slide(contentRoot, Vector2.left, true);
    }

    public override async Task HideAsync(object transitionData = null)
    {
        await Effects.PanelTransition.Slide(contentRoot, Vector2.right, false);
        await base.HideAsync();
    }



    private async void OnContinueClicked()
    {
        Debug.Log("Continue to next level");

        // await EventBus.PublishAuto(new NextLevelRequestedEvent());
        
        await EventBus.PublishAuto(new GameStartRequestedEvent());

    }


    protected override void OnDestroy()
    {
        if (continueButton != null) continueButton.onClick.RemoveAllListeners();
        base.OnDestroy();
    }
}
