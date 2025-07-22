
using UnityEngine;
using GameEvents;

public class ExtraSlotButton : AsyncPassiveJokerUIBase
{
    private int _slotCount = 1;

    protected override int JokerCount
    {
        get => _slotCount;
        set => _slotCount = Mathf.Clamp(value, 0, 1);
    }

    private async void OnEarnClicked()
    {
        if (isProcessing) return;

        isProcessing = true;

        try
        {
            // Example ad integration
            await EventBus.PublishAuto(new ExtraSlotRequestedEvent());

        }
        finally
        {
            isProcessing = false;
        }
    }

}


