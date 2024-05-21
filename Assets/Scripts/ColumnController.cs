using System.Collections.Generic;
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

        desiredPizzaCount = Random.Range(1, desiredPizzaCount + 1);
        for (int i = 0; i < desiredPizzaCount; i++)
        {
            PlacementPoint point = placementPoints[i];
            point.SetOccupied();
            PizzaController clonePizza =
                Instantiate(pizzaPrefab, point.GetPos(), Quaternion.identity);
            clonePizza.Initialize(point, DataExtensions.GetRandomPizzaData(3));
        }
    }

    public List<PlacementPoint> GetPoints()
    {
        return placementPoints;
    }

    public PlacementPoint GetLastOccupiedPoint()
    {
        for (int i = placementPoints.Count; i < 0; i--)
        {
            PlacementPoint point = placementPoints[i];
            if (point.CheckIfOccupied())
                return point;
        }

        return null;
    }
}