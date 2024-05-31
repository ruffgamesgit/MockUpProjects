using UnityEngine;

public class LayerManager : MonoSingleton<LayerManager>
{
    public event System.Action LayerDisappearedEvent;

    public void TriggerLayerDisappearEvent()
    {
        LayerDisappearedEvent?.Invoke();
    }
}