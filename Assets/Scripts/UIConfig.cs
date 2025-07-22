
using System;
using UnityEngine;

[Serializable]
public abstract class PanelConfigBase<TPanelType>
{
    [NonSerialized] public IPanel panel;

    public PanelType category { get; protected set; }  // korumalı set
    public TPanelType panelType;
    public GameObject gameObject;

    public virtual bool TryInitialize()
    {
        if (gameObject == null)
        {
            Debug.LogError($"{typeof(TPanelType)} config for {panelType} has null GameObject");
            return false;
        }

        panel = gameObject.GetComponent<IPanel>();
        if (panel == null)
        {
            Debug.LogError($"GameObject {gameObject.name} doesn't implement IPanel");
            return false;
        }
        return true;
    }
}

[Serializable]
public class ScreenConfig : PanelConfigBase<ScreenType>
{
    public ScreenConfig()
    {
        category = PanelType.Screen;
    }
}


[Serializable]
public class OverlayConfig : PanelConfigBase<OverlayType>
{
    public OverlayConfig()
    {
        category = PanelType.Overlay;
    }
}
