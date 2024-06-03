using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ConveyorBoxController : MonoBehaviour
{
    [SerializeField] private ColorEnum colorEnum = ColorEnum.NONE;
    [SerializeField] private GameObject selectedMesh;
    [SerializeField] private List<PlacementPoint> placementPoints = new();

    public void Initialize(ColorEnum assignedColor = ColorEnum.NONE)
    {
        colorEnum = assignedColor;
        SetMesh();
    }

    void SetMesh()
    {
        Transform firstChild = transform.GetChild(0);
        switch (colorEnum)
        {
            case ColorEnum.NONE:
                selectedMesh = firstChild.GetChild(0).gameObject;
                break;
            case ColorEnum.BLUE:
                selectedMesh = firstChild.GetChild(1).gameObject;
                break;
            case ColorEnum.GREEN:
                selectedMesh = firstChild.GetChild(2).gameObject;
                break;
            case ColorEnum.ORANGE:
                selectedMesh = firstChild.GetChild(3).gameObject;
                break;
            case ColorEnum.PINK:
                selectedMesh = firstChild.GetChild(4).gameObject;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        for (int i = 0; i < transform.GetChild(0).childCount; i++)
        {
            if (selectedMesh != firstChild.GetChild(i).gameObject)
                Destroy(firstChild.GetChild(i).gameObject);
        }
    }

    public ColorEnum GetColorEnum()
    {
        return colorEnum;
    }

    public PlacementPoint GetAvailablePoint()
    {
        for (int i = 0; i < placementPoints.Count; i++)
        {
            if (!placementPoints[i].isOccupied)
                return placementPoints[i];
        }

        return null;
    }

    public void OnBottleArrived()
    {
        if (GetOccupiedPointCount() == placementPoints.Count)
        {
            Debug.LogWarning("Conveyor box is full");
            ConveyorManager.instance.RemoveOldBringNew();
            Sequence sq = DOTween.Sequence();
            sq.Append(transform.DOMoveX(transform.position.x + 7, .25f));
            sq.Append(transform.DOScale(Vector3.zero, .25f).OnComplete(() => Destroy(gameObject, .5f)));
        }
    }

    public int GetOccupiedPointCount()
    {
        int counter = 0;
        for (int i = 0; i < placementPoints.Count; i++)
        {
            if (placementPoints[i].isOccupied)
                counter++;
        }

        return counter;
    }
}