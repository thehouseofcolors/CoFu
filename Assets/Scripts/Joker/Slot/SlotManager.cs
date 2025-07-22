using System.Threading.Tasks;
using UnityEngine;
using GameEvents;
using NUnit.Framework.Interfaces;
using System.Collections.Generic;
using System;
using UnityEngine.UI;


public class IntermediateSlotManager : MonoBehaviour, IGameSystem
{
    [SerializeField] private GameObject slotCellPrefab;
    [SerializeField] private Transform slotParent;

    private List<SlotController> slotControllers;
    private int unlockedSlotCount = 1;
    private int slotCount ;

    private List<IDisposable> _eventSubscriptions = new List<IDisposable>();

    public async Task Initialize()
    {
        SubscribeEvents();
        await Task.CompletedTask;
    }

    public async Task Shutdown()
    {
        UnsubscribeEvents();
        await Task.CompletedTask;
    }

    private void SubscribeEvents()
    {
        _eventSubscriptions.Add(EventBus.Subscribe<GameLoadEvent>(OnLevelLoad));
    }

    private void UnsubscribeEvents()
    {
        foreach (var subscription in _eventSubscriptions)
        {
            subscription?.Dispose();
        }
        _eventSubscriptions.Clear();
    }

    public bool HasEmptySlot()
    {
        foreach (var slot in slotControllers)
        {
            // Only consider interactable (unlocked) slots
            if (slot.IsInteractable() && slot.IsEmpty())
                return true;
        }
        return false;
    }
    private void OnLevelLoad(GameLoadEvent e)
    {
        LevelConfig levelConfig = LevelManager.Instance.CurrentLevel;
        if (levelConfig != null)
        {
            Debug.LogWarning($"Level config not found for level {PlayerPrefsService.CurrentLevel}");
            return;
        }

        InitializeSlots(3);
    }


    // public void InitializeSlots(int slotCount)
    // {
    //     for (int i = 0; i < slotCount; i++)
    //     {
    //         GameObject slot = Instantiate(slotCellPrefab, slotParent);
    //         TryGetComponent<SlotController>
    //         slotControllers.Add(slot.GetComponent<SlotController>());
    //     }
    // }
    public void InitializeSlots(int slotCount)
    {
        // Validate required references
        if (slotCellPrefab == null)
        {
            Debug.LogError("SlotCellPrefab is not assigned!", this);
            return;
        }

        if (slotParent == null)
        {
            Debug.LogError("SlotParent transform is not assigned!", this);
            return;
        }

        if (slotControllers == null)
        {
            slotControllers = new List<SlotController>();
        }

        // Clear existing slots if any
        slotControllers.Clear();

        for (int i = 0; i < slotCount; i++)
        {
            // Instantiate new slot
            GameObject slot = Instantiate(slotCellPrefab, slotParent);
            if (slot == null) continue;

            // Get and validate SlotController
            SlotController controller = slot.GetComponent<SlotController>();
            if (controller == null)
            {
                Debug.LogWarning($"Slot prefab at index {i} is missing SlotController component", slot);
                continue;
            }

            slotControllers.Add(controller);
        }

        Debug.Log($"Initialized {slotControllers.Count} slots out of requested {slotCount}");
    }

    public void UnlockSlotByAd()
    {
        if (unlockedSlotCount >= slotCount) return;

        unlockedSlotCount++;
    }

    public SlotController GetSlot()
    {
        foreach (var slot in slotControllers)
        {
            if (slot.IsEmpty())
                return slot;

        }
        return null;
    }
}
