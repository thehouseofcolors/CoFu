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
    [SerializeField] private TextMeshProUGUI failText;
    private bool _isTransitioning;



    protected override void InitializeButtons()
    {
        restartButton.transform.localScale = Vector3.zero;

        AddButtonListenerWithFeedback(restartButton, OnRestartClicked);
    }

    protected override void InitializeText()
    {
        if (failText != null)
        {
            failText.alpha = 0f;
            failText.gameObject.SetActive(true);
        }
    }



    private async void OnRestartClicked()
    {
        if (_isTransitioning) return;

        PlayButtonClickFeedback(restartButton.transform);
        await EventBus.PublishAuto(new LevelRestartRequestedEvent());
        _ = HideAsync();//bu ne hiçbir fikrim yok
    }



}