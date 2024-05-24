using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class ColumnController : MonoBehaviour
{
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
                Instantiate(pizzaPrefab, point.GetPos(), pizzaPrefab.transform.rotation);

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
            {
                return point;
            }
        }

        return null;
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private void AddOnePizza()
    {
        PlacementPoint firstPoint = placementPoints[0];
        PizzaData firstData = placementPoints[0].GetPizza().GetPizzaData();
        PizzaData randomData = DataExtensions.GetRandomPizzaData(2);

        int counter = 0;
        for (int i = 0; i < placementPoints.Count; i++)
        {
            if (placementPoints[i].GetPizza() != null)
            {
                placementPoints[i].GetPizza().MoveToNextPoint(this);
                counter++;
            }
        }

        Debug.LogWarning("Total pizza count: " + counter);
        while (randomData.pizzaType == firstData.pizzaType)
        {
            randomData = DataExtensions.GetRandomPizzaData(2);
        }

        PizzaController clonePizza =
            Instantiate(pizzaPrefab, firstPoint.GetPos(), pizzaPrefab.transform.rotation);

        firstPoint.SetOccupied(clonePizza);
        clonePizza.Initialize(firstPoint, randomData);
        clonePizza.name = "Pizza_" + randomData.pizzaType + "-Level: " + randomData.level;
        clonePizza.transform.DOScale(Vector3.one, .5f).From(Vector3.zero).SetEase(Ease.InBounce);
    }

    [HideInInspector] public int iterateCount;

    // ReSharper disable Unity.PerformanceAnalysis
    public void CheckInnerSort(PizzaController lastPlacedPizza = null, bool isLeaderPizzaInvokesAgain = false)
    {
        bool matched = false;
        PlacementPoint lastOccupiedPoint =
            lastPlacedPizza == null ? GetLastOccupiedPoint() : lastPlacedPizza.GetPoint();
        int lastOccupiedIndex = lastOccupiedPoint.GetIndex();

        PizzaController previousElementPizza = null;
        if (lastOccupiedIndex - 1 >= 0 && placementPoints[lastOccupiedIndex - 1].GetPizza() is not null)
            previousElementPizza = placementPoints[lastOccupiedIndex - 1].GetPizza();

        PizzaController lastElementPizza = lastPlacedPizza == null ? lastOccupiedPoint.GetPizza() : lastPlacedPizza;

        if (previousElementPizza is not null &&
            DataExtensions.CheckIfDataMatches(lastElementPizza.GetPizzaData(), previousElementPizza.GetPizzaData())
           )
        {
            matched = true;
            previousElementPizza.IncrementLevelAndSetMesh();
            lastElementPizza.Disappear();
        }

        if (matched)
        {
            iterateCount++;
            CheckInnerSort();
        }
        else
        {
            if (iterateCount != 0) // matching happened along the method 
            {
                if(!lastElementPizza.isLeaderPizza) return;
                
                if (lastElementPizza.GetPizzaData().level == 3)
                {
                    int targetPointIndex = lastElementPizza.GetPoint().GetIndex() - 1 <= -1
                        ? -1
                        : lastElementPizza.GetPoint().GetIndex() - 1;
                    InputManager.instance.TriggerFollowePizzasPlacement(targetPointIndex,
                        lastElementPizza.GetPoint().GetColumn());
                }
                else
                {
                    InputManager.instance.TriggerFollowePizzasPlacement(lastElementPizza.GetPoint().GetIndex(),
                        lastElementPizza.GetPoint().GetColumn());
                }
            }
        }

        if (iterateCount == 0) // means no matching happened, adding directly one pizza
        {
            IEnumerator Routine()
            {
                if (lastElementPizza.isLeaderPizza)
                {
                    InputManager.instance.TriggerFollowePizzasPlacement(lastElementPizza.GetPoint().GetIndex(),
                        lastElementPizza.GetPoint().GetColumn());
                }
                yield return null;
                AddOnePizza();
            }

            StartCoroutine(Routine());
        }
    }
}