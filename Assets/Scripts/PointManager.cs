using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;

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
        foreach (PlacementPoint t in placementPoints)
        {
            if (t.isOccupied)
                counter++;
        }

        return counter;
    }

    public void OnNewNumberArrived(float delay = 0.125f)
    {
        StartCoroutine(CheckForPossibleMatches(delay));
    }

    // ReSharper disable Unity.PerformanceAnalysis
    IEnumerator CheckForPossibleMatches(float delay = 0.125f)
    {
        yield return new WaitForSeconds(delay);
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
            PerformInnerSort();
        }
    }
    
    // public void FailCheck()
    // {
    //     if (!GameManager.instance.isLevelActive) return;
    //     Debug.LogWarning(1);
    //
    //     int occPoint = GetOccupiedPointCount();
    //     if (occPoint != placementPoints.Count) return;
    //
    //     List<NumberObject> numberObjects = AGetAllNumberObjectss();
    //     Debug.LogError("Number objects count: " + numberObjects.Count);
    //
    //     Dictionary<int, List<NumberObject>> numObjectsDict = SeparateObjectsByLevelValue(numberObjects);
    //     List<NumberObject> matchableNumberObjects = new();
    //
    //     foreach (var kvp in numObjectsDict)
    //     {
    //         if (kvp.Value.Count >= MinMatchCount)
    //         {
    //             matchableNumberObjects = kvp.Value;
    //             break;
    //         }
    //     }
    //
    //     Debug.LogWarning(2);
    //     if (matchableNumberObjects.Count >= 3) return;
    //     Debug.LogWarning(3);
    //
    //     GameManager.instance.EndGame(false);
    // }

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

        foreach (NumberObject numObject in allNumberObjects)
        {
            if (!separatedControllers.ContainsKey(numObject.levelValue)) continue;
            separatedControllers[numObject.levelValue].Add(numObject);
        }

        return separatedControllers;
    }

    private List<NumberObject> GetAllNumberObjects(bool excludeMovingObjects = false)
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