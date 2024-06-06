using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseHoleClass : MonoBehaviour
{
    [SerializeField] protected List<PlacementPoint> placementPoints = new();
    
    public PlacementPoint GetAvailablePoint()
    {
        foreach (PlacementPoint point in placementPoints)
        {
            if (!point.isOccupied)
                return point;
        }

        return null;
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

    public abstract void OnBoltArrived();
}
