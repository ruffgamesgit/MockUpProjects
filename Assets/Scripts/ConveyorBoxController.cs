using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine; 
public class ConveyorBoxController : MonoBehaviour
{
    [SerializeField] private ColorEnum colorEnum = ColorEnum.NONE;
    [SerializeField] private GameObject selectedMesh;
    [SerializeField] private List<PlacementPoint> placementPoints = new List<PlacementPoint>();

    public void Initialize(ColorEnum assignedColor = ColorEnum.NONE)
    {
        colorEnum = assignedColor;
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
        bool isFull = true;
        for (int i = 0; i < placementPoints.Count; i++)
        {
            if (!placementPoints[i].isOccupied)
                isFull = false;
        }

        if (isFull)
        {
            ConveyorManager.instance.BringNewBox();
            gameObject.SetActive(false);
        }
    }
}