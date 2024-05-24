using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;

public enum PizzaType
{
    Type1,
    Type2,
    Type3,
    Type4,
}

public class PizzaController : MonoBehaviour
{
    #region PIZZA CONTROLLER

    private const int MAX_PIZZA_LEVEL = 3;

    [Header("Debug")] [SerializeField] private Transform selectedMesh;
    [SerializeField] private PlacementPoint currentPoint;
    [SerializeField] private PizzaData pizzaData;
    private bool _blockMovingNext;
    private Coroutine _routine;
    private Lot _lot;

    public void Initialize(PlacementPoint point, PizzaData data)
    {
        SetPizzaData(data);
        SetMesh(data);
        currentPoint = point;
    }

    public void MoveToNextPoint(ColumnController parentColumn)
    {
        if (_blockMovingNext)
        {
            Debug.LogWarning("Pizza: " + gameObject.name + ", Point: " + currentPoint);
            return;
        }

        int nextPointIndex = currentPoint.GetIndex() + 1;
        if (nextPointIndex == parentColumn.GetPoints().Count)
        {
            Debug.LogError("No more available point, column is FULL");
        }
        else
        {
            currentPoint.SetFree();
            PlacementPoint nextPoint = parentColumn.GetPoints()[nextPointIndex];

            void Callback()
            {
                nextPoint.SetOccupied(this);
                SetPoint(nextPoint, true, false);
            }

            GoToPoint(nextPoint.GetPos(), Callback);
        }
    }

    public void SetMesh(PizzaData data)
    {
        Transform meshParent = transform.GetChild(0);
        switch (data.pizzaType)
        {
            case PizzaType.Type1:
                selectedMesh = meshParent.GetChild(0);
                break;
            case PizzaType.Type2:
                selectedMesh = meshParent.GetChild(1);
                break;
            case PizzaType.Type3:
                selectedMesh = meshParent.GetChild(2);
                break;
            case PizzaType.Type4:
                selectedMesh = meshParent.GetChild(3);
                break;
            default:
                Debug.LogError("Unknown pizza type!");
                break;
        }

        for (int i = 0; i < meshParent.childCount; i++)
        {
            if (meshParent.GetChild(i) != selectedMesh)
                Destroy(meshParent.GetChild(i).gameObject);
        }

        selectedMesh.gameObject.SetActive(true);
        selectedMesh.GetChild(data.level).gameObject.SetActive(true);
    }

    void SetPizzaData(PizzaData data)
    {
        pizzaData = data;
    }

    public PizzaData GetPizzaData()
    {
        return pizzaData;
    }

    private Tween _placementTween;

    // ReSharper disable Unity.PerformanceAnalysis
    public void GoToPoint(Vector3 pointPos, TweenCallback callback = null)
    {
        _placementTween = transform.DOMove(pointPos, .25f).OnComplete(() =>
        {
            _placementTween = null;
            //isLeaderPizza = false;
            callback?.Invoke();
        });
        _placementTween.Play();
    }

    public void IncrementLevelAndSetMesh()
    {
        pizzaData.level++;
        for (int i = 0; i < selectedMesh.childCount; i++)
        {
            selectedMesh.GetChild(i).gameObject.SetActive(false);
        }

        selectedMesh.GetChild(pizzaData.level).gameObject.SetActive(true);

        if (pizzaData.level == MAX_PIZZA_LEVEL)
        {
            if (LotHolder.instance.HasAvailableLot())
                GoForUpperLots();
            else
            {
                Debug.LogWarning("No more available placement point, FAILED");
                transform.DOScale(Vector3.zero, .5f).OnComplete(() => { gameObject.SetActive(false); });
            }
        }
    }

    private void GoForUpperLots()
    {
        SetPrevPickableStatus(currentPoint);

        ResetParams();
        _blockMovingNext = true;
        canPickable = false;
        transform.rotation = Quaternion.identity;
        _lot = LotHolder.instance.GetAvailableLot();
        _lot.SetOccupied(this);
        GoToPoint(_lot.GetPos(), () => LotHolder.instance.CheckForPossibleMatches());
        _routine = null;
    }

    public void ResetParams()
    {
        if (GetPoint().GetPizza() == this)
            SetPoint(null, true);
    }

    public void Disappear()
    {
        if (isLeaderPizza)
            InputManager.instance.TriggerFollowePizzasPlacement(currentPoint.GetIndex() - 1, currentPoint.GetColumn());

        _blockMovingNext = true;

        ResetParams();

        _placementTween?.Kill();
        transform.DOScale(Vector3.zero, .5f).OnComplete(() => { Destroy(gameObject); });
    }

