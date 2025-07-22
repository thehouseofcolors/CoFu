
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameEvents;
using UnityEngine;

public class GameEconomyManager : MonoBehaviour, IGameSystem
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
        disposables.Add(EventBus.Subscribe<ExtraMovesRequestedEvent>(ExtraMove));
        disposables.Add(EventBus.Subscribe<ExtraTimeRequestedEvent>(ExtraTime));
        disposables.Add(EventBus.Subscribe<ExtraSlotRequestedEvent>(ExtraSlot));
    }

    private void Unsubscribe()
    {
        foreach (var disposable in disposables)
        {
            disposable.Dispose();
        }
        disposables.Clear();
    }

    private void ExtraMove(ExtraMovesRequestedEvent e)
    {
        Debug.Log("Extra Move Requested");

        // Örnek işlem:
        if (TrySpend(JokerCosts.ExtraMoves))
        {
            PlayerPrefsService.RemainingMoves += 5;
            Debug.Log("Extra moves granted.");
        }
    }



    private void ExtraTime(ExtraTimeRequestedEvent e)
    {
        Debug.Log("Extra Time Requested");

        if (TrySpend(JokerCosts.ExtraTime))
        {
            PlayerPrefsService.TimerStart += 30;
            Debug.Log("Extra time granted.");
        }
    }


    private void ExtraSlot(ExtraSlotRequestedEvent e)
    {
        Debug.Log("Extra Slot Requested");

        if (TrySpend(JokerCosts.ExtraSlot))
        {
            // Slot hakkı arttırma işlemi burada yapılmalı (örneğin tile grid'de bir ekstra slot açmak gibi)
            Debug.Log("Extra slot granted.");
        }
    }

    private bool TrySpend(int cost)
    {
        if (PlayerPrefsService.CurrentCoin >= cost)
        {
            typeof(PlayerPrefsService)
                .GetProperty("CurrentCoin", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)
                ?.SetValue(null, PlayerPrefsService.CurrentCoin - cost);

            return true;
        }

        Debug.LogWarning("Yetersiz coin!");
        return false;
    }
}
