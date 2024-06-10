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
    private Quaternion _initRot;
    private PlacementPoint _currentPoint;

    protected virtual void Awake()
    {
        isActive = true;
    }

    protected void OnMouseDown()
    {
        if (!GameManager.instance.isLevelActive) return;
        if (Rotater.instance.isRotating) return;
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
        int iterator = 0;
        while (iterator < 3)
        {
            Taptic.Light();
            iterator++;
        }

        RealMoveStartedEvent?.Invoke();
        isActive = false;
        Vector3 movementDirection = transform.up * 1.5f;
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

        transform.SetParent(targetPoint.transform);
        transform.forward = targetPoint.transform.forward;
        Vector3 virtualPos = new(targetPoint.transform.position.x, targetPoint.transform.position.y + 1f,
            targetPoint.transform.position.z);
        Sequence sq = DOTween.Sequence();
        sq.Append(
            transform.DOMove(virtualPos, .35f).OnComplete(() => { shouldRotate = true; }));
        sq.Append(transform.DOMove(targetPoint.transform.position, .25f).OnStart(() => { shouldRotate = false; }));
        sq.OnComplete(() => { newHole?.OnBoltArrived(); });
    }

    private void FakeMove()
    {
        PerformFakeMove = true;
        Vector3 movementDirection = transform.up * 1.5f;
        Vector3 targetPosition = transform.position + movementDirection;
        _startPos = transform.position;
        _initRot = transform.rotation;
        _fakeMoveTween = transform.DOMove(targetPosition, .5f).SetDelay(0.15f);
        _fakeMoveTween.Play();
    }

    public void StopFakeMove(BaseBoltClass collidedBolt)
    {
        Taptic.Medium();
        shouldRotate = false;
        PerformFakeMove = false;
        _fakeMoveTween.Kill();
        collidedBolt?.OnCollided();

        transform.DOMove(_startPos, .5f);
        
        DOTween.To(() => 0f, x => { transform.rotation = Quaternion.Lerp(transform.rotation, _initRot, x); }, 1f, 1f)
            .OnComplete(() =>
            {
                AnyMoveSequenceEndedEvent?.Invoke();
                isPicked = false;
            });
    }

    private void OnCollided()
    {
        transform.DOMove(transform.position + transform.up * 0.125f, .125f).SetLoops(2, LoopType.Yoyo);
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