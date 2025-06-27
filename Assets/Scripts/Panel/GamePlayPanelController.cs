using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using GameEvents;
using System;
using DG.Tweening;
using UnityEngine.UI;


public class GamePlayPanelController : BasePanelController
{
    [Header("Gameplay Specifics")]
    [SerializeField] private Button undoButton;

    protected override void Awake()
    {
        base.Awake();
        SafeAddButtonListener(undoButton, OnUndoClicked);
    }

    public override async Task ShowAsync(object transitionData = null)
    {
        if (canvasGroup == null || panelContent == null)
        {
            Debug.LogWarning("ShowAsync failed: canvasGroup or panelContent is null.");
            return;
        }
        gameObject.SetActive(true); // animasyondan önce

        // Simpler show animation for gameplay HUD
        _currentAnimation = DOTween.Sequence()
            .Append(canvasGroup.DOFade(1, fadeDuration))
            .Join(panelContent.DOScale(_originalScale, scaleDuration).SetEase(showEase));

        await _currentAnimation.AsyncWaitForCompletion();
        
        Debug.Log("play panel show");
    }

    private void OnUndoClicked()
    {
        Debug.Log("Undo clicked");
        // Add undo functionality here
    }
}

