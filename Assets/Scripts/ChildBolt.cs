using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class ChildBolt : BaseBoltClass
{
    [Header("References")] [SerializeField]
    List<BaseBoltClass> possibleCollidableBoltsWhileParentMoving;

    private ParentBolts _parentBolt;

    private bool _isParentPicked;

    protected override void Awake()
    {
        base.Awake();

        _parentBolt = transform.GetComponentInParent<ParentBolts>();
        gameObject.name = "Child_Bolt_" + colourEnum;

        RealMoveStartedEvent += OnRealMoveStarted;
        _parentBolt.PickedEvent += OnParentBoltPicked;
        _parentBolt.AnyMoveSequenceEndedEvent += OnParentAnyMoveSequenceEnded;
        _parentBolt.ReleasedEvent += OnParentBoltReleasedEvent;
    }


    #region EVENT SUBSCRIBERS

    private void OnParentBoltReleasedEvent()
    {
        OnReleased();
    }

    private void OnParentBoltPicked()
    {
        _isParentPicked = true;
    }

    private void OnParentAnyMoveSequenceEnded()
    {
        _isParentPicked = false;
    }

    void OnRealMoveStarted()
    {
        _parentBolt.RemoveChildBolt(this);
        transform.SetParent(null);
    }

    #endregion

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out BaseBoltClass bolt))
        {
            if (bolt == _parentBolt) return;

            if (isPicked)
            {
                if (obstacleBolts.Contains(bolt))
                    StopFakeMove(bolt);
            }
            else
            {
                if (possibleCollidableBoltsWhileParentMoving.Contains(bolt))
                    _parentBolt.StopFakeMove(bolt);
            }
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

        return canPerform;
    }

    protected override bool IsPickable()
    {
        return !isPicked && !_isParentPicked;
    }

    protected override void UnsubscribeFromEvents()
    {
        RealMoveStartedEvent -= OnRealMoveStarted;
        _parentBolt.PickedEvent -= OnParentBoltPicked;
        _parentBolt.AnyMoveSequenceEndedEvent -= OnParentAnyMoveSequenceEnded;
        _parentBolt.ReleasedEvent -= OnParentBoltReleasedEvent;
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