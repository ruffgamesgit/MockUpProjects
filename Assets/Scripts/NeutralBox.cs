using System.Collections.Generic;
using UnityEngine;

public class NeutralBox : MonoSingleton<NeutralBox>
{
    [SerializeField] private List<PlacementPoint> placementPoints = new();

    private void Start()
    {
        ConveyorManager.instance.NewConveyorBoxCameEvent += OnNewConveyorBoxCameEvent;
    }

    public void OnBottleArrived()
    {
        List<BottleController> bottles = new();
        for (int i = 0; i < placementPoints.Count; i++)
        {
            if (placementPoints[i].GetBottle())
                bottles.Add(placementPoints[i].GetBottle());
        }

        if (bottles.Count != placementPoints.Count) return;

        bool hasMatch = false;
        ColorEnum conveyorBoxColor = ConveyorManager.instance.GetCurrentBox().GetColorEnum();
        for (int i = 0; i < bottles.Count; i++)
        {
            if (bottles[i].GetColorEnum() == conveyorBoxColor)
                hasMatch = true;
        }

        if (hasMatch) return;
        
        Debug.LogWarning("No Placeable point left, FAIL");
    }

    private void OnNewConveyorBoxCameEvent(ConveyorBoxController conveyorBox)
    {
        ColorEnum boxColor = conveyorBox.GetColorEnum();
        List<BottleController> matchedBottles = new();

        for (int i = 0; i < placementPoints.Count; i++)
        {
            BottleController bottle = placementPoints[i].GetBottle();
            if (bottle && bottle.GetColorEnum() == boxColor)
            {
                matchedBottles.Add(bottle);
            }
        }

        int lastIndex = matchedBottles.Count >= 3 ? 3 : matchedBottles.Count;
        for (int i = 0; i < matchedBottles.Count; i++)
        {
            if (conveyorBox.GetAvailablePoint() != null)
            {
                PlacementPoint conveyorBoxPoint = conveyorBox.GetAvailablePoint();
                if (i == lastIndex - 1)
                    matchedBottles[i].PerformMoving(conveyorBoxPoint, conveyorBox);
                else
                {
                    matchedBottles[i].PerformMoving(conveyorBoxPoint);
                }
            }
        }
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
}