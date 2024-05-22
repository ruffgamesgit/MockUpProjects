using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;


public class ColumnController : MonoBehaviour
{
    public event System.Action<ColumnController> OnAddedNewPizzaEvent;

    [Header("Config")] [SerializeField] private int desiredPizzaCount;

    [Header("References")] [SerializeField]
    PizzaController pizzaPrefab;

    [Header("Debug")] [SerializeField] private List<PlacementPoint> placementPoints;

    private void Awake()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            placementPoints.Add(transform.GetChild(i).GetComponent<PlacementPoint>());
        }

        InitialColumnFill();
    }

    private void InitialColumnFill()
    {
        List<PizzaData> instantiatedData = new List<PizzaData>();
        desiredPizzaCount = Random.Range(1, desiredPizzaCount + 1);

        for (int i = 0; i < desiredPizzaCount; i++)
        {
            PlacementPoint point = placementPoints[i];
            PizzaController clonePizza =
                Instantiate(pizzaPrefab, point.GetPos(), Quaternion.identity);

            point.SetOccupied(clonePizza);
            PizzaData randomData = DataExtensions.GetRandomPizzaData(2);

            if (instantiatedData.Count >= 1)
            {
                PizzaData lastData = instantiatedData[^1];
                while (randomData.pizzaType == lastData.pizzaType)
                {
                    randomData = DataExtensions.GetRandomPizzaData(2);
                }
            }


            clonePizza.Initialize(point, randomData);
            clonePizza.name = "Pizza_" + randomData.pizzaType + "-Level: " + randomData.level;
            clonePizza.transform.DOScale(Vector3.one, .5f).From(Vector3.zero).SetEase(Ease.InBounce);
            instantiatedData.Add(clonePizza.GetPizzaData());
        }
    }

    public List<PlacementPoint> GetPoints()
    {
        return placementPoints;
    }

    public PlacementPoint GetLastOccupiedPoint()
    {
        for (int i = placementPoints.Count - 1; i >= 0; i--)
        {
            PlacementPoint point = placementPoints[i];
            if (point.CheckIfOccupied())
                return point;
        }

        return null;
    }

    private void AddOnePizza()
    {
        PlacementPoint firstPoint = placementPoints[0];
        PizzaData firstData = placementPoints[0].GetPizza().GetPizzaData();
        PizzaData randomData = DataExtensions.GetRandomPizzaData(2);
        
        for (int i = 0; i < placementPoints.Count; i++)
        {
            if (placementPoints[i].GetPizza() != null)
                placementPoints[i].GetPizza().MoveToNextPoint(this);
        }

        while (randomData.pizzaType == firstData.pizzaType)
        {
            randomData = DataExtensions.GetRandomPizzaData(2);
        }
        
        PizzaController clonePizza =
            Instantiate(pizzaPrefab, firstPoint.GetPos(), Quaternion.identity);
        
        firstPoint.SetOccupied(clonePizza);
        clonePizza.Initialize(firstPoint, randomData);
        clonePizza.name = "Pizza_" + randomData.pizzaType + "-Level: " + randomData.level;
        clonePizza.transform.DOScale(Vector3.one, .5f).From(Vector3.zero).SetEase(Ease.InBounce);
    }

    // ReSharper disable Unity.PerformanceAnalysis
    public void CheckInnerSort()
    {
        // sadece bir adet occupied point var ise return, sonradan arkadan pizza gelme özelliği de eklenirse bu durum değişebilir
        if (DataExtensions.GetOccupiedPointsCount(placementPoints) <= 1) return;

        bool matched = false;
        PlacementPoint lastOccupiedPoint = GetLastOccupiedPoint();
        int lastOccupiedIndex = placementPoints.IndexOf(lastOccupiedPoint);

        PizzaController previousElementPizza = placementPoints[lastOccupiedIndex - 1].GetPizza();
        PizzaController lastElementPizza = lastOccupiedPoint.GetPizza();

        if (DataExtensions.CheckIfDataMatches(lastElementPizza.GetPizzaData(), previousElementPizza.GetPizzaData()))
        {
            Debug.LogWarning("Data matched");
            matched = true;
            previousElementPizza.IncrementLevelAndSetMesh();
            lastElementPizza.Disappear();
        }

        if (matched) CheckInnerSort();
        else
        {
            AddOnePizza();
        }
    }
}