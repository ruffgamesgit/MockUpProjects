using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoSingleton<GridManager>
{
    [Header("Debug")] public List<GridCell> gridPlan;

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

    public void CheckIfFoodMatches(FoodController placedFood)
    {
        GridCell originCell = placedFood.GetCell();
        GridCell neighborMathcedCell = null;
        List<GridCell> neighbours = originCell.neighbours;
        bool hasMatch = false;
        for (int i = 0; i < neighbours.Count; i++)
        {
            if (!neighbours[i].currentFood) continue;

            FoodController neighborFood = neighbours[i].currentFood;
            if (placedFood.GetFoodData().foodType != neighborFood.GetFoodData().foodType) continue;
            if (placedFood.GetFoodData().level != neighborFood.GetFoodData().level) continue;
            neighborMathcedCell = neighbours[i];
            hasMatch = true;
            break;
        }

        if (hasMatch)
        {
            neighborMathcedCell?.currentFood.IncrementSelf();
            placedFood.Disappear();

            if (neighborMathcedCell?.currentFood.GetFoodData().level < 2)
                CheckIfFoodMatches(neighborMathcedCell.currentFood);
        }
    }
}