using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public abstract class BaseBoltClass : MonoBehaviour
{
    public event Action RealMoveStartedEvent;
    public event Action AnyMoveSequenceEndedEvent;
    public event Action ReleasedEvent;
    public event Action PickedEvent;
    [Header("Base Config")] public ColourEnum colourEnum;
    [SerializeField] private float frontOffset;

    [Header("Base References")] [SerializeField]
    protected ParticleSystem sparkParticle;

    [SerializeField] protected Transform slotObject;
    [SerializeField] protected BoltHeadCollision headCollision;
    [SerializeField] protected List<BaseBoltClass> obstacleBolts;

    [Header("Base Debug")] [SerializeField]
    protected bool isActive;

    [SerializeField] public bool isPicked;
    [SerializeField] protected bool shouldRotate;
    protected bool BlockPickingAnotherSequenceIsOn;
    protected bool PerformFakeMove;
    private const float RotationSpeed = 700f;
    private Tween _fakeMoveTween;
    private Vector3 _startPos;
    private Quaternion _initRot;
    private PlacementPoint _currentPoint;

    protected virtual void Awake()
    {
        isActive = true;

        headCollision.SetParent(this);
        headCollision.CollidedWithBoltEvent += OnCollidedWithBolt;
    }

    protected abstract void OnCollidedWithBolt(BaseBoltClass collidedBolt);

    protected void OnMouseDown()
    {
        OnPicked();
    }

    public void OnPicked()
    {
        if (!GameManager.instance.isLevelActive) return;
        if (Rotater.instance.isRotating) return;
        if (!IsPickable()) return;

        Rotater.instance.blockPlatformRotation = true;

        PickedEvent?.Invoke();
        isPicked = true;
        shouldRotate = true;

        if (!CanPerformMoving())
        {
            FakeMove();
        }

        else
        {
            RealMove();
        }
    }

    private void RealMove()
    {
        int iterator = 0;
        while (iterator < 3)
        {
            Taptic.Light();
            iterator++;
        }

        RealMoveStartedEvent?.Invoke();
        isActive = false;
        Vector3 movementDirection = transform.up * frontOffset;
        Vector3 targetPosition = transform.position + movementDirection;

        transform.DOMove(targetPosition, .5f).SetDelay(0.15f).OnComplete(() =>
        {
            shouldRotate = false;
            AnyMoveSequenceEndedEvent?.Invoke();
            Rotater.instance.blockPlatformRotation = false;
            OnReleased();
        });
    }

    protected void OnReleased()
    {
        UnsubscribeFromEvents();
        Destroy(transform.GetComponent<Collider>());
        Destroy(transform.GetComponent<Rigidbody>());
        transform.SetParent(null);
        ReleasedEvent?.Invoke();
        ColoredHole coloredHole = HoleManager.instance.GetCurrentHole();

        if (ColourUtility.CheckIfColorsMatch(colourEnum, coloredHole.GetColorEnum())
            && !coloredHole.willBeDisappeared)
        {
            if (coloredHole.GetAvailablePoint() != null)
            {
                GoToPoint(coloredHole.GetAvailablePoint(), coloredHole);
            }
        }
        else
        {
            if (NeutralHole.instance.GetAvailablePoint())
            {
                GoToPoint(NeutralHole.instance.GetAvailablePoint(), NeutralHole.instance);
            }
            else
            {
                Debug.LogWarning("No Placeable point left on NEUTRAL HOLE, FAIL");
                GameManager.instance.EndGame(false);
            }
        }
    }

    public void GoToPoint(PlacementPoint targetPoint, BaseHoleClass newHole = null)
    {
        _currentPoint?.SetFree();
        _currentPoint = targetPoint;
        targetPoint.SetOccupied(this);

        if (newHole && newHole != NeutralHole.instance)
        {
            ColoredHole hole = newHole as ColoredHole;
            hole?.CheckDisappearingSequence();
        }

        transform.SetParent(targetPoint.transform);
        transform.forward = targetPoint.transform.forward;
        Vector3 virtualPos = new(targetPoint.transform.position.x, targetPoint.transform.position.y + 1f,
            targetPoint.transform.position.z);
        Sequence sq = DOTween.Sequence();
        sq.Append(
            transform.DOMove(virtualPos, .35f).OnComplete(() => { shouldRotate = true; }));
        sq.Append(transform.DOMove(targetPoint.transform.position
            , .25f).OnStart(() => { shouldRotate = false; }));
        sq.OnComplete(() =>
        {
            if (newHole == null) return;

            if (newHole == NeutralHole.instance)
                newHole.OnBoltArrived();
            else
            {
                if (newHole.GetOccupiedPointCount() == newHole.GetPointCount())
                    newHole.OnBoltArrived();
            }
        });
    }

    private void FakeMove()
    {
        PerformFakeMove = true;
        Vector3 movementDirection = transform.up * frontOffset;
        Vector3 targetPosition = transform.position + movementDirection;
        _startPos = transform.position;
        _initRot = transform.rotation;

        #region Delay for rotation

        float delay = .15f;
        if (transform.GetComponent<ParentBolts>())
        {
            delay = transform.GetComponent<ParentBolts>().GetChildrenBoltCount() > 0 ? .35f : delay;
        }

        #endregion

        _fakeMoveTween = transform.DOMove(targetPosition, .5f).SetDelay(delay);
        _fakeMoveTween.Play();
    }

    public void StopFakeMove(BaseBoltClass collidedBolt, bool isChildBolt)
    {
        Taptic.Medium();
        shouldRotate = false;
        PerformFakeMove = false;
        _fakeMoveTween.Kill();
        collidedBolt?.OnCollidedAndRoleIsPassive();
        transform.DOMove(_startPos, .5f);

        DOTween.To(() => 0f, x =>
            {
                transform.rotation =
                    Quaternion.Lerp(transform.rotation, _initRot, x);
            }, 0.99f, .5f)
            .OnComplete(() =>
            {
                AnyMoveSequenceEndedEvent?.Invoke();
                Rotater.instance.blockPlatformRotation = false;
                SetSlotParent(transform);

                isPicked = false;
            });
    }

    private void OnCollidedAndRoleIsPassive()
    {
        // SetSlotParent(Rotater.instance.transform);
        BlockPickingAnotherSequenceIsOn = true;
        transform.DOMove(transform.position + transform.up * 0.125f, .125f)
            .SetLoops(2, LoopType.Yoyo)
            .OnComplete(() =>
            {
                // SetSlotParent(transform);
                BlockPickingAnotherSequenceIsOn = false;
            });
    }

    protected virtual void Update()
    {
        if (shouldRotate)
        {
            transform.Rotate(Vector3.up, 1 * RotationSpeed * Time.deltaTime);
        }
    }

    protected abstract bool CanPerformMoving();
    protected abstract bool IsPickable();

    public bool IsBoltActive()
    {
        return isActive;
    }

    public ColourEnum GetColor()
    {
        return colourEnum;
    }

    protected abstract void UnsubscribeFromEvents();

    protected void SetSlotParent(Transform newParent)
    {
        slotObject.SetParent(newParent);
    }

    private void OnDestroy()
    {
        transform.DOKill();
    }
}