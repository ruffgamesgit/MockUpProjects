using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PointManager : MonoSingleton<PointManager>
{
    [SerializeField] private List<PlacementPoint> placementPoints = new();
    private int minMatchCount = 3;

    public PlacementPoint GetAvailablePoint()
    {
        foreach (PlacementPoint point in placementPoints)
        {
            if (!point.isOccupied)
                return point;
        }

        return null;
    }

    public int GetOccupiedPointCount()
    {
        int counter = 0;
        for (int i = 0; i < placementPoints.Count; i++)
        {
            if (placementPoints[i].isOccupied)
                counter++;
        }

        return counter;
    }

    public void OnNewNumberArrived()
    {
        CheckForPossibleMatches();
    }

    void CheckForPossibleMatches()
    {
        List<NumberObject> numberObjects = GetAllNumberObjects();
        if (numberObjects.Count < minMatchCount) return;

        Dictionary<int, List<NumberObject>> numObjectsDict = SeparateObjectsByLevelValue(numberObjects);
        List<NumberObject> matchableNumberObjects = new();

        foreach (var kvp in numObjectsDict)
        {
            if (kvp.Value.Count >= minMatchCount)
            {
                matchableNumberObjects = kvp.Value;
                break;
            }
        }

        int iterate = minMatchCount;

        if (matchableNumberObjects.Count != 0)
        {
            for (int i = 0; i < matchableNumberObjects.Count; i++)
            {
                if (iterate == 0) break;
                NumberObject numObj = matchableNumberObjects[i];
                if (i == 0)
                    numObj.UpgradeSelf();
                else
                {
                    numObj.Merge(matchableNumberObjects[0].transform.position);
                }

                // pizzas.Remove(matchablePizzas[i]);
                iterate--;
            }
        }
        else
        {
            // if (pizzas.Count == Lots.Count)
            // {
            //     GameManager.instance.EndGame(false);
            //     Debug.LogError("No more available lot, LOT is FULL");
            // }
        }
    }

    Dictionary<int, List<NumberObject>> SeparateObjectsByLevelValue(List<NumberObject> allNumberObjects)
    {
        Dictionary<int, List<NumberObject>> separatedControllers = new();

        // Initialize the dictionary with empty lists for each PizzaType
        for (int i = 1; i < 10; i++)
        {
            separatedControllers[i] = new List<NumberObject>();
        }

        // Populate the dictionary
        foreach (NumberObject numObjects in allNumberObjects)
        {
            if (separatedControllers.ContainsKey(numObjects.levelValue))
            {
                separatedControllers[numObjects.levelValue].Add(numObjects);
            }
        }

        return separatedControllers;
    }

    List<NumberObject> GetAllNumberObjects()
    {
        List<NumberObject> numberObjects = new();

        for (int i = 0; i < placementPoints.Count; i++)
        {
            if (placementPoints[i].GetNumberObject())
                numberObjects.Add(placementPoints[i].GetNumberObject());
        }

        return numberObjects;
    }

    public int GetPointCount()
    {
        return placementPoints.Count;
    }
}