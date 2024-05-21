using System;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;
using UnityEngine.Serialization;

public class Pickable : MonoBehaviour
{
    public event Action OnPicked;
    public event Action OnReleased;
    public event Action<PlacementPoint> OnPlaced;

    [SerializeField] LayerMask columnLayer;
    [Header("Debug")] public bool IsPicked;
    public bool CanPickable;

    public void SetPickableStatus(PlacementPoint point)
    {
        ColumnController column = point.GetColumn();
        PlacementPoint lastOccupiedPoint = column.GetLastOccupiedPoint();
        PlacementPoint firstPoint = column.GetPoints()[0];

        CanPickable = point == lastOccupiedPoint || point == firstPoint;
    }

    public void GetPicked()
    {
        IsPicked = true;
        OnPicked?.Invoke();
    }

    public void GetReleased()
    {
        IsPicked = false;
        OnReleased?.Invoke();
    }

    private void GoToPoint(PlacementPoint point)
    {
        // Invoke an event named like "OnPlacedNewPoint" and subscribe this event from PizzaController
        IsPicked = false;
        OnPlaced?.Invoke(point);
        SetPickableStatus(point);
    }

    void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            if (!IsPicked) return;
            IsPicked = false;

            ColumnController column = GetCellFront();

            if (column != null && column.GetAvailablePoint() is not null)
            {
                GoToPoint(column.GetAvailablePoint());
            }
            else
            {
                GetReleased();
            }
        }
    }


    ColumnController GetCellFront()
    {
        RaycastHit hit;
        Ray ray = new Ray(transform.position + (Vector3.up), Vector3.down);

        Debug.DrawRay(ray.origin, ray.direction * 10f, Color.red);

        if (Physics.Raycast(ray, out hit, 100, columnLayer))
        {
            if (hit.collider.transform.TryGetComponent(out ColumnController column))
            {
                return column;
            }
        }

        return null;
    }
}