using UnityEngine;
using GameEvents;

public class ExtraTimeButton : AsyncPassiveJokerUIBase
{
    private int _extraTimeCount = 20;

    protected override int JokerCount
    {
        get => _extraTimeCount;
        set => _extraTimeCount = Mathf.Clamp(value, 10, 30);
    }

    private async void OnEarnClicked()
    {
        if (isProcessing) return;

        isProcessing = true;

        try
        {
            // Example ad integration
            await EventBus.PublishAuto(new ExtraTimeRequestedEvent());

        }
        finally
        {
            isProcessing = false;
        }
    }

}

