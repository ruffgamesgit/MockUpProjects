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
    private BoardBoxController _initialBoardBoardBox;
    private PlacementPoint _initPoint;
    private GameObject _selectedMesh;

    public void Initialize(ColorEnum _colorEnum, PlacementPoint placementPoint, BoardBoxController initialBoardBox)
    {
        transform.SetParent(placementPoint.transform);
        colorEnum = _colorEnum;
        gameObject.name = "Bottle_" + colorEnum;
        _initialBoardBoardBox = initialBoardBox;
        _initPoint = placementPoint;
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

    public void GetPicked()
    {
        // go for the conveyor's current box
        bool bottleLeftTheBox = false;
        ConveyorBoxController currentConveyorBox = ConveyorManager.instance.GetCurrentBox();
        if (currentConveyorBox.GetColorEnum() == GetColorEnum())
        {
            if (currentConveyorBox.GetAvailablePoint())
            {
                _initPoint.SetFree();
                PlacementPoint point = currentConveyorBox.GetAvailablePoint();
                transform.SetParent(point.transform);
                point.SetOccupied();
                currentConveyorBox.OnBottleArrived();
                bottleLeftTheBox = true;

                GoOtherBox(point.transform.position);
            }
        }
        else
        {
            bottleLeftTheBox = true;
        }


        if (bottleLeftTheBox)
            _initialBoardBoardBox.OnBottleLeft();
    }

    public ColorEnum GetColorEnum()
    {
        return colorEnum;
    }

    void GoOtherBox(Vector3 targePos)
    {
        transform.DOMove(targePos, .5f);
    }

    public void SetMeshLayer(string layerIndex)
    {
        _selectedMesh.layer = LayerMask.NameToLayer(layerIndex);
    }

    public bool IsPickable()
    {
        return transform.parent.parent.GetComponent<BoardBoxController>().IsPickable();
    }
}