using UnityEngine;
using UnityEngine.Serialization;

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
}