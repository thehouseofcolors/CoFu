
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;


public class GamePlayPanelController : BasePanelController
{

    public override async Task ShowAsync(object transitionData = null)
    {

        await base.ShowAsync();
        await Effects.PanelTransition.Fade(canvasGroup, true, 3f);
    }

    public override async Task HideAsync(object transitionData = null)
    {
        await Effects.PanelTransition.Slide(contentRoot, Vector2.right, false);
        await base.HideAsync();
    }


}


// using System.Threading.Tasks;
// using UnityEngine;
// using UnityEngine.UI;
// using TMPro;
// using GameEvents;
// using System;
// using System.Collections.Generic;
// public class GamePlayPanelController : BasePanelController
// {

//     [Header("UI References")]
//     [SerializeField] private TextMeshProUGUI moveText;
//     [SerializeField] private TextMeshProUGUI timerText;
//     [SerializeField] private TextMeshProUGUI coinText;
//     [SerializeField] private TextMeshProUGUI lifeText;


//     private List<IDisposable> disposables = new List<IDisposable>();
//     private Vector3 _originalMoveTextScale;
//     private Vector3 _originalTimerTextScale;

//     protected override async void Setup()
//     {
//         base.Setup();
//         CacheOriginalScales();
//         ResetUI();
//         SubscribeToEvents();
//         await Task.CompletedTask;
//     }

//     public override async Task ShowAsync(object transitionData = null)
//     {
//         await base.ShowAsync();
//         await Effects.PanelTransition.Fade(canvasGroup, true, 3f);
//     }

//     public override async Task HideAsync(object transitionData = null)
//     {
//         await Effects.PanelTransition.Slide(contentRoot, Vector2.right, false);
//         await base.HideAsync();
//     }




//     private void CacheOriginalScales()
//     {
//         _originalMoveTextScale = moveText.transform.localScale;
//         _originalTimerTextScale = timerText.transform.localScale;
//     }

//     private void ResetUI()
//     {
//         moveText.text = "Moves: 0";
//         timerText.text = "Time: 0";
//     }

//     private void SubscribeToEvents()
//     {
//         disposables.Add(EventBus.Subscribe<UpdateMoveCountUIEvent>(OnMoveUpdate));
//         disposables.Add(EventBus.Subscribe<UpdateTimerUIEvent>(OnTimerUpdate));
//     }

//     public override void OnQuit()
//     {
//         foreach (var disposable in disposables)
//         {
//             disposable?.Dispose();
//         }
//     }

//     private async Task OnMoveUpdate(UpdateMoveCountUIEvent evt)
//     {
//         moveText.text = $"Moves: {evt.MoveCount}";
//         await Task.CompletedTask;
//     }

//     private async Task OnTimerUpdate(UpdateTimerUIEvent evt)
//     {
//         timerText.text = $"Time: {Mathf.CeilToInt(evt.RemainingTime)}";
//         if (evt.RemainingTime == 0)
//         {
//             //publişhh timeout
//         }
//         // Add urgency effect when time is low
//         if (evt.RemainingTime < 10f)
//         {
//             timerText.color = Color.red;
//         }
//         else
//         {
//             timerText.color = Color.white;
//         }

//         await Task.CompletedTask;
//     }

// }

