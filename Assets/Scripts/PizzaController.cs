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
    [Header("Config")] [SerializeField] LayerMask columnLayer;

    [Header("Pickable Debug")] public bool canPickable;
    public bool isLeaderPizza;
    [HideInInspector] public bool isPicked;

    [Header("Debug")] [SerializeField] private PlacementPoint currentPoint;
    private PizzaData pizzaData;
    private Transform selectedMesh;
    private bool _blockMovingNext;
    private Coroutine _routine;
    private Lot _lot;
    private const int MaxPizzaLevel = 3;
    private Tween _placementTween;

    public void Initialize(PlacementPoint point, PizzaData data)
    {
        SetPizzaData(data);
        SetMesh(data);
        currentPoint = point;
    }

    private void Start()
    {
        SetPickableStatus();
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

        PlacementPoint point = column.GetAvailablePoint();

        if (column != null)
        {
            if (currentPoint.GetColumn() != column && point is not null)
                GetPlaced(point);
            else
                GetReleased(currentPoint);
        }
    }

    #region GETTERS

    public PizzaData GetPizzaData()
    {
        return pizzaData;
    }

    public PlacementPoint GetPoint()
    {
        return currentPoint;
    }

    public void GetPicked()
    {
        isPicked = true;
    }

    public void GetReleased(PlacementPoint point)
    {
        isPicked = false;
        if (isLeaderPizza)
        {
            InputManager.instance.OnPlaceFollowersTheSameColumn();
        }

        GoPointToPlace(point.GetPos());
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
            SetPrevPickableStatus();

        currentPoint = newPoint;
        currentPoint.SetOccupied(this);

        void Callback()
        {
            newPoint.GetColumn().iterateCount = 0;
            if (!isFollowerPizza)
                newPoint.GetColumn().CheckInnerSort(lastPlacedPizza: this, isLeaderPizza);
        }

        GoPointToPlace(newPoint.GetPos(), Callback);
    }

    #endregion

    #region SETTERS

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

    public void SetPrevPickableStatus()
    {
        int prevIndex = currentPoint.GetIndex() - 1;
        if (prevIndex < 0) return;
        PizzaController prevPizza = currentPoint.GetColumn().GetPoints()[prevIndex].GetPizza();
        if (prevPizza is not null)
            prevPizza.canPickable = true;
    }

    public void SetLeader()
    {
        isLeaderPizza = true;
    }

    public void SetPoint(PlacementPoint newPoint, bool checkForThePickableStatus = false, bool setFreePrevPoint = true)
    {
        if (setFreePrevPoint)
            currentPoint.SetFree();

        currentPoint = newPoint;
        if (checkForThePickableStatus)
            SetPickableStatus();
    }

    #endregion

    #region MOVEMENT & DISAPPEARING

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

            GoPointToPlace(nextPoint.GetPos(), Callback);
        }
    }

    // ReSharper disable Unity.PerformanceAnalysis
    public void GoPointToPlace(Vector3 pointPos, TweenCallback callback = null)
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

        if (pizzaData.level == MaxPizzaLevel)
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
        SetPrevPickableStatus();

        ResetParams();
        _blockMovingNext = true;
        canPickable = false;
        transform.rotation = Quaternion.identity;
        _lot = LotHolder.instance.GetAvailableLot();
        _lot.SetOccupied(this);
        GoPointToPlace(_lot.GetPos(), () => LotHolder.instance.CheckForPossibleMatches());
        _routine = null;
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

    public void ResetParams()
    {
        if (GetPoint().GetPizza() == this)
            SetPoint(null, true);
    }

    #endregion

    #region HELPER METHODS

    public bool CanBeLeaderPizza()
    {
        return currentPoint.GetIndex() == 0 &&
               DataExtensions.GetOccupiedPointsCount(currentPoint.GetColumn().GetPoints()) > 1;
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

    // ReSharper disable once Unity.NoNullPropagation
    public void OnFail(Vector3 fallingPos)
    {
        SetPoint(null);
        currentPoint?.SetFree();
        transform.DOMove(fallingPos, .25f).OnComplete(() =>
            transform.DOScale(Vector3.zero, .5f).OnComplete(() => { Destroy(gameObject); }));
    }

    #endregion
}

[System.Serializable]
public class PizzaData
{
    public PizzaType pizzaType;
    public int level;
}