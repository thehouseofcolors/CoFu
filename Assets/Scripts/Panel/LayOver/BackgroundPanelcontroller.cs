using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using GameEvents;

[RequireComponent(typeof(CanvasGroup))]
public class ResumeOverlayController : BasePanelController
{
    [Header("Win Overlay References")]
    [SerializeField] private Button continueButton;
    [SerializeField] private Button menuButton;

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
    
    protected override void InitializeButtons()
    {
        continueButton.transform.localScale = Vector3.zero;
        menuButton.transform.localScale = Vector3.zero;

        AddButtonListenerWithFeedback(continueButton, OnContinueClicked);
        AddButtonListenerWithFeedback(menuButton, OnMenuClicked);
    }






    private void OnContinueClicked()
    {
        
    }

    private async void OnMenuClicked()
    {
        try
        {
            await GameStateMachine.SetStateAsync(new MenuState());

        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to restart game: {e}");
        }
    }


    protected override void OnDestroy()
    {
        base.OnDestroy();
        if (continueButton != null) continueButton.onClick.RemoveAllListeners();
        if (menuButton != null) menuButton.onClick.RemoveAllListeners();
    }
    
}
