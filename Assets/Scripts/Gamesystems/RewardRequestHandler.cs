
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameEvents;
using UnityEngine;

public class RewardRequesHandler : MonoBehaviour, IGameSystem
{
    private readonly List<IDisposable> disposables = new();

    public async Task Initialize()
    {
        Subscribe();
        await Task.CompletedTask;
    }

    public async Task Shutdown()
    {
        Unsubscribe();
        await Task.CompletedTask;
    }

    private void Subscribe()
    {
        disposables.Add(EventBus.Subscribe<RewardRequestedEvent>(HandleRewardRequest));
        // disposables.Add(EventBus.Subscribe<RewardResultEvent>(HandleRewardResult));
        
    }

    private void Unsubscribe()
    {
        foreach (var disposable in disposables)
        {
            disposable.Dispose();
        }
        disposables.Clear();
    }
    private async Task HandleRewardRequest(RewardRequestedEvent e)
    {
        RewardType rType = e.RewardType;
        JokerData jokerData = ExtraJokerDataConfigs.GetJokerData(e.RewardType);
        Cost[] cost = jokerData.Cost;
        Reward[] rewards = jokerData.Reward;
        if (TrySpend(cost))
        {

        }
        await Task.CompletedTask;
    }
    
    // private async void HandleRewardRequest(RewardRequestedEvent e)
    // {
    //     switch (e.RewardType)
    //     {
    //         case RewardType.Moves:
    //             await ExtraMove(e);
    //             break;
    //         case RewardType.Time:
    //             await ExtraTime(e);
    //             break;
    //         case RewardType.Slot:
    //             await ExtraSlot(e);
    //             break;
    //         case RewardType.Life:
    //             await ExtraLife(e);
    //             break;
    //         case RewardType.Coin:
    //             await ExtraCoin(e);
    //             break;
    //         // Diğer RewardType'lar varsa ekle
    //         default:
    //             Debug.LogWarning($"Unhandled reward type: {e.RewardType}");
    //             break;
    //     }
    // }

    // private async void HandleRewardResult(RewardResultEvent e)
    // {
    //     if (!e.IsSuccess) return;

    //     switch (e.RewardType)
    //     {
    //         case RewardType.Moves:
    //             await ExtraMove(e);
    //             break;
    //         case RewardType.Time:
    //             await ExtraTime(e);
    //             break;
    //         case RewardType.Slot:
    //             await ExtraSlot(e);
    //             break;
    //         case RewardType.Life:
    //             await ExtraLife(e);
    //             break;
    //         case RewardType.Coin:
    //             await ExtraCoin(e);
    //             break;
    //         // Diğer RewardType'lar varsa ekle
    //         default:
    //             Debug.LogWarning($"Unhandled reward type: {e.RewardType}");
    //             break;
    //     }
    // }

    // private async Task ExtraMove(RewardRequestedEvent e)
    // {
    //     Debug.Log("Extra Move Requested");

    //     if (TrySpend(ExtraJokerDataConfigs.ExtraMoves.Cost))
    //     {
    //         PlayerPrefsService.RemainingMoves += 5;
    //         await EventBus.PublishAuto(new RewardResultEvent(RewardType.Moves, true));
    //         Debug.Log("Extra moves granted.");
    //     }
    //     else
    //     {
    //         await EventBus.PublishAuto(new RewardResultEvent(RewardType.Moves, false));
    //         Debug.LogWarning("Not enough coins for Extra Moves!");
    //     }
    // }
    // private async Task ExtraTime(RewardRequestedEvent e)
    // {
    //     Debug.Log("Extra Time Requested");

    //     if (TrySpend(ExtraJokerDataConfigs.ExtraTime.Cost))
    //     {
    //         PlayerPrefsService.TimerStart += 30;

    //         await EventBus.PublishAuto(new RewardResultEvent(RewardType.Time, true));
    //         Debug.Log("Extra time granted.");
    //     }
    //     else
    //     {
    //         await EventBus.PublishAuto(new RewardResultEvent(RewardType.Time, false));
    //         Debug.LogWarning("Not enough coins for Extra Time!");
    //     }
    // }
    // private async Task ExtraSlot(RewardRequestedEvent e)
    // {
    //     Debug.Log("Extra Slot Requested");

    //     if (TrySpend(ExtraJokerDataConfigs.ExtraSlot.Cost))
    //     {
    //         // Slot hakkı arttırma işlemi burada yapılmalı (örneğin tile grid'de bir ekstra slot açmak gibi)

    //         await EventBus.PublishAuto(new RewardResultEvent(RewardType.Slot, true));
    //         Debug.Log("Extra slot granted.");
    //     }
    //     else
    //     {
    //         await EventBus.PublishAuto(new RewardResultEvent(RewardType.Slot, false));
    //         Debug.LogWarning("Not enough coins for Extra Slot!");
    //     }
    // }
    // private async Task ExtraLife(RewardRequestedEvent e)
    // {
    //     Debug.Log("Extra Life Requested");
    //     await EventBus.PublishAuto(new RewardResultEvent(RewardType.Life, true));

    // }
    // private async Task ExtraCoin(RewardRequestedEvent e)
    // {
    //     Debug.Log("Extra coin Requested");
    //     await EventBus.PublishAuto(new RewardResultEvent(RewardType.Coin, true));

    // }



    // private async Task ExtraMove(RewardResultEvent e)
    // {
    //     await Task.CompletedTask;
    // }


    // private async Task ExtraTime(RewardResultEvent e)
    // {
    //     await Task.CompletedTask;
    // }


    // private async Task ExtraSlot(RewardResultEvent e)
    // {
    //     await Task.CompletedTask;

    // }
    // private async Task ExtraLife(RewardResultEvent e)
    // {

    //     await Task.CompletedTask;
    // }


    // private async Task ExtraCoin(RewardResultEvent e)
    // {

    //     await Task.CompletedTask;

    // }


    private bool TrySpend(Cost[] cost)
    {
        foreach (var c in cost)
        {
            if (!ResourceManager.TrySpend(c.currencyType, c.cost))
            {
                Debug.LogWarning("Yetersiz coin!");
                return false;
            }
        }
        return true;
    }
}

