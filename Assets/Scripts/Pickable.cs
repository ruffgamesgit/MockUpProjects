using System;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

public class Pickable : MonoBehaviour
{
    public event Action OnPicked;
    public event Action<PlacementPoint> OnReleased;
    public event Action<PlacementPoint,TweenCallback> OnPlaced;

    [SerializeField] LayerMask columnLayer;
    [Header("Debug")] public bool IsPicked;
    public bool CanPickable;
    private PlacementPoint _point;
    private PizzaController _pizzaController;

    private void Awake()
    {
        _pizzaController = transform.GetComponent<PizzaController>();
    }

    private void Start()
    {
        SetPickableStatus();
        InputManager.instance.OnPickablePlacedEvent += SetPickableStatus;
    }

    public void SetPoint(PlacementPoint p)
    {
        _point = p;
    }

    private void SetPickableStatus()
    {
        ColumnController column = _point.GetColumn();
        PlacementPoint lastOccupiedPoint = column.GetLastOccupiedPoint();
        PlacementPoint firstPoint = column.GetPoints()[0];
        
        if (_point == firstPoint || _point == lastOccupiedPoint)
        {
            CanPickable = true;
        }
    }

    public void GetPicked()
    {
        IsPicked = true;
        OnPicked?.Invoke();
    }

    public void GetReleased(PlacementPoint point)
    {
        IsPicked = false;
        OnReleased?.Invoke(point);
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private void GetPlaced(PlacementPoint point)
    {
        IsPicked = false;
        if (_point is not null) _point.SetFree();
        
        SetPoint(point);
        OnPlaced?.Invoke(point , () =>  point.GetColumn().CheckInnerSort());
        InputManager.instance.TriggerOnPickablePlacedEvent();
    }

    void OnMouseUp()
    {
        if (!IsPicked) return;
        IsPicked = false;

        ColumnController column = GetColumnBelow();
        PlacementPoint point = column.GetAvailablePoint();

        if (_point.GetColumn() != column && column != null && point is not null)
        {
            GetPlaced(point);
        }
        else
        {
            GetReleased(_point);
        }
    }

    public PlacementPoint GetPoint()
    {
        return _point;
    }

    ColumnController GetColumnBelow()
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