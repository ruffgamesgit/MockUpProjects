using System;
using System.Collections.Generic;
using UnityEngine;

public class ParentBolts : BaseBoltClass
{
    [Header("References")] [SerializeField]
    private List<ChildBolt> childrenBolts;

    protected override void Awake()
    {
        base.Awake();
        gameObject.name = "Parent_Bolt_" + colourEnum;

        #region Slot Parent Assign

        PickedEvent += () => SetSlotParent(Rotater.instance.platform);
        AnyMoveSequenceStartedEvent += () => SetSlotParent(Rotater.instance.platform);
        CollidedAndRoleIsPassiveEvent += () => SetSlotParent(Rotater.instance.platform);
        AnyMoveSequenceEndedEvent += () => SetSlotParent(transform);
        ReleasedEvent += () => SetSlotParent(Rotater.instance.platform);

        #endregion
    }

    void LateUpdate()
    {
        if (!performFakeMove) return;
        if (!GetLowestObstacleBolt()) return;

        if (GetLowestObstacleBolt().transform.position.y < transform.position.y)
        {
            OnCollidedWithBolt(GetLowestObstacleBolt());
        }
    }

    protected override void OnCollidedWithBolt(BaseBoltClass collidedBolt)
    {
        sparkParticle?.Play();
        StopFakeMove(collidedBolt);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out BaseBoltClass bolt))
        {
            if (!performFakeMove) return;
            if (childrenBolts.Contains(bolt as ChildBolt)) return;
            Debug.LogError("Sparkle");

            OnCollidedWithBolt(bolt);
        }
    }

    protected override bool CanPerformMoving()
    {
        bool canPerform = true;
        for (int i = 0; i < obstacleBolts.Count; i++)
        {
            if (obstacleBolts[i].IsBoltActive())
                canPerform = false;
        }

        for (int i = 0; i < childrenBolts.Count; i++)
        {
            if (!childrenBolts[i].CanPerformWhileParentMoving())
                canPerform = false;
        }


        return canPerform;
    }

    protected override bool IsPickable()
    {
        return !isPicked && !BlockPickingAnotherSequenceIsOn;
    }


    public void RemoveChildBolt(ChildBolt childBolt)
    {
        if (childrenBolts.Contains(childBolt))
            childrenBolts.Remove(childBolt);
    }

    protected override void UnsubscribeFromEvents()
    {
        // unsubscribe from future subscribed events   
    }

    public void SetPickable(bool blockPicking)
    {
        isPicked = blockPicking;
    }

    public int GetChildrenBoltCount()
    {
        return childrenBolts.Count;
    }

    BaseBoltClass GetLowestObstacleBolt()
    {
        BaseBoltClass targebolt = null;
        for (int i = 0; i < obstacleBolts.Count; i++)
        {
            if (!obstacleBolts[i].IsBoltActive()) continue;
            if (targebolt) break;
            targebolt = obstacleBolts[i];
        }

        return targebolt;
    }
}