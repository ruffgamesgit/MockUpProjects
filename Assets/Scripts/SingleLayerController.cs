using DG.Tweening;
using UnityEngine;

public class SingleLayerController : MonoBehaviour
{
    public bool IsLayerPickable()
    {
        return transform.parent.GetChild(transform.parent.childCount - 1).transform == transform;
    }

    // ReSharper disable Unity.PerformanceAnalysis
    public void OnBoxDisappear()
    {
        if (transform.childCount > 0) return;
        LayerManager.instance.TriggerLayerDisappearEvent();
        transform.DOScale(Vector3.zero, .5f).OnComplete((() => Destroy(gameObject)));
    }
}