using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class GridCell : MonoBehaviour
{
    [Header("Config")] [SerializeField] Vector2Int coordinates;
    [SerializeField] private bool initiliazeWithFood;

    [Header("Debug")] public bool isOccupied;
    public FoodController currentFood;
    public List<GridCell> neighbours;

    private void Awake()
    {
        if (!initiliazeWithFood)
            Destroy(transform.GetComponentInChildren<FoodController>().gameObject);
    }

    private void Start()
    {
        name = coordinates.ToString();

        neighbours = GetNeighbors();
    }
    
    #region GETTERS & SETTERS

    public void SetOccupied(FoodController foodController)
    {
        isOccupied = true;
        currentFood = foodController;
    }
    public void SetFree()
    {
        isOccupied = false;
        currentFood = null;
    }
    public Vector3 GetCenter()
    {
        return transform.position + transform.up / 4;
    }

    public Vector2Int GetCoordinates()
    {
        return coordinates;
    }

    private List<GridCell> GetNeighbors()
    {
        List<GridCell> gridCells = GridManager.instance.gridPlan;
        List<GridCell> neighbors = new();

        int[] dx = { 1, 0, -1, 0 };
        int[] dz = { 0, 1, 0, -1 };

        for (int i = 0; i < dx.Length; i++)
        {
            Vector2Int neighborCoordinates = coordinates + new Vector2Int(dx[i], dz[i]);
            GridCell neighbor = gridCells.Find(cell => cell.coordinates == neighborCoordinates);

            if (neighbor != null)
            {
                neighbors.Add(neighbor);
            }
        }

        return neighbors;
    }

    #endregion
}