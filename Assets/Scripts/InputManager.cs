using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class InputManager : MonoSingleton<InputManager>
{
    private const float OffsetBetweenPizzas = .5f;
    [Header("References")] public LayerMask PickibleLayer;

    [Header("Config")] [SerializeField] float yDefaultPos;
    [SerializeField] float zAxisOffset;

    [Header("Debug")] [SerializeField] GameObject selectedObject;

    [SerializeField] PizzaController selectedPizza;

    [SerializeField] bool blockPicking;
    [SerializeField] private bool isDragging;
    private Camera _mainCamera;
    private List<PizzaController> _allPizzasOnColumn = new();

    void Start()
    {
        _mainCamera = Camera.main;
    }

    void Update()
    {
        if (!GameManager.instance.isLevelActive) return;
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, 300, PickibleLayer))
            {
                if (hit.collider.TryGetComponent(out PizzaController pizza))
                {
                    if (blockPicking) return;
                    if (pizza.isPicked) return;
                    if (!pizza.canPickable) return;

                    selectedObject = pizza.gameObject;
                    selectedPizza = pizza;


                    if (selectedPizza.CanBeLeaderPizza())
                    {
                        _allPizzasOnColumn.AddRange(
                            DataExtensions.GetAllPizzasOnColumn(selectedPizza.GetPoint().GetColumn()));
                        selectedPizza.SetLeader();
                    }

                    pizza.GetPicked();
                    _highlightedColumn = pizza.GetPoint().GetColumn();

                    blockPicking = true;
                    isDragging = true;
                }
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            selectedObject = null;
            isDragging = false;
            blockPicking = false;
        }

        if (isDragging && selectedObject is not null)
        {
            DragSelectedObject();
        }
    }

    private ColumnController _highlightedColumn;

    private void DragSelectedObject()
    {
        Vector3 mousePos = Input.mousePosition;
        Vector3 objectScreenPos = _mainCamera.WorldToScreenPoint(selectedObject.transform.position);
        mousePos.z = objectScreenPos.z;

        Vector3 worldPos = _mainCamera.ScreenToWorldPoint(mousePos);
        worldPos.y = yDefaultPos;

        selectedObject.transform.position = worldPos;

        ColumnController newColumn = selectedPizza.GetColumnBelow();
        if (_highlightedColumn != newColumn)
        {
            if (newColumn)
            {
                _highlightedColumn.SetHighlightStatus(false);
                _highlightedColumn = newColumn;
                _highlightedColumn.SetHighlightStatus(true);
            }
            else
            {
                _highlightedColumn.SetHighlightStatus(false);
            }
        }


        if (_allPizzasOnColumn.Count <= 0) return;

        MoveFollowerPizzas(worldPos);
    }

    private void MoveFollowerPizzas(Vector3 worldPos)
    {
        for (var i = 1; i < _allPizzasOnColumn.Count; i++)
        {
            PizzaController followerPizza = _allPizzasOnColumn[i];
            if (followerPizza is null) continue;
            var zOffset = i * OffsetBetweenPizzas;

            var pos = new Vector3(worldPos.x, worldPos.y, worldPos.z - zOffset);
            followerPizza.FollowTheLeaderPizza(pos, i);
        }
    }

    // ReSharper disable Unity.PerformanceAnalysis
    public void TriggerFollowePizzasPlacement(int leaderPointIndex, ColumnController newParentColumn)
    {
        if (_allPizzasOnColumn.Count == 0) return;

        int offsetCounter = 0;
        Vector3 lastFailPos = Vector3.zero;
        PizzaController leaderPizza = null;
        if (leaderPointIndex >= 0 && newParentColumn.GetPoints()[leaderPointIndex].GetPizza() is not null)
            leaderPizza = newParentColumn.GetPoints()[leaderPointIndex].GetPizza();

        for (int i = 1; i < _allPizzasOnColumn.Count; i++)
        {
            PizzaController followerPizza = _allPizzasOnColumn[i];
            if (followerPizza is null) continue;

            if (leaderPointIndex + i < newParentColumn.GetPoints().Count)
            {
                int newPointIndex = leaderPointIndex + i;
                PlacementPoint newPointToPlace = newParentColumn.GetPoints()[newPointIndex];
                followerPizza.GetPlaced(newPointToPlace, true, i == _allPizzasOnColumn.Count - 1, leaderPizza);
            }
            else
            {
                offsetCounter++;
                Vector3 lastPointPos = newParentColumn.GetPoints()[^1].GetPos();
                lastFailPos = new Vector3(lastPointPos.x, lastPointPos.y,
                    lastPointPos.z - (OffsetBetweenPizzas * offsetCounter));

                followerPizza.OnFail(lastFailPos);
                Debug.LogWarning("No more available placement point, FAILED");
                GameManager.instance.EndGame(false);
            }
        }

        _allPizzasOnColumn[^1].GetPoint().GetColumn().OnEveryMovementEnd();
        _allPizzasOnColumn.Clear();

       
    }

    public void OnPlaceFollowersTheSameColumn()
    {
        for (var i = 1; i < _allPizzasOnColumn.Count; i++)
        {
            PizzaController followerPizza = _allPizzasOnColumn[i];
            if (followerPizza is null) continue;
            if (!followerPizza.isLeaderPizza)
                followerPizza.GetReleased(followerPizza.GetPoint());
        }

        _allPizzasOnColumn.Clear();
    }
}