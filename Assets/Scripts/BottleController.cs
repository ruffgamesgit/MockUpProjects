using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class BottleController : MonoBehaviour
{
    [Header("References")] [SerializeField]
    private List<GameObject> meshes;

    [Header("Debug")] [SerializeField] private ColorEnum colorEnum;
    public bool isPicked;
    [SerializeField] private PlacementPoint _currentPoint;
    [SerializeField] private GameObject _selectedMesh;
    private BoardBoxController _initialBoardBoardBox;

    public void Initialize(ColorEnum _colorEnum, PlacementPoint placementPoint, BoardBoxController initialBoardBox)
    {
        transform.SetParent(placementPoint.transform);
        colorEnum = _colorEnum;
        gameObject.name = "Bottle_" + colorEnum;
        _initialBoardBoardBox = initialBoardBox;
        _currentPoint = placementPoint;
        _currentPoint.SetOccupied(this);
        SetMesh();
    }

    void SetMesh()
    {
        Transform firstChild = transform.GetChild(0);
        switch (colorEnum)
        {
            case ColorEnum.NONE:
                Debug.LogWarning("ColorEnum is NONE, no material will be set.");
                break;
            case ColorEnum.BLUE:
                _selectedMesh = firstChild.GetChild(0).gameObject;
                break;
            case ColorEnum.GREEN:
                _selectedMesh = firstChild.GetChild(1).gameObject;
                break;
            case ColorEnum.ORANGE:
                _selectedMesh = firstChild.GetChild(2).gameObject;
                break;
            case ColorEnum.PINK:
                _selectedMesh = firstChild.GetChild(3).gameObject;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        for (int i = 0; i < transform.GetChild(0).childCount; i++)
        {
            if (_selectedMesh != firstChild.GetChild(i).gameObject)
                Destroy(firstChild.GetChild(i).gameObject);
        }
    }

    // ReSharper disable Unity.PerformanceAnalysis
    public void GetPicked()
    {
        // go for the conveyor's current box
        bool bottleLeftTheBox = false;
        ConveyorBoxController currentConveyorBox = ConveyorManager.instance.GetCurrentBox();
        if (currentConveyorBox.GetColorEnum() == GetColorEnum())
        {
            if (currentConveyorBox.GetAvailablePoint())
            {
                PlacementPoint newPoint = currentConveyorBox.GetAvailablePoint();
                PerformMoving(newPoint, currentConveyorBox);
                bottleLeftTheBox = true;
            }
        }
        else
        {
            if (NeutralBox.instance.GetAvailablePoint() is not null)
            {
                PlacementPoint newPoint = NeutralBox.instance.GetAvailablePoint();
                PerformMoving(newPoint, null, true);
                bottleLeftTheBox = true;
            }
        }


        if (bottleLeftTheBox)
            _initialBoardBoardBox.OnBottleLeft();
    }

    public void PerformMoving(PlacementPoint newPoint, ConveyorBoxController conveyorBox = null,
        bool isTargetNeutralBox = false)
    {
        _currentPoint.SetFree();
        _currentPoint = newPoint;
        newPoint.SetOccupied(this);
        transform.SetParent(_currentPoint.transform);
        bool triggerOnComplete = conveyorBox && conveyorBox.GetOccupiedPointCount() == 3;
        if (isTargetNeutralBox) NeutralBox.instance.OnBottleArrived();

        Sequence sq = DOTween.Sequence();
        sq.Append(transform.DOJump(newPoint.transform.position, 10, 1,
            .5f)).OnComplete(() =>
        {
            if (triggerOnComplete)
            {
                conveyorBox?.OnBottleArrived();
            }
        });
        sq.Join(transform.DOScale(transform.lossyScale * 2, .3f).SetLoops(2, LoopType.Yoyo));
        sq.Play();
    }

    public ColorEnum GetColorEnum()
    {
        return colorEnum;
    }

    public void SetMeshLayer(string layerIndex)
    {
        _selectedMesh.layer = LayerMask.NameToLayer(layerIndex);
    }

    // ReSharper disable Unity.PerformanceAnalysis
    public bool IsPickable()
    {
        bool pickable = false;
        if (transform.parent.parent.GetComponent<BoardBoxController>() != null)
            pickable = transform.parent.parent.GetComponent<BoardBoxController>().IsPickable();

        return pickable;
    }
}