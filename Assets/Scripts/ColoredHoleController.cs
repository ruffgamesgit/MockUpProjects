using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ColoredHoleController : MonoBehaviour
{
    [SerializeField] private ColourEnum colorEnum;
    [SerializeField] private List<PlacementPoint> placementPoints;
    
    
    public ColourEnum GetColorEnum()
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
        //    ConveyorManager.instance.RemoveOldBringNew();
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
