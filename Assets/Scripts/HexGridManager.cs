using System.Collections.Generic;
using UnityEngine;

public class HexGridManager : MonoSingleton<HexGridManager>
{
    [Header("References")] public GameObject hexPrefab;

    [Header("Config")] public int gridWidth = 10;
    public int gridHeight = 10;
    [SerializeField] private int desiredInitialFoodCount;

    [Header("Debug")] public List<FoodController> foodsOnGrid;
    [SerializeField] List<GridCell> allCells = new();
    private const float HexWidth = .75f;
    private const float HexHeight = .6f;
    private const float HexVertOffset = .43f;
    private const float HexHorizOffset = 1.5f;
    private readonly Dictionary<Vector2Int, GridCell> _hexDict = new();


    protected override void Awake()
    {
        base.Awake();
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
                float xPos = x * HexHorizOffset;
                float yPos = y * HexVertOffset;

                // Offset every other row
                if (y % 2 == 1)
                {
                    xPos += HexWidth;
                }

                Vector3 hexPosition = new(xPos, 0, yPos);
                GameObject hex = Instantiate(hexPrefab, hexPosition, Quaternion.identity, transform);
                hex.name = $"Hex_{x}_{y}";

                Vector2Int hexCoordinates = new(x, y);
                GridCell hexCell = hex.transform.GetComponent<GridCell>();
                hexCell.SetCoordinates(hexCoordinates);
                _hexDict.Add(hexCoordinates, hexCell);
                allCells.Add(hexCell);
            }
        }

        MarkInitialFoodCells();
    }

    private void MarkInitialFoodCells()
    {
        int cellCount = Mathf.Min(desiredInitialFoodCount, allCells.Count);
        List<GridCell> selectedCells = new();

        while (selectedCells.Count < cellCount)
        {
            GridCell randomCell = allCells[Random.Range(0, allCells.Count)];
            if (selectedCells.Contains(randomCell)) continue;

            randomCell.SpawnFood();
            selectedCells.Add(randomCell);
        }
    }

    private void AssignNeighbors()
    {
        foreach (KeyValuePair<Vector2Int, GridCell> hexPair in _hexDict)
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

    public void CheckIfFoodMatches(FoodController lastPlacedFood)
    {
        GridCell lastPlacedCell = lastPlacedFood.GetCell();
        GridCell neighborMathcedCell = null;
        List<GridCell> neighbours = lastPlacedCell.neighbours;
        bool hasMatch = false;


        List<FoodController> matchedFoods = new();

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

        if (matchedFoods.Count >= 1) // one neighbour has same food
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
                    if (cell == lastPlacedCell) continue;
                    if (!cell.currentFood) continue;
                    if (cell.currentFood.GetFoodData().level >= 2) continue;
                    if (lastPlacedFood.GetFoodData().foodType != cell.currentFood.GetFoodData().foodType) continue;
                    if (lastPlacedFood.GetFoodData().level != cell.currentFood.GetFoodData().level) continue;

                    FoodController food = cell.currentFood;
                    if (matchedFoods.Contains(food)) continue;
                    if (food == lastPlacedFood) continue;

                    matchedFoods.Add(food);
                }
            }
        }

        if (matchedFoods.Count < 2) return;
        List<FoodController> incrementedFoods = new();
        lastPlacedFood.IncrementSelf();
        incrementedFoods.Add(lastPlacedFood);
        if (matchedFoods.Count <= 2)
        {
            for (int i = 0; i < 2; i++)
            {
                matchedFoods[i].Disappear(lastPlacedCell.GetCenter());
            }
        }
        else
        {
            if (matchedFoods.Count <= 5)
            {
                for (int i = 0; i < matchedFoods.Count; i++)
                {
                    if (i > 0)
                        matchedFoods[i].Disappear(lastPlacedCell.GetCenter());
                    else
                    {
                        matchedFoods[i].IncrementSelf();
                        incrementedFoods.Add(matchedFoods[i]);
                    }
                }
            }
        }

        foreach (FoodController food in incrementedFoods)
        {
            if (food != null && !food.isDisappearing && food?.GetFoodData().level < 2)
                CheckIfFoodMatches(food);
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