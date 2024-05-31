using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.UI;

public class NeutralBox : MonoSingleton<NeutralBox>
{
    [SerializeField] private List<PlacementPoint> placementPoints = new();

    private void Start()
    {
        ConveyorManager.instance.NewConveyorBoxCameEvent += OnNewConveyorBoxCameEvent;
    }

    private void OnNewConveyorBoxCameEvent(ConveyorBoxController conveyorBox)
    {
        ColorEnum boxColor = conveyorBox.GetColorEnum();

        for (int i = 0; i < placementPoints.Count; i++)
        {
            BottleController bottle = placementPoints[i].GetBottle();
            if (bottle && bottle.GetColorEnum() == boxColor)
            {
                if (conveyorBox.GetAvailablePoint() != null)
                {
                    PlacementPoint conveyorBoxPoint = conveyorBox.GetAvailablePoint();

                    void TweenCallback()
                    {
                        conveyorBox.OnBottleArrived();
                    }

                    bottle.PerformMoving(conveyorBoxPoint, TweenCallback);
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