using UnityEngine;
using GameEvents;

public class ExtraMoveButton : AsyncPassiveJokerUIBase
{
    private int _extraMoveCount = 10;

    protected override int JokerCount
    {
        get => _extraMoveCount;
        set => _extraMoveCount = Mathf.Clamp(value, 1, 10);
    }

    private async void OnEarnClicked()
    {
        if (isProcessing) return;

        isProcessing = true;

        try
        {
            // Example ad integration
            await EventBus.PublishAuto(new ExtraMovesRequestedEvent());

        }
        finally
        {
            isProcessing = false;
        }
    }

}

