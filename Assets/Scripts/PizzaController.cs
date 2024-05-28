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
    private ParticleHandler _particleHandler;

    public void Initialize(PlacementPoint point, PizzaData data)
    {
        SetPizzaData(data);
        SetMesh(data);
        currentPoint = point;
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
        currentPoint.GetColumn().SetHighlightStatus(true);
    }

    public void GetReleased(PlacementPoint point)
    {
        isPicked = false;
        if (isLeaderPizza)
        {
            InputManager.instance.OnPlaceFollowersTheSameColumn();
        }

        currentPoint.GetColumn().SetHighlightStatus(false);
        GoPointToPlace(point.GetPos());
    }

// ReSharper disable Unity.PerformanceAnalysis
    public void GetPlaced(PlacementPoint newPoint, bool isFollowerPizza = false, bool isLastFollower = false,
        PizzaController leaderPizza = null)
    {
        currentPoint.GetColumn().SetHighlightStatus(false);
        isPicked = false;
        if (currentPoint is not null)
        {
            currentPoint.SetFree();
        }

        // When placed on a new column
        if (!isLeaderPizza || isFollowerPizza)
        {
            SetPreviousPickableStatus();

            if (isLastFollower)
                if (leaderPizza != null)
                    leaderPizza.SetLeader(assignLeader: false);
        }

        currentPoint?.GetColumn()?.OnEveryMovementEnd();
        currentPoint = newPoint;
        currentPoint.SetOccupied(this);
        currentPoint.GetColumn().SetHighlightStatus(false);

        void Callback()
        {
            newPoint.GetColumn().iterateCount = 0;
            if (!isFollowerPizza)
                newPoint.GetColumn().CheckInnerSort(lastPlacedPizza: this);
            newPoint.GetColumn().OnEveryMovementEnd();
        }

        GoPointToPlace(newPoint.GetPos(), Callback);
    }

    #endregion

    #region SETTERS

    void SetName()
    {
        gameObject.name = "Pizza_" + pizzaData.pizzaType + "-Level: " + pizzaData.level;
    }

    public void SetLeader(bool assignLeader = true)
    {
        isLeaderPizza = assignLeader;
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
        _particleHandler = selectedMesh?.GetComponent<ParticleHandler>();
    }

    void SetPizzaData(PizzaData data)
    {
        pizzaData = data;
        SetName();
    }

    public void SetPickableStatus(bool assignAsPickable)
    {
        canPickable = assignAsPickable;
    }

    public void SetPreviousPickableStatus()
    {
        int prevIndex = currentPoint.GetIndex() - 1;
        if (prevIndex < 0) return;
        PizzaController prevPizza = currentPoint.GetColumn().GetPoints()[prevIndex].GetPizza();
        if (prevPizza is not null)
            prevPizza.canPickable = true;
    }

    public void SetPoint(PlacementPoint newPoint, bool checkForThePickableStatus = false, bool setFreePrevPoint = true)
    {
        if (setFreePrevPoint)
            currentPoint?.SetFree();

        currentPoint?.GetColumn().UpdatePizzasPickableStatus();
        currentPoint = newPoint;

        if (checkForThePickableStatus)
        {
            if (currentPoint != null) currentPoint.GetColumn().UpdatePizzasPickableStatus();
        }
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
            GameManager.instance.EndGame(false, 1f);
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
            callback?.Invoke();
        });
        _placementTween.Play();
    }

    public void IncrementLevelAndSetMesh()
    {
        if (!GameManager.instance.isLevelActive) return;
        _particleHandler.PlayExplosion();

        pizzaData.level++;
        for (int i = 0; i < selectedMesh.childCount; i++)
        {
            selectedMesh.GetChild(i).gameObject.SetActive(false);
        }

        selectedMesh.GetChild(pizzaData.level).gameObject.SetActive(true);
        SetName();

        if (pizzaData.level == MaxPizzaLevel)
        {
            if (LotHolder.instance.HasAvailableLot())
            {
                int targetIndex = currentPoint.GetIndex() - 1 < -1 ? -1 : currentPoint.GetIndex() - 1;
                InputManager.instance.TriggerFollowePizzasPlacement(targetIndex,
                    currentPoint.GetColumn());
                PizzaController previousPizza = currentPoint.GetColumn().GetPoints()[targetIndex + 1].GetPizza();
                currentPoint.GetColumn().CheckInnerSort(previousPizza, true);
                
                GoForUpperLots();
            }
            else
            {
                GameManager.instance.EndGame(false);
                Debug.LogWarning("No more available placement point, FAILED");
                transform.DOScale(Vector3.zero, .5f).OnComplete(() => { gameObject.SetActive(false); });
            }
        }
        else
        {
            InputManager.instance.TriggerFollowePizzasPlacement(currentPoint.GetIndex(), currentPoint.GetColumn());
            currentPoint.GetColumn().CheckInnerSort(this, true);
        }
    }

    private void GoForUpperLots()
    {
        SetPreviousPickableStatus();

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
        {
            int indexInterval = pizzaData.level == 2 ? 2 : 1;
            int targetPointIndex = currentPoint.GetIndex() - indexInterval;
            InputManager.instance.TriggerFollowePizzasPlacement(targetPointIndex, currentPoint.GetColumn());
            PizzaController nextPizza = currentPoint.GetColumn().GetPoints()[currentPoint.GetIndex() + 1].GetPizza();
            currentPoint.GetColumn().CheckInnerSort(nextPizza, true);
        }

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

    public ColumnController GetColumnBelow()
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

[Serializable]
public class PizzaData
{
    public PizzaType pizzaType;
    public int level;
}