using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

public class Pickable : MonoBehaviour
{
    public event Action OnPicked;
    public event Action<PlacementPoint> OnReleased;
    public event Action<PlacementPoint, TweenCallback> OnPlaced;

    [Header("Config")] [SerializeField] LayerMask columnLayer;

    [Header("Debug")] public bool isPicked;
    public bool canPickable;
    [SerializeField] private PlacementPoint currentPoint;
    private PizzaController _pizzaController;

    private void Awake()
    {
        _pizzaController = transform.GetComponent<PizzaController>();
    }

    private void Start()
    {
        SetPickableStatus();
    }

    public void SetPoint(PlacementPoint newPoint, bool checkForThePickableStatus = false)
    {
        currentPoint = newPoint;
        if (checkForThePickableStatus)
            SetPickableStatus();
    }

    private void SetPickableStatus()
    {
        IEnumerator CheckingRoutine()
        {
            yield return null;

            ColumnController column = currentPoint.GetColumn();
            PlacementPoint lastOccupiedPoint = column.GetLastOccupiedPoint();
            PlacementPoint firstPoint = column.GetPoints()[0];

            if (currentPoint == firstPoint || currentPoint == lastOccupiedPoint)
            {
                canPickable = true;
            }
            else
            {
                canPickable = false;
            }
        }

        if (currentPoint is not null)
            StartCoroutine(CheckingRoutine());
        else
            canPickable = false;
    }


    public void GetPicked()
    {
        isPicked = true;
        OnPicked?.Invoke();
    }

    public void GetReleased(PlacementPoint _point)
    {
        isPicked = false;
        OnReleased?.Invoke(_point);
    }

    public bool CanBeLeaderPizza()
    {
        return currentPoint.GetIndex() == 0 && currentPoint.GetColumn().GetPoints().Count > 1;
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private void GetPlaced(PlacementPoint newPoint)
    {
        isPicked = false;

        SetPrevPickableStatus(newPoint);

        if (currentPoint is not null) currentPoint.SetFree();

        SetPoint(newPoint);

        OnPlaced?.Invoke(newPoint, () => newPoint.GetColumn().CheckInnerSort(lastPlacedPizza: _pizzaController));
        InputManager.instance.TriggerOnPickablePlacedEvent();
    }

    public void SetPrevPickableStatus(PlacementPoint newPoint = null)
    {
        // newPoint = null means the pizza is disappearing the previous one should be pickable
        if (newPoint != currentPoint)
        {
            int prevIndex = currentPoint.GetColumn().GetPoints().IndexOf(currentPoint) - 1;
            if (prevIndex < 0) return;
            Pickable prevPickable = currentPoint.GetColumn().GetPoints()[prevIndex].GetPizza().GetPickable();
            prevPickable.canPickable = true;
        }
    }

    public PlacementPoint GetPoint()
    {
        return currentPoint;
    }

    void OnMouseUp()
    {
        if (!isPicked) return;
        isPicked = false;

        ColumnController column = GetColumnBelow();
        if (column == null) return;

        PlacementPoint _point = column.GetAvailablePoint();

        if (currentPoint.GetColumn() != column && column != null && _point is not null)
        {
            GetPlaced(_point);
        }
        else
        {
            GetReleased(currentPoint);
        }
    }

    ColumnController GetColumnBelow()
    {
        RaycastHit hit;
        Ray ray = new Ray(transform.position + (Vector3.up), Vector3.down);

        if (Physics.Raycast(ray, out hit, 100, columnLayer))
        {
            if (hit.collider.transform.TryGetComponent(out ColumnController column))
            {
                return column;
            }
        }

        return null;
    }

    public async void FollowTheLeaderPizza(Vector3 newPos, int index)
    {
        await UniTask.Delay(index * 50);

        transform.position = newPos;
    }
}