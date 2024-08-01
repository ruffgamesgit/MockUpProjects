using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class DoubleFoodHolder : MonoBehaviour
{
    [Header("Config")] [SerializeField] private float dragThreshold;
    [SerializeField] float zOffset;

    [Header("References")] [SerializeField]
    private List<Transform> points;

    [SerializeField] private List<FoodController> foods;

    [Header("Debug")] private bool _isDragged;
    private Vector3 _initialMousePosition;
    private Vector3 _offset;
    [SerializeField] private Vector3 _initPos;
    private GridCell _cellBelow;
    GridCell _neighborHighlightedCell;

    private void Awake()
    {
        foreach (FoodController food in foods)
        {
            food.DisableCollider();
        }
    }

    private void Start()
    {
        _initPos = transform.position;
    }

    #region Mouse Events

    private void OnMouseDown()
    {
        _initialMousePosition = Input.mousePosition;
        _offset = transform.position - GetMouseWorldPosition() + new Vector3(0, 0, zOffset);
    }

    private void OnMouseDrag()
    {
        float distance = Vector3.Distance(_initialMousePosition, Input.mousePosition);

        if (distance > dragThreshold)
        {
            _isDragged = true;
            transform.position = GetMouseWorldPosition() + _offset;
        }

        #region Handle Highlighting

        FoodController firstFood = foods[0];

        Ray ray = new(firstFood.transform.position, Vector3.down);

        if (Physics.Raycast(ray, out RaycastHit hit) && hit.transform.TryGetComponent(out GridCell cell))
        {
            if (!cell.isOccupied)
            {
                if (_cellBelow == cell) return;

                _cellBelow?.indicatorController.DisableIndicator();
                _neighborHighlightedCell?.indicatorController.DisableIndicator();
                _cellBelow = cell;

                _neighborHighlightedCell = DirectionExtensions.GetCellByDirectionEnum(
                    DirectionExtensions.GetSiblingDirection(
                        points.IndexOf(firstFood.transform.parent)),
                    _cellBelow.GetCoordinates());

                if (_neighborHighlightedCell && _neighborHighlightedCell.isOccupied) return;

                cell.indicatorController.EnableIndicator();
                _neighborHighlightedCell?.indicatorController.EnableIndicator();
            }
            else
            {
                _cellBelow?.indicatorController.DisableIndicator();
                _neighborHighlightedCell?.indicatorController.DisableIndicator();
                _cellBelow = null;
            }
        }
        else
        {
            _cellBelow?.indicatorController.DisableIndicator();
            _neighborHighlightedCell?.indicatorController.DisableIndicator();
            _cellBelow = null;
        }

        #endregion
    }

    private void OnMouseUp()
    {
        if (_isDragged)
        {
            PerformRaycast();
        }
        else
        {
            MoveFoods();
        }

        _isDragged = false;
    }

    #endregion

    private void MoveFoods()
    {
        foreach (FoodController food in foods)
        {
            food.transform.SetParent(GetNextPoint(food.transform.parent));
            food.transform.DOLocalMove(Vector3.zero, .1f);
        }
    }

    private void PerformRaycast()
    {
        List<GridCell> targetCells = new();

        foreach (FoodController food in foods)
        {
            Ray ray = new Ray(food.transform.position, Vector3.down);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.transform.TryGetComponent(out GridCell cell))
                {
                    if (!cell.isOccupied)
                    {
                        if (!targetCells.Contains(cell))
                            targetCells.Add(cell);
                    }
                }
            }
        }

        _cellBelow?.indicatorController.DisableIndicator();
        _neighborHighlightedCell?.indicatorController.DisableIndicator();

        if (targetCells.Count < 2)
        {
            GetReleased();
            return;
        }

        for (int i = 0; i < foods.Count; i++)
        {
            FoodController f = foods[i];
            GridCell c = targetCells[i];

            f.transform.SetParent(null);
            f.GetPlaced(c);
        }

        ConveyorManager.instance.OnFoodPlaced(transform);
        Destroy(gameObject);
    }

    #region GETTERS

    private void GetReleased()
    {
        transform.position = _initPos;
    }

    Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = Camera.main!.nearClipPlane;
        return Camera.main.ScreenToWorldPoint(mousePoint);
    }

    Transform GetNextPoint(Transform currentPoint)
    {
        int currentindex = points.IndexOf(currentPoint);

        currentindex = currentindex + 1 > points.Count - 1 ? 0 : currentindex + 1;

        return points[currentindex];
    }

    #endregion
}