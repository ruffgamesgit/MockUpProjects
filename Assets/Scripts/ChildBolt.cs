using System.Collections.Generic;
using UnityEngine;

public class ChildBolt : BaseBoltClass
{
    [Header("References")] [SerializeField]
    List<BaseBoltClass> possibleCollidableBoltsWhileParentMoving;

    private ParentBolts _parentBolt;
    public bool isParentPicked;
    [SerializeField] private bool blockCollision;

    protected override void Awake()
    {
        base.Awake();

        _parentBolt = transform.GetComponentInParent<ParentBolts>();
        gameObject.name = "Child_Bolt_" + colourEnum;

        RealMoveStartedEvent += OnRealMoveStarted;
        PickedEvent += () => _parentBolt.SetPickable(true);
        AnyMoveSequenceEndedEvent += OnAnyMoveSequenceEnded;

        _parentBolt.PickedEvent += OnParentBoltPicked;
        _parentBolt.AnyMoveSequenceEndedEvent += OnParentAnyMoveSequenceEnded;
        _parentBolt.ReleasedEvent += OnParentBoltReleasedEvent;
        SetSlotParent(_parentBolt.transform);
    }
    
    #region EVENT SUBSCRIBERS
    private void OnAnyMoveSequenceEnded()
    {
        _parentBolt.SetPickable(false);
        blockCollision = false;
    }

    private void OnParentBoltReleasedEvent()
    {
        isActive = false;
        OnReleased();
    }

    private void OnParentBoltPicked()
    {
        isParentPicked = true;
    }

    private void OnParentAnyMoveSequenceEnded()
    {
        isParentPicked = false;
        blockCollision = false;
    }

    void OnRealMoveStarted()
    {
        _parentBolt.RemoveChildBolt(this);
        transform.SetParent(null);
    }

    #endregion
    protected override void OnCollidedWithBolt(BaseBoltClass collidedBolt)
    {
        if (isPicked)
        {
            if (obstacleBolts.Contains(collidedBolt))
            {
                blockCollision = true;
                sparkParticle?.Play();
                StopFakeMove(collidedBolt);
            }
        }
        else
        {
            if (possibleCollidableBoltsWhileParentMoving.Contains(collidedBolt))
            {
                blockCollision = true;
                sparkParticle?.Play();
                _parentBolt.StopFakeMove(collidedBolt);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out BaseBoltClass bolt))
        {
            if (!isPicked && !_parentBolt.isPicked) return;
            if (bolt == _parentBolt) return;
            if (blockCollision) return;

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

        return canPerform;
    }

    protected override bool IsPickable()
    {
        return !isPicked && !isParentPicked;
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