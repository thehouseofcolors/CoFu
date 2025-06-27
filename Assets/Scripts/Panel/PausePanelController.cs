using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using GameEvents;
using DG.Tweening;
using TMPro;


public class PausePanelController : BasePanelController
{
    [Header("Pause Panel Specific References")]
    [SerializeField] private Button extraTimeButton;
    [SerializeField] private Button failButton;

    protected override void Awake()
    {
        base.Awake();
        
        // Setup button listeners using the safe method
        SafeAddButtonListener(extraTimeButton, OnExtraTimeClicked);
        SafeAddButtonListener(failButton, OnFailClicked);
    }

    public override async Task ShowAsync(object transitionData = null)
    {
        await base.ShowAsync(transitionData);

        // Add panel-specific animations
        PlayButtonAppearAnimation(extraTimeButton.transform);
        await Task.Delay(100);
        PlayButtonAppearAnimation(failButton.transform);
    }

    private void OnExtraTimeClicked()
    {
        Debug.Log("Extra time requested");
        // Handle extra time logic
    }

    private async void OnFailClicked()
    {
        Debug.Log("Fail clicked");
        await GameStateMachine.ChangeStateAsync(new GameFailState());
    }
}