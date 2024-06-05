using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public abstract class BaseBoltClass : MonoBehaviour
{
    [Header("Base Config")]
    public ColourEnum colourEnum;
    
    [Header("Base References")] 
    [SerializeField] protected List<BaseBoltClass> obstacleBolts;
    
    [Header("Base Debug")] 
    [SerializeField] protected bool isActive;
    [SerializeField] protected bool isPicked;
    [SerializeField] protected bool shouldRotate;
    protected bool PerformFakeMove;
    private const float RotationSpeed = 700f;
    private Tween _fakeMoveTween;
    private Vector3 _startPos;
    private Vector3 _startRot;

    protected virtual void Awake()
    {
        isActive = true;
    }

    protected void OnMouseDown()
    {
        if (isPicked) return;

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
        OnRealMoveStarted();
        isActive = false;

        // Sequence sq = DOTween.Sequence();
        Vector3 movementDirection = transform.up * 5f;
        Vector3 targetPosition = transform.position + movementDirection;

        transform.DOMove(targetPosition, 1f).OnComplete(() =>
        {
            gameObject.SetActive(false);
            shouldRotate = false;
            isPicked = false;
            OnRealMoveEnded();
        });
    }

    protected abstract void OnRealMoveStarted();
    protected abstract void OnRealMoveEnded();

    private void FakeMove()
    {
        PerformFakeMove = true;
        Vector3 movementDirection = transform.up * 5f;
        Vector3 targetPosition = transform.position + movementDirection;
        _startPos = transform.position;
        _startRot = transform.rotation.eulerAngles;
        _fakeMoveTween = transform.DOMove(targetPosition, 2f);
        _fakeMoveTween.Play();
    }

    public void StopFakeMove()
    {
        shouldRotate = false;
        PerformFakeMove = false;
        _fakeMoveTween.Kill();
        transform.DOMove(_startPos, .5f);
        transform.DORotate(_startRot, .5f).OnComplete(() => { isPicked = false; });
    }

    protected virtual void Update()
    {
        if (shouldRotate)
        {
            transform.Rotate(Vector3.up, 1 * RotationSpeed * Time.deltaTime);
        }
    }

    public abstract bool CanPerformMoving();

    public bool IsBoltActive()
    {
        return isActive;
    }
}