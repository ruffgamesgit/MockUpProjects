using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public enum PizzaType
{
    Type1,
    Type2,
    Type3,
    Type4,
}

public class PizzaController : MonoBehaviour
{
    private const int MAX_PIZZA_LEVEL = 3;

    [Header("Debug")] [SerializeField] private Transform selectedMesh;
    [SerializeField] private PizzaData pizzaData;
    private Pickable _pickable;
    private Coroutine _routine;
    private Lot _lot;
    private PlacementPoint _currentPoint;
    private bool blockMovingNext;


    public void Initialize(PlacementPoint point, PizzaData data)
    {
        SetPizzaData(data);
        SetMesh(data);
        _currentPoint = point;

        _pickable = GetComponent<Pickable>();

        _pickable.SetPoint(_currentPoint);
        _pickable.OnPicked += OnPicked;
        _pickable.OnReleased += OnReleased;
        _pickable.OnPlaced += OnPlaced;
    }

    public Pickable GetPickable()
    {
        return _pickable;
    }

    public void MoveToNextPoint(ColumnController parentColumn)
    {
        if (blockMovingNext)
        {
            Debug.LogWarning("Pizza: " + gameObject.name + ", Point: " + _currentPoint);
            return;
        }

        int nextPointIndex = parentColumn.GetPoints().IndexOf(_pickable.GetPoint()) + 1;
        if (nextPointIndex == parentColumn.GetPoints().Count)
        {
            Debug.LogError("No more available point, column is FULL");
        }
        else
        {
            _currentPoint.SetFree();
            PlacementPoint nextPoint = parentColumn.GetPoints()[nextPointIndex];

            void Callback()
            {
                nextPoint.SetOccupied(this);
                _pickable.SetPoint(nextPoint, true);
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

    public void GoToPoint(Vector3 pointPos, TweenCallback callback = null)
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
        pizzaData.level++;
        for (int i = 0; i < selectedMesh.childCount; i++)
        {
            selectedMesh.GetChild(i).gameObject.SetActive(false);
        }

        selectedMesh.GetChild(pizzaData.level).gameObject.SetActive(true);

        if (pizzaData.level == MAX_PIZZA_LEVEL && _routine == null)
        {
            _routine = StartCoroutine(Routine());

            IEnumerator Routine()
            {
                yield return new WaitUntil(() => LotHolder.instance.HasAvailableLot() == true);
                GoForUpperLots();
            }
        }
    }

    private void GoForUpperLots()
    {
        _pickable.SetPrevPickableStatus(_currentPoint);

        ResetParams();
        blockMovingNext = true;

        _lot = LotHolder.instance.GetAvailableLot();
        _lot.SetOccupied(this);
        GoToPoint(_lot.GetPos(), () => LotHolder.instance.CheckForPossibleMatches());
        _routine = null;
    }

    public void ResetParams()
    {
        _pickable.SetPoint(null, true);
        _currentPoint.SetFree();
        _currentPoint = null;
    }

    public void Disappear()
    {
        blockMovingNext = true;

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

    #region Event Subscribers

    void OnReleased(PlacementPoint point)
    {
        GoToPoint(point.GetPos());
    }

    private void OnPicked()
    {
        // Add some outline etc.
    }

    private void OnPlaced(PlacementPoint newPoint, TweenCallback callback = null)
    {
        _currentPoint.SetOccupied(this);
        GoToPoint(newPoint.GetPos(), callback);
    }

    #endregion
}

[System.Serializable]
public class PizzaData
{
    public PizzaType pizzaType;
    public int level;
}