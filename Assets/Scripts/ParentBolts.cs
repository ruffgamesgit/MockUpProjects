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
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out BaseBoltClass bolt))
        {
            if (!PerformFakeMove) return;
            if (childrenBolts.Contains(bolt as ChildBolt)) return;

            StopFakeMove();
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
        return !isPicked;
    }

    public void RemoveChildBolt(ChildBolt childBolt)
    {
        if (childrenBolts.Contains(childBolt))
            childrenBolts.Remove(childBolt);
    }
}