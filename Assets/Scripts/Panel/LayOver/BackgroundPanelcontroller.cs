using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using GameEvents;

[RequireComponent(typeof(CanvasGroup))]
public class BackgroundLayOverController : BasePanelController
{
    [Header("Win Overlay References")]
    [SerializeField] private Button continueButton;
    [SerializeField] private Button menuButton;


    protected override void InitializeButtons()
    {
        continueButton.transform.localScale = Vector3.zero;
        menuButton.transform.localScale = Vector3.zero;

        AddButtonListenerWithFeedback(continueButton, OnContinueClicked);
        AddButtonListenerWithFeedback(menuButton, OnMenuClicked);
    }






    private void OnContinueClicked()
    {

        EventBus.PublishAuto(new GameResumeEvent());
    }

    private async void OnMenuClicked()
    {
        try
        {
            await GameStateMachine.SetStateAsync(new NonPlayingState());

        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to restart game: {e}");
        }
    }


    void OnDestroy()
    {
        if (continueButton != null) continueButton.onClick.RemoveAllListeners();
        if (menuButton != null) menuButton.onClick.RemoveAllListeners();
    }
    
}
