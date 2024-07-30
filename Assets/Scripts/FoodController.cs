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

    [SerializeField] private Material defaultFoodMaterial;

    [Header("Debug")] [SerializeField] private GridCell currentCell;
    [SerializeField] private FoodData foodData;
    private Vector3 _initPos;
    private Image _foodImage;
    private MeshFilter _foodMeshFilter;
    private MeshRenderer _meshRenderer;
    [HideInInspector] public bool isDisappearing;
    private GameObject _currentModel;

    void Start()
    {
        if (transform.parent && transform.parent.TryGetComponent(out GridCell cell))
        {
            currentCell = cell;
            currentCell.SetOccupied(this);
        }

        foodData.foodType = DataExtensions.GetRandomFoodType();
        foodData.level = 0;
        
        AssignMeshAndMaterial();
        HexGridManager.instance?.AddFood(this);
    }

    public void IncrementSelf()
    {
        foodData.level++;
        AssignMeshAndMaterial();

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


    void AssignMeshAndMaterial()
    {
        if (_currentModel != null)
        {
            Destroy(_currentModel);
        }

        _currentModel = Instantiate(GetSoData(foodData).model, transform.position, Quaternion.identity, transform);
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
        cell.indicatorController.DisableIndicator();

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