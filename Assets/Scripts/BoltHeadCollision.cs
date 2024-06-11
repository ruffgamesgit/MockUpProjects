using System;
using UnityEngine;

public class BoltHeadCollision : MonoBehaviour
{
    public event Action<BaseBoltClass> CollidedWithBoltEvent;
    [Header("Debug")] [SerializeField] private BaseBoltClass parentBolt;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out BoltHeadCollision headCollision))
        {
            if(!parentBolt.isPicked) return;
            if(!parentBolt.IsBoltActive()) return;
            
            if (headCollision.GetParentBolt() != parentBolt)
            {
                Debug.LogWarning("Head collision worked: " + headCollision.GetParentBolt().name);
                CollidedWithBoltEvent?.Invoke(headCollision.GetParentBolt());
            }
        }
    }

    private void OnMouseDown()
    {
         parentBolt.OnPicked();
    }

    public void SetParent(BaseBoltClass parent)
    {
        parentBolt = parent;
    }

    public BaseBoltClass GetParentBolt()
    {
        return parentBolt;
    }
}