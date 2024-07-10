using System;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public enum FoodType
{
    None,
    Chicken,
    Patato,
    Bread,
    Juice,
    Coffee
}

public class FoodController : MonoBehaviour
{
    [SerializeField] private FoodData foodData;
    [SerializeField] private GridCell currentCell;
    private Vector3 _initPos;

    void Start()
    {
        if (transform.parent && transform.parent.TryGetComponent(out GridCell cell))
        {
            currentCell = cell;
            currentCell.SetOccupied(this);
        }

        foodData.foodType = DataExtensions.GetRandomFoodType();
        foodData.level = GetRandomLevel();


        transform.GetComponentInChildren<TextMeshProUGUI>().text = foodData.foodType + " _ " + foodData.level;
    }

    public void IncrementSelf()
    {
        foodData.level++;
        transform.GetComponentInChildren<TextMeshProUGUI>().text = foodData.foodType + " _ " + foodData.level;
    }

    public void Disappear()
    {
        currentCell.SetFree();
        Destroy(gameObject);
    }

    public FoodData GetFoodData()
    {
        return foodData;
    }


    private int GetRandomLevel()
    {
        return !currentCell ? 0 : Random.Range(0, 3);
    }

    public void GetPicked()
    {
        _initPos = transform.position;
    }

    public void GetPlaced(GridCell cell)
    {
        ConveyorManager.instance.OnFoodPlaced(this);
        transform.GetComponent<Collider>().enabled = false;

        currentCell = cell;
        currentCell.SetOccupied(this);
        transform.SetParent(cell.transform);
        transform.position = cell.GetCenter();

        GridManager.instance.CheckIfFoodMatches(this);
    }

    public void GetReleased()
    {
        transform.position = _initPos;
    }

    public GridCell GetCell()
    {
        return currentCell;
    }
}

[Serializable]
public class FoodData
{
    public FoodType foodType;
    public int level;
}