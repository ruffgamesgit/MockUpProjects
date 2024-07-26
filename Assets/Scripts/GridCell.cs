using System.Collections.Generic;
using UnityEngine;

public class GridCell : MonoBehaviour
{
    [Header("References")] [SerializeField]
    private FoodController foodPrefab;
    
    [Header("Debug")] public bool isOccupied;
    [SerializeField] Vector2Int coordinates;
    public FoodController currentFood;
    public List<GridCell> neighbours;

    public void SpawnFood()
    {
        Instantiate(foodPrefab, GetCenter(), Quaternion.identity, transform);
    }

    private void Start()
    {
        name = coordinates.ToString();
    }


    #region GETTERS & SETTERS

    public void SetCoordinates(Vector2Int _coordinates)
    {
        coordinates = _coordinates;
    }

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

    #endregion
}