using UnityEngine;

public class PlacementPoint : MonoBehaviour
{
    [Header("Debug")] public NumberObject currentNumberObject;
    public bool isOccupied;

    public void SetOccupied(NumberObject numberObject)
    {
        isOccupied = true;
        currentNumberObject = numberObject;
    }

    public void SetFree()
    {
        isOccupied = false;
        currentNumberObject = null;
    }

    public NumberObject GetNumberObject()
    {
        return currentNumberObject;
    }

    public int GetIndex()
    {
        return PointManager.instance.placementPoints.IndexOf(this);
    }
}