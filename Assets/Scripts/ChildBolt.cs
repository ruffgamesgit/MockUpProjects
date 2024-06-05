using System.Collections.Generic;
using UnityEngine;

public class ChildBolt : BaseBoltClass
{
    [Header("References")] [SerializeField]
    List<BaseBoltClass> possibleCollidableBoltsWhileParentMoving;
    private ParentBolts _parentBolt;

    protected override void Awake()
    {
        base.Awake();

        _parentBolt = transform.GetComponentInParent<ParentBolts>();
        gameObject.name = "Child_Bolt_" + colourEnum;
    }

    protected override void OnRealMoveStarted()
    {
        _parentBolt.RemoveChildBolt(this);
        transform.SetParent(null);
    }

    protected override void OnRealMoveEnded()
    {
         
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out BaseBoltClass bolt))
        {
            if (bolt == _parentBolt) return;

            if (isPicked)
                StopFakeMove();
            else
                _parentBolt.StopFakeMove();
        }
    }

    public override bool CanPerformMoving()
    {
        bool canPerform = true;
        for (int i = 0; i < obstacleBolts.Count; i++)
        {
            if (obstacleBolts[i].IsBoltActive())
                canPerform = false;
        }

        return canPerform;
    }

    public bool CanPerformWhileParentMoving()
    {
        bool canPerform = true;
        for (int i = 0; i < possibleCollidableBoltsWhileParentMoving.Count; i++)
        {
            if (possibleCollidableBoltsWhileParentMoving[i].IsBoltActive())
                canPerform = false;
        }

        return canPerform;
    }
}