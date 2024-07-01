using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PointManager : MonoSingleton<PointManager>
{
    [SerializeField] private List<PlacementPoint> placementPoints = new();
    private const int MinMatchCount = 3;

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
        StartCoroutine(CheckForPossibleMatches());
    }

    IEnumerator CheckForPossibleMatches()
    {
        yield return new WaitForSeconds(0.125f);
        List<NumberObject> numberObjects = GetAllNumberObjects(excludeMovingObjects: true);
        if (numberObjects.Count < MinMatchCount) yield break;
        Dictionary<int, List<NumberObject>> numObjectsDict = SeparateObjectsByLevelValue(numberObjects);
        List<NumberObject> matchableNumberObjects = new();

        foreach (var kvp in numObjectsDict)
        {
            if (kvp.Value.Count >= MinMatchCount)
            {
                matchableNumberObjects = kvp.Value;
                break;
            }
        }

        int iterate = MinMatchCount;

        if (matchableNumberObjects.Count >= 3)
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

                PerformInnerSort();
                iterate--;
            }
        }
        else
        {
            StartCoroutine(FailCheck());
            PerformInnerSort();
        }
    }

    IEnumerator FailCheck()
    {
        yield return new WaitForSeconds(1.5f);
        if(!GameManager.instance.isLevelActive) yield break;
        if (GetOccupiedPointCount() == placementPoints.Count)
            GameManager.instance.EndGame(false);
    }

    private void PerformInnerSort()
    {
        List<NumberObject> numberObjects = new(GetAllNumberObjects());

        numberObjects = numberObjects.OrderByDescending(n => n.levelValue).ToList();

        foreach (PlacementPoint point in placementPoints)
        {
            point.SetFree();
        }

        for (int i = 0; i < numberObjects.Count; i++)
        {
            NumberObject obj = numberObjects[i];

            obj.InnerSortMovement(placementPoints[i]);
        }
    }

    Dictionary<int, List<NumberObject>> SeparateObjectsByLevelValue(List<NumberObject> allNumberObjects)
    {
        Dictionary<int, List<NumberObject>> separatedControllers = new();

        for (int i = 1; i < 10; i++)
        {
            separatedControllers[i] = new List<NumberObject>();
        }

        foreach (NumberObject numObjects in allNumberObjects)
        {
            if (separatedControllers.ContainsKey(numObjects.levelValue))
            {
                separatedControllers[numObjects.levelValue].Add(numObjects);
            }
        }

        return separatedControllers;
    }

    List<NumberObject> GetAllNumberObjects(bool excludeMovingObjects = false)
    {
        List<NumberObject> numberObjects = new();

        for (int i = 0; i < placementPoints.Count; i++)
        {
            if (!placementPoints[i].GetNumberObject()) continue;

            if (excludeMovingObjects)
            {
                if (!placementPoints[i].GetNumberObject().isMovingToPoint)
                    numberObjects.Add(placementPoints[i].GetNumberObject());
            }
            else
                numberObjects.Add(placementPoints[i].GetNumberObject());
        }

        return numberObjects;
    }
}