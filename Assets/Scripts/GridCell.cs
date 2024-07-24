using System.Collections.Generic;
using UnityEngine;

public class GridCell : MonoBehaviour
{
    [Header("Config")] [SerializeField] Vector2Int coordinates;
    [SerializeField] private bool initiliazeWithFood;

    [Header("Debug")] public bool isOccupied;
    public FoodController currentFood;
    public List<GridCell> neighbours;

    private void Awake()
    {
        int a = Random.Range(0, 2);
        if (a > 0)
            Destroy(transform.GetComponentInChildren<FoodController>().gameObject);
    }

    private void Start()
    {
        name = coordinates.ToString();

        //    neighbours = GetNeighbors();
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