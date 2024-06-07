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

    [Header("Base References")] [SerializeField]
    protected List<BaseBoltClass> obstacleBolts;

    [Header("Base Debug")] [SerializeField]
    protected bool isActive;

    [SerializeField] protected bool isPicked;
    [SerializeField] protected bool shouldRotate;
    protected bool PerformFakeMove;
    private const float RotationSpeed = 700f;
    private Tween _fakeMoveTween;
    private Vector3 _startPos;
    private Vector3 _startRot;
    private PlacementPoint _currentPoint;

    protected virtual void Awake()
    {
        isActive = true;
    }

    protected void OnMouseDown()
    {
        if (!IsPickable()) return;
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
        RealMoveStartedEvent?.Invoke();
        isActive = false;
        Vector3 movementDirection = transform.up * 2.5f;
        Vector3 targetPosition = transform.position + movementDirection;

        transform.DOMove(targetPosition, .5f).SetDelay(0.15f).OnComplete(() =>
        {
            shouldRotate = false;
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
        ColoredHole currentHole = HoleManager.instance.GetCurrentHole();

        if (ColourUtility.CheckIfColorsMatch(colourEnum, currentHole.GetColorEnum()))
        {
            if (currentHole.GetAvailablePoint() != null)
            {
                GoToPoint(currentHole.GetAvailablePoint(), currentHole);
            }
        }
        else
        {
            if (NeutralHole.instance.GetAvailablePoint())
            {
                GoToPoint(NeutralHole.instance.GetAvailablePoint());
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

        transform.SetParent(targetPoint.transform);
        transform.forward = targetPoint.transform.forward;
        transform.DOMove(targetPoint.transform.position, .25f).OnComplete(() => { newHole?.OnBoltArrived(); });
    }

    private void FakeMove()
    {
        PerformFakeMove = true;
        Vector3 movementDirection = transform.up * 2.5f;
        Vector3 targetPosition = transform.position + movementDirection;
        _startPos = transform.position;
        _startRot = transform.rotation.eulerAngles;
        _fakeMoveTween = transform.DOMove(targetPosition, .5f).SetDelay(0.15f);
        _fakeMoveTween.Play();
    }

    public void StopFakeMove()
    {
        shouldRotate = false;
        PerformFakeMove = false;
        _fakeMoveTween.Kill();
        transform.DOMove(_startPos, .5f);
        transform.DORotate(_startRot, .5f).OnComplete(() =>
        {
            AnyMoveSequenceEndedEvent?.Invoke();
            isPicked = false;
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
}