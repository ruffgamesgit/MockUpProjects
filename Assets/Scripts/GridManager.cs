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

    public void CheckIfFoodMatches([CanBeNull] FoodController lastPlacedFood, bool tripleMatch = false)
    {
        GridCell lastPlacedCell = lastPlacedFood.GetCell();
        GridCell neighborMathcedCell = null;
        List<GridCell> neighbours = lastPlacedCell.neighbours;
        bool hasMatch = false;

        if (!tripleMatch)
        {
            for (int i = 0; i < neighbours.Count; i++)
            {
                if (!neighbours[i].currentFood) continue;
                if (neighbours[i].currentFood.GetFoodData().level >= 2) continue;

                FoodController neighborFood = neighbours[i].currentFood;
                if (lastPlacedFood.GetFoodData().foodType != neighborFood.GetFoodData().foodType) continue;
                if (lastPlacedFood.GetFoodData().level != neighborFood.GetFoodData().level) continue;
                neighborMathcedCell = neighbours[i];
                hasMatch = true;
                break;
            }

            if (!hasMatch) return;

            neighborMathcedCell?.currentFood.IncrementSelf();
            lastPlacedFood.Disappear(lastPlacedCell.GetCenter());

            if (neighborMathcedCell?.currentFood?.GetFoodData().level < 2)
                CheckIfFoodMatches(neighborMathcedCell.currentFood);
        }
        else
        {
            List<FoodController> matchedFoods = new List<FoodController>();

            foreach (GridCell neighborCell in neighbours)
            {
                if (!neighborCell.currentFood) continue;
                if (neighborCell.currentFood.GetFoodData().level >= 2) continue;

                FoodController neighborFood = neighborCell.currentFood;
                if (lastPlacedFood.GetFoodData().foodType != neighborFood.GetFoodData().foodType) continue;
                if (lastPlacedFood.GetFoodData().level != neighborFood.GetFoodData().level) continue;
                if (!matchedFoods.Contains(neighborFood))
                    matchedFoods.Add(neighborFood);
            }

            Debug.Log("Matched food count: " + matchedFoods.Count);

            if (matchedFoods.Count is < 2 and > 0)
            {
                foreach (GridCell neighborCell in neighbours)
                {
                    if (!neighborCell.currentFood) continue;
                    if (neighborCell.currentFood.GetFoodData().level >= 2) continue;
                    if (lastPlacedFood.GetFoodData().foodType !=
                        neighborCell.currentFood.GetFoodData().foodType) continue;
                    if (lastPlacedFood.GetFoodData().level != neighborCell.currentFood.GetFoodData().level) continue;

                    foreach (GridCell cell in neighborCell.neighbours)
                    {
                        if (matchedFoods.Count >= 2) break;

                        if (cell == lastPlacedCell) continue;
                        if (!cell.currentFood) continue;
                        if (cell.currentFood.GetFoodData().level >= 2) continue;
                        if (lastPlacedFood.GetFoodData().foodType != cell.currentFood.GetFoodData().foodType) continue;
                        if (lastPlacedFood.GetFoodData().level != cell.currentFood.GetFoodData().level) continue;

                        FoodController food = cell.currentFood;
                        if (matchedFoods.Contains(food) && food == lastPlacedFood) continue;

                        matchedFoods.Add(food);
                    }
                }
            }

            if (matchedFoods.Count < 2) return;

            lastPlacedFood.IncrementSelf();
            for (int i = 0; i < 2; i++)
            {
                matchedFoods[i].Disappear(lastPlacedCell.GetCenter());
            }

            if (lastPlacedFood?.GetFoodData().level < 2)
                CheckIfFoodMatches(lastPlacedFood, true);
        }
    }

    public void AddFood(FoodController foodController)
    {
        if (foodsOnGrid.Contains(foodController)) return;
        foodsOnGrid.Add(foodController);
    }

    public void RemoveFood(FoodController foodController)
    {
        if (!foodsOnGrid.Contains(foodController)) return;
        foodsOnGrid.Remove(foodController);
    }
}