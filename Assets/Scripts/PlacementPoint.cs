using UnityEngine;
using UnityEngine.Serialization;

public class PlacementPoint : MonoBehaviour
{
    [Header("Debug")]
    public BaseBoltClass currentBolt;
    public bool isOccupied;

    public void SetOccupied(BaseBoltClass bolt)
    {
        isOccupied = true;
        currentBolt = bolt;
    }

    public void SetFree()
    {
        isOccupied = false;
        currentBolt = null;
    }

    public BaseBoltClass GetBolt()
    {
        return currentBolt;
    }
}