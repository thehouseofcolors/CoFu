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
    [SerializeField] private Button menuButton;
    [SerializeField] private TextMeshProUGUI scoreText;


    private bool _isAnimating;



    protected override void InitializeButtons()
    {
        continueButton.transform.localScale = Vector3.zero;
        menuButton.transform.localScale = Vector3.zero;

        AddButtonListenerWithFeedback(continueButton, OnContinueClicked);
        AddButtonListenerWithFeedback(menuButton, OnMenuClicked);
    }

    protected override void InitializeText()
    {
        if (scoreText != null)
        {
            scoreText.text = "0";
        }
    }

    public override async Task ShowAsync(object transitionData=null)
    {
        if (_isAnimating) return;
        _isAnimating = true;
        await base.ShowAsync(null);
         _isAnimating = false;
        
    }



   
    private void OnContinueClicked()
    {
        if (_isAnimating) return;

        Debug.Log("Continue to next level");

        EventBus.PublishAuto(new NextLevelRequestedEvent());
    }

    private async void OnMenuClicked()
    {
        if (_isAnimating) return;

        try
        {
            _isAnimating = true;
            await GameStateMachine.SetStateAsync(new PlayingState());
            
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to restart game: {e}");
        }
        finally
        {
            _isAnimating = false;
        }
    }

    public override async Task HideAsync(object transitionData=null)
    {
        await base.HideAsync();
    }

    void OnDestroy()
    {
        if (continueButton != null) continueButton.onClick.RemoveAllListeners();
        if (menuButton != null) menuButton.onClick.RemoveAllListeners();
    }
}
