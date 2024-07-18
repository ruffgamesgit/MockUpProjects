using System;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
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
    [Header("References")] [SerializeField]
    private FoodDataHolder dataSO;

    [Header("Debug")] [SerializeField] private FoodData foodData;
    [SerializeField] private GridCell currentCell;
    private Vector3 _initPos;
    private Image _foodImage;

    void Start()
    {
        if (transform.parent && transform.parent.TryGetComponent(out GridCell cell))
        {
            currentCell = cell;
            currentCell.SetOccupied(this);
        }

        foodData.foodType = DataExtensions.GetRandomFoodType();
        foodData.level = GetRandomLevel();

        _foodImage = transform.GetComponentInChildren<Image>();
        _foodImage.sprite = GetSpriteFromSO(foodData);
        GridManager.instance.AddFood(this);
    }

    public void IncrementSelf()
    {
        foodData.level++;
        _foodImage.sprite = GetSpriteFromSO(foodData);

        CustomerManager.instance.CheckIfDataMatches(foodData, this);
    }

    public void Disappear()
    {
        currentCell.SetFree();
        GridManager.instance.RemoveFood(this);
        Destroy(gameObject);
    }

    public Sprite GetSpriteFromSO(FoodData data)
    {
        foreach (var soData in dataSO.generalFoodDatas)
        {
            if (data.foodType == soData.foodData.foodType &&
                data.level == soData.foodData.level)
            {
                return soData.sprite;
            }
        }

        return null;
    }

    public FoodData GetFoodData()
    {
        return foodData;
    }


    private int GetRandomLevel()
    {
        return !currentCell ? 0 : Random.Range(0, 2);
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