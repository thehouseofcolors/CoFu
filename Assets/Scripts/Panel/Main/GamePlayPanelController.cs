
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;


public class GamePlayPanelController : BasePanelController
{

    public override async Task ShowAsync(object transitionData = null)
    {

        await base.ShowAsync();
        await Effects.PanelTransition.Slide(contentRoot, Vector2.left, true);
    }

    public override async Task HideAsync(object transitionData = null)
    {
        await Effects.PanelTransition.Slide(contentRoot, Vector2.right, false);
        await base.HideAsync();
    }



}

