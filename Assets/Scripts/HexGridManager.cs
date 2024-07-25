using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class HexGridManager : MonoSingleton<HexGridManager>
{
    public GameObject hexPrefab;
    public int gridWidth = 10;
    public int gridHeight = 10;

    [SerializeField] private float hexWidth;
    [SerializeField] private float hexHeight;
    [SerializeField] private float hexVertOffset;
    [SerializeField] private float hexHorizOffset;
    private readonly Dictionary<Vector2Int, GridCell> _hexDict = new Dictionary<Vector2Int, GridCell>();

    public List<FoodController> foodsOnGrid;

    protected override void Awake()
    {
        base.Awake();
        //   hexWidth = 1f; // Adjust based on your hexagon prefab's size
        //   hexHeight = Mathf.Sqrt(3) / 2 * hexWidth;
        //_hexVertOffset = hexHeight;
        //  _hexHorizOffset = 1.5f * hexWidth;

        GenerateGrid();
        AssignNeighbors();
    }

    #region Generation Region

    private void GenerateGrid()
    {
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                float xPos = x * hexHorizOffset;
                float yPos = y * hexVertOffset;

                // Offset every other row
                if (y % 2 == 1)
                {
                    xPos += hexWidth;
                }

                Vector3 hexPosition = new(xPos, 0, yPos);
                GameObject hex = Instantiate(hexPrefab, hexPosition, Quaternion.identity, transform);
                hex.name = $"Hex_{x}_{y}";

                Vector2Int hexCoordinates = new(x, y);
                GridCell hexCell = hex.transform.GetComponent<GridCell>();
                hexCell.SetCoordinates(hexCoordinates);
                _hexDict.Add(hexCoordinates, hexCell);
            }
        }
    }

    private void AssignNeighbors()
    {
        foreach (var hexPair in _hexDict)
        {
            Vector2Int coord = hexPair.Key;
            GridCell hex = hexPair.Value;

            Vector2Int[] neighborOffsets = (coord.y % 2 == 0) ? EvenRowOffsets : OddRowOffsets;
            foreach (Vector2Int offset in neighborOffsets)
            {
                Vector2Int neighborCoord = coord + offset;
                if (_hexDict.TryGetValue(neighborCoord, value: out GridCell value))
                {
                    hex.neighbours.Add(value);
                }
            }
        }
    }

    private static readonly Vector2Int[] EvenRowOffsets =
    {
        new(0, 2), new(0, 1), new(0, -1),
        new(0, -2), new(-1, -1), new(-1, 1)
    };

    private static readonly Vector2Int[] OddRowOffsets =
    {
        new(0, 2), new(1, 1), new(1, -1),
        new(0, -2), new(0, -1), new(0, 1)
    };

    #endregion

    public void CheckIfFoodMatches(FoodController lastPlacedFood, bool tripleMatch = false)
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