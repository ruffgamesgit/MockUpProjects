using System;
using UnityEngine;

public class BoltHeadCollision : MonoBehaviour
{
    public event Action<BaseBoltClass> CollidedWithBoltEvent;
    [Header("Debug")] [SerializeField] private BaseBoltClass parentBolt;

    private void OnTriggerEnter(Collider other)
    {
        Debug.LogWarning("1");
        if (other.TryGetComponent(out BoltHeadCollision headCollision))
        {
            Debug.LogWarning("2");
            if (parentBolt.transform.GetComponent<ParentBolts>())
            {
                Debug.LogWarning("3");
                if (!parentBolt.isPicked) return;
            }
            else if (parentBolt.transform.GetComponent<ChildBolt>())
            {
                Debug.LogWarning("4");
                ChildBolt childBolt = parentBolt as ChildBolt;
                if (!childBolt.isPicked && !childBolt.isParentPicked) return;
                if(childBolt.blockCollision) return;
            }
            Debug.LogWarning("5");
            if (!parentBolt.IsBoltActive()) return;
            Debug.LogWarning("6");
            if (headCollision.GetParentBolt() != parentBolt)
            {
                Debug.LogWarning("7");
                if(headCollision.GetParentBolt().performFakeMove) return;
                Debug.LogWarning("Collided with bolt: " + headCollision.GetParentBolt().gameObject.name);
                CollidedWithBoltEvent?.Invoke(headCollision.GetParentBolt());
            }
            Debug.LogWarning("8");
        }
    }
    
    public void SetParent(BaseBoltClass parent)
    {
        parentBolt = parent;
    }

    private BaseBoltClass GetParentBolt()
    {
        return parentBolt;
    }
}