    public void DisapearFromLot()
    {
        transform.DOScale(Vector3.zero, .5f).OnComplete(() =>
        {
            _lot.SetFree();
            Destroy(gameObject);
        });
    }

    public void OnFail(Vector3 fallingPos)
    {
        SetPoint(null);
        currentPoint?.SetFree();
        transform.DOMove(fallingPos, .25f).OnComplete(() =>
            transform.DOScale(Vector3.zero, .5f).OnComplete(() => { Destroy(gameObject); }));
    }

    #endregion

    #region PICKABLE

    [Header("Config")] [SerializeField] LayerMask columnLayer;
    [Header("Pickable Debug")] public bool isPicked;
    public bool canPickable;
    public bool isLeaderPizza;

    private void Start()
    {
        SetPickableStatus();
    }

    private void SetPickableStatus()
    {
        IEnumerator CheckingRoutine()
        {
            yield return null;

            ColumnController column = currentPoint.GetColumn();
            PlacementPoint lastOccupiedPoint = column.GetLastOccupiedPoint();
            PlacementPoint firstPoint = column.GetPoints()[0];

            if (currentPoint == firstPoint || currentPoint == lastOccupiedPoint)
            {
                canPickable = true;
            }
            else
            {
                canPickable = false;
            }
        }

        if (currentPoint is not null)
            StartCoroutine(CheckingRoutine());
        else
            canPickable = false;
    }

    public void GetPicked()
    {
        isPicked = true;
    }

    public void GetReleased(PlacementPoint _point)
    {
        isPicked = false;
        if (isLeaderPizza)
        {
            InputManager.instance.OnPlaceFollowersTheSameColumn();
        }

        GoToPoint(_point.GetPos());
    }

// ReSharper disable Unity.PerformanceAnalysis
    public void GetPlaced(PlacementPoint newPoint, bool isFollowerPizza = false)
    {
        isPicked = false;
        if (currentPoint is not null)
        {
            currentPoint.SetFree();
        }
        // When placed on a new column

        if (!isLeaderPizza || isFollowerPizza)
            SetPrevPickableStatus(currentPoint);

        currentPoint = newPoint;
        currentPoint.SetOccupied(this);

        void Callback()
        {
            newPoint.GetColumn().iterateCount = 0;
            if (!isFollowerPizza)
                newPoint.GetColumn().CheckInnerSort(lastPlacedPizza: this, isLeaderPizza);
        }

        GoToPoint(newPoint.GetPos(), Callback);
    }

    public void SetPoint(PlacementPoint newPoint, bool checkForThePickableStatus = false, bool setFreePrevPoint = true)
    {
        if (setFreePrevPoint)
            currentPoint.SetFree();

        currentPoint = newPoint;
        if (checkForThePickableStatus)
            SetPickableStatus();
    }


    public void SetLeader()
    {
        isLeaderPizza = true;
    }

    public bool CanBeLeaderPizza()
    {
        return currentPoint.GetIndex() == 0 &&
               DataExtensions.GetOccupiedPointsCount(currentPoint.GetColumn().GetPoints()) > 1;
    }

    public void SetPrevPickableStatus(PlacementPoint newPoint = null)
    {
        int prevIndex = currentPoint.GetIndex() - 1;
        if (prevIndex < 0) return;
        PizzaController prevPizza = currentPoint.GetColumn().GetPoints()[prevIndex].GetPizza();
        if (prevPizza is not null)
            prevPizza.canPickable = true;
    }

    public PlacementPoint GetPoint()
    {
        return currentPoint;
    }

    void OnMouseUp()
    {
        if (!isPicked) return;
        isPicked = false;

        ColumnController column = GetColumnBelow();

        if (column == null)
        {
            GetReleased(currentPoint);
            return;
        }

        PlacementPoint _point = column.GetAvailablePoint();

        if (column != null)
        {
            if (currentPoint.GetColumn() != column && _point is not null)
                GetPlaced(_point);
            else
                GetReleased(currentPoint);
        }
    }

    ColumnController GetColumnBelow()
    {
        RaycastHit hit;
        Ray ray = new Ray(transform.position + (Vector3.up), Vector3.down);

        if (Physics.Raycast(ray, out hit, 100, columnLayer))
        {
            if (hit.collider.transform.TryGetComponent(out ColumnController column))
            {
                return column;
            }
        }

        return null;
    }

    public async void FollowTheLeaderPizza(Vector3 newPos, int index)
    {
        await UniTask.Delay(index * 50);

        transform.position = newPos;
    }

    #endregion
}

[System.Serializable]
public class PizzaData
{
    public PizzaType pizzaType;
    public int level;
}