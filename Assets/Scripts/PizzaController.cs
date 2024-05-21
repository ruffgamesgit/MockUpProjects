using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

public enum PizzaType
{
    Type1,
    Type2,
    Type3,
    Type4,
}

public class PizzaController : MonoBehaviour
{
    [SerializeField] private PizzaData _pizzaData;
    [HideInInspector] public PlacementPoint placementPoint;

    public void Initialize(PlacementPoint point, PizzaData data)
    {
        placementPoint = point;
        SetPizzaData(data);

        Pickable pickable = GetComponent<Pickable>();
        pickable.SetPickableStatus(placementPoint);
        pickable.OnPicked += OnPicked;
        pickable.OnReleased += OnReleased;
        pickable.OnPlaced += OnPlaced;
    }


    #region Event Subscribers

    void OnReleased()
    {
        GoBackToPoint();
    }

    private void OnPicked()
    {
        placementPoint.SetFree();
        
    }

    private void OnPlaced(PlacementPoint newPoint)
    {
        SetNewPoint(newPoint);
        GoBackToPoint();
    }

    #endregion

    void SetPizzaData(PizzaData data)
    {
        _pizzaData = data;
    }

    public PizzaData GetPizzaData()
    {
        return _pizzaData;
    }

    public void SetNewPoint(PlacementPoint point)
    {
        if (placementPoint != null) placementPoint.SetFree();

        placementPoint = point;
        placementPoint.SetOccupied();
    }

    private void GoBackToPoint()
    {
        transform.DOMove(placementPoint.GetPos(), .25f);
    }
}

[System.Serializable]
public class PizzaData
{
    public PizzaType pizzaType;
    public int level;
}