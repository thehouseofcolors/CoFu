using System.Threading.Tasks;
using UnityEngine;
using GameEvents;
using NUnit.Framework.Interfaces;
using System.Collections.Generic;
using System;
using UnityEngine.UI;


// public class IntermediateSlotManager : MonoBehaviour
// {
//     [SerializeField] private List<Button> slotButtons;
//     [SerializeField] private GameObject slotHighlightPrefab; // opsiyonel
//     [SerializeField] private int maxSlotCount = 4;

//     private Color?[] slotColors; // nullable color: null = boş

//     private int unlockedSlotCount = 1;

//     private void Awake()
//     {
//         slotColors = new Color?[slotButtons.Count];

//         for (int i = 0; i < slotButtons.Count; i++)
//         {
//             int index = i;
//             slotButtons[i].onClick.AddListener(() => OnSlotClicked(index));
//         }

//         RefreshUI();
//     }

//     public void AddColorToAvailableSlot(Color newColor)
//     {
//         for (int i = 0; i < unlockedSlotCount; i++)
//         {
//             if (slotColors[i] == null)
//             {
//                 slotColors[i] = newColor;
//                 RefreshUI();
//                 return;
//             }
//         }

//         Debug.Log("Tüm aktif slotlar dolu!");
//     }

//     public void UnlockSlotByAd()
//     {
//         if (unlockedSlotCount >= maxSlotCount)
//         {
//             Debug.Log("Maksimum slot sayısına ulaşıldı.");
//             return;
//         }

//         // Buraya reklam tetikleme kodu eklenecek
//         // Reklam izlendikten sonra:
//         unlockedSlotCount++;
//         RefreshUI();
//     }

//     private void RefreshUI()
//     {
//         for (int i = 0; i < slotButtons.Count; i++)
//         {
//             bool isUnlocked = i < unlockedSlotCount;
//             slotButtons[i].interactable = isUnlocked;

//             Image slotImage = slotButtons[i].GetComponent<Image>();

//             if (isUnlocked && slotColors[i] != null)
//             {
//                 slotImage.color = (Color)slotColors[i];
//             }
//             else if (isUnlocked)
//             {
//                 slotImage.color = Color.gray; // boş slot
//             }
//             else
//             {
//                 slotImage.color = Color.black; // kilitli slot
//             }
//         }
//     }

//     private void OnSlotClicked(int index)
//     {
//         if (index >= unlockedSlotCount) return;

//         if (slotColors[index] != null)
//         {
//             Debug.Log($"Slot {index} içeriği: {slotColors[index]}");
//             // İstersen sahneye geri ekleme işlemi burada yapılabilir
//         }
//     }

//     public bool HasEmptySlot()
//     {
//         for (int i = 0; i < unlockedSlotCount; i++)
//         {
//             if (slotColors[i] == null)
//                 return true;
//         }
//         return false;
//     }
// }

public class IntermediateSlotManager : Singleton<IntermediateSlotManager>
{
    [SerializeField] private List<SlotController> slotControllers;
    private int unlockedSlotCount = 1;
    private int maxSlotCount = 4;

    public bool HasEmptySlot()
    {
        foreach (var slot in slotControllers)
        {
            if (slot.IsInteractable () && slot.hasColor) return false;

        }
        return true;
    }

    private void Start()
    {
        for (int i = 0; i < slotControllers.Count; i++)
        {
            int index = i;
            slotControllers[i].Init(index, this);

            // Buton’a click listener ekle
            slotControllers[i].GetComponent<Button>().onClick.AddListener(() =>
            {
                slotControllers[index].OnSlotClicked();
            });
        }

        RefreshSlotAvailability();
    }

    public void UnlockSlotByAd()
    {
        if (unlockedSlotCount >= maxSlotCount) return;

        unlockedSlotCount++;
        RefreshSlotAvailability();
    }

    private void RefreshSlotAvailability()
    {
        // for (int i = 0; i < slotControllers.Count; i++)
        // {
        //     bool active = i < unlockedSlotCount;
        //     slotControllers[i].gameObject.SetActive(active);
        // }
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
