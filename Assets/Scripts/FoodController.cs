using System;
using DG.Tweening;
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
    private FoodDataHolderSoHolder dataSo;

    [Header("Debug")] [SerializeField] private GridCell currentCell;
    [SerializeField] private FoodData foodData;
    private Vector3 _initPos;
    private Image _foodImage;
    private MeshFilter _foodMeshFilter;
    [HideInInspector] public bool isDisappearing;

    void Start()
    {
        if (transform.parent && transform.parent.TryGetComponent(out GridCell cell))
        {
            currentCell = cell;
            currentCell.SetOccupied(this);
        }

        foodData.foodType = DataExtensions.GetRandomFoodType();
        foodData.level = 0;

        _foodImage = transform.GetComponentInChildren<Image>();
        _foodImage.sprite = GetSpriteFromSo(foodData);
        _foodMeshFilter = transform.GetComponentInChildren<MeshFilter>();
        _foodMeshFilter.mesh = GetSoData(foodData).mesh;
        if (foodData.foodType == FoodType.Patato)
            transform.GetComponentInChildren<MeshRenderer>().material = GetSoData(foodData).material;
        
        HexGridManager.instance?.AddFood(this);
    }

    public void IncrementSelf()
    {
        foodData.level++;
        _foodImage.sprite = GetSpriteFromSo(foodData);

        transform.DOScale(Vector3.one * 1.5f, .15f).SetLoops(2, LoopType.Yoyo)
            .OnComplete(() => CustomerManager.instance.CheckIfDataMatches(foodData, this));
    }

    public void Disappear(Vector3 pos)
    {
        isDisappearing = true;
        currentCell.SetFree();
        HexGridManager.instance.RemoveFood(this);

        Sequence sq = DOTween.Sequence();
        sq.Append(transform.DOMove(pos, .2f));
        sq.Append(transform.DOScale(Vector3.zero, .45f));
        sq.OnComplete(() => Destroy(gameObject));
    }

    private Sprite GetSpriteFromSo(FoodData data)
    {
        foreach (var soData in dataSo.generalFoodDatas)
        {
            if (data.foodType == soData.foodData.foodType &&
                data.level == soData.foodData.level)
            {
                return soData.sprite;
            }
        }

        return null;
    }

    private SoData GetSoData(FoodData data)
    {
        foreach (SoData soData in dataSo.generalFoodDatas)
        {
            if (data.foodType == soData.foodData.foodType &&
                data.level == soData.foodData.level)
            {
                return soData;
            }
        }

        return null;
    }

    public FoodData GetFoodData()
    {
        return foodData;
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

        HexGridManager.instance.CheckIfFoodMatches(this);
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