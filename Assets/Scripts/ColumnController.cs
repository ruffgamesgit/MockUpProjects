using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class ColumnController : MonoBehaviour
{
    [Header("Config")] [SerializeField] private int desiredPizzaCount;
    public int PizzaTypeVarietyCount;
    [Header("References")] [SerializeField]
    PizzaController pizzaPrefab;

    [Header("Debug")] [SerializeField] private GameObject highlighterObj;
    [SerializeField] private List<PlacementPoint> placementPoints;
    [HideInInspector] public int iterateCount;

    private void Awake()
    {
        for (int i = 0; i < transform.childCount - 1; i++)
        {
            placementPoints.Add(transform.GetChild(i).GetComponent<PlacementPoint>());
        }

        InitialColumnFill();
        UpdatePizzasPickableStatus();
    }

    public void SetHighlightStatus(bool activate)
    {
        highlighterObj.SetActive(activate);
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
            PizzaData randomData = DataExtensions.GetRandomPizzaData(2, PizzaTypeVarietyCount);

            if (instantiatedData.Count >= 1)
            {
                PizzaData lastData = instantiatedData[^1];
                while (randomData.pizzaType == lastData.pizzaType)
                {
                    randomData = DataExtensions.GetRandomPizzaData(2, PizzaTypeVarietyCount);
                }
            }


            clonePizza.Initialize(point, randomData);
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
        PizzaData randomData = DataExtensions.GetRandomPizzaData(2, PizzaTypeVarietyCount);


        for (int i = 0; i < placementPoints.Count; i++)
        {
            if (placementPoints[i].GetPizza() != null)
            {
                placementPoints[i].GetPizza().MoveToNextPoint(this);
            }
        }

        while (randomData.pizzaType == firstData.pizzaType)
        {
            randomData = DataExtensions.GetRandomPizzaData(2, PizzaTypeVarietyCount);
        }

        PizzaController clonePizza =
            Instantiate(pizzaPrefab, firstPoint.GetPos(), pizzaPrefab.transform.rotation);

        firstPoint.SetOccupied(clonePizza);
        clonePizza.Initialize(firstPoint, randomData);
        clonePizza.name = "Pizza_" + randomData.pizzaType + "-Level: " + randomData.level;
        clonePizza.transform.DOScale(Vector3.one, .5f).From(Vector3.zero).SetEase(Ease.InBounce);

        OnEveryMovementEnd();
    }

    // ReSharper disable Unity.PerformanceAnalysis
    public void CheckInnerSort(PizzaController lastPlacedPizza = null, bool blockAddingPizza = false)
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
                if (!lastElementPizza.isLeaderPizza) return;

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
                    yield return null;
                }

                if (!blockAddingPizza)
                    AddOnePizza();
            }

            StartCoroutine(Routine());
        }
    }

    // ReSharper disable Unity.PerformanceAnalysis
    public async void UpdatePizzasPickableStatus()
    {
        await UniTask.Delay(250);
        if (GetPoints().Count == 0) return;
        for (int i = 0; i < placementPoints.Count; i++)
        {
            if (placementPoints[i].GetPizza() != null)
                placementPoints[i].GetPizza().SetPickableStatus(assignAsPickable: false);
        }

        PlacementPoint firstPoint = GetPoints()[0];
        PlacementPoint lastOccupiedPoint = GetLastOccupiedPoint();

        if (firstPoint.GetPizza() != null) firstPoint.GetPizza().SetPickableStatus(assignAsPickable: true);
        if (lastOccupiedPoint != null && lastOccupiedPoint.GetPizza() != null)
            lastOccupiedPoint.GetPizza().SetPickableStatus(assignAsPickable: true);
    }

    // ReSharper disable Unity.PerformanceAnalysis
    public void OnEveryMovementEnd()
    {
        iterateCount = 0;
        UpdatePizzasPickableStatus();
    }
}