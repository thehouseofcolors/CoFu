using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using GameEvents;
using DG.Tweening;
using TMPro;



public class FailPanelController : BasePanelController 
{
    [Header("Fail Panel Specifics")]
    [SerializeField] private Button restartButton;
    [SerializeField] private Button menuButton;

    protected override void Awake()
    {
        base.Awake();
        
        SafeAddButtonListener(restartButton, OnRestartClicked);
        SafeAddButtonListener(menuButton, OnMenuClicked);
    }

    public override async Task ShowAsync(object transitionData = null)
    {
        await base.ShowAsync(transitionData);
        
        // Animate buttons with sequential delay
        PlayButtonAppearAnimation(restartButton.transform);
        await Task.Delay((int)(buttonAppearDelay * 1000));
        PlayButtonAppearAnimation(menuButton.transform);
    }

    private void OnRestartClicked()
    {
        EventBus.PublishAuto(new LevelRestartRequestedEvent());
    }

    private void OnMenuClicked()
    {
        EventBus.PublishAuto(new MenuRequestedEvent());
    }
}