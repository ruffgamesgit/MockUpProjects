using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class GridManager : MonoSingleton<GridManager>
{
    [Header("Debug")] public List<GridCell> gridPlan;
    public List<FoodController> foodsOnGrid;

    protected override void Awake()
    {
        base.Awake();
        for (int i = 0; i < transform.childCount; i++)
        {
            GridCell cell = transform.GetChild(i).GetComponent<GridCell>();
            if ((cell.gameObject.activeInHierarchy))
                gridPlan.Add(cell);
        }
    }

    public GridCell GetClosestGridCell(Vector3 from)
    {
        if (gridPlan == null || gridPlan.Count == 0)
        {
            Debug.LogWarning("GridPlan list is empty or null!");
            return null;
        }

        GridCell closestCell = null;
        float closestDistance = Mathf.Infinity;

        for (int i = 0; i < gridPlan.Count; i++)
        {
            GridCell cell = gridPlan[i];
            float distance = Vector3.Distance(cell.transform.position, from);

            if (distance < closestDistance)
            {
                closestCell = cell;
                closestDistance = distance;
            }
        }

        return closestCell;
    }

    public GridCell GetGridCellByCoordinates(Vector2Int coordinates)
    {
        if (gridPlan == null || gridPlan.Count == 0)
        {
            return null;
        }

        for (int i = 0; i < gridPlan.Count; i++)
        {
            GridCell cell = gridPlan[i];
            if (cell.GetCoordinates() == coordinates)
            {
                return cell;
            }
        }

        return null;
    }

  
}