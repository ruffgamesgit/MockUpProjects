using System.Collections.Generic;
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


    private void DragSelectedObject()
    {
        Vector3 mousePos = Input.mousePosition;
        Vector3 objectScreenPos = _mainCamera.WorldToScreenPoint(selectedObject.transform.position);
        mousePos.z = objectScreenPos.z;

        Vector3 worldPos = _mainCamera.ScreenToWorldPoint(mousePos);
        worldPos.y = yDefaultPos;

        selectedObject.transform.position = worldPos;

        if (_allPizzasOnColumn.Count <= 0) return;

        MoveOtherPizzas(worldPos);
    }

    private void MoveOtherPizzas(Vector3 worldPos)
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


    public void TriggerFollowePizzasPlacement(int leaderPointIndex, ColumnController newParentColumn)
    {
        int offsetCounter = 0;
        Vector3 lastFailPos = Vector3.zero;

        for (var i = 1; i < _allPizzasOnColumn.Count; i++)
        {
            PizzaController followerPizza = _allPizzasOnColumn[i];
            if (followerPizza is null) continue;

            if (leaderPointIndex + i < newParentColumn.GetPoints().Count)
            {
                int newPointIndex = leaderPointIndex + i;
                PlacementPoint newPointToPlace = newParentColumn.GetPoints()[newPointIndex];
                followerPizza.GetPlaced(newPointToPlace, true);
            }
            else
            {
                offsetCounter++;
                Vector3 lastPointPos = newParentColumn.GetPoints()[^1].GetPos();
                lastFailPos = new Vector3(lastPointPos.x, lastPointPos.y,
                    lastPointPos.z - (OffsetBetweenPizzas * offsetCounter));

                followerPizza.OnFail(lastFailPos);
                Debug.LogWarning("No more available placement point, FAILED");
            }
        }

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