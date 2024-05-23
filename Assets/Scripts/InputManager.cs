using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;

public class InputManager : MonoSingleton<InputManager>
{
    public event System.Action OnPickablePlacedEvent;

    [Header("References")] public LayerMask PickibleLayer;

    [Header("Config")] [SerializeField] float yDefaultPos;
    [SerializeField] float zAxisOffset;

    [Header("Debug")] [SerializeField] GameObject selectedObject;

    [SerializeField] Pickable selectedPickable;

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
                if (hit.collider.TryGetComponent(out Pickable pickable))
                {
                    if (blockPicking) return;
                    if (pickable.isPicked) return;
                    if (!pickable.canPickable) return;

                    selectedObject = pickable.gameObject;
                    selectedPickable = pickable;

                    if (selectedPickable.CanBeLeaderPizza())
                        _allPizzasOnColumn.AddRange(
                            DataExtensions.GetAllPizzasOnColumn(selectedPickable.GetPoint().GetColumn()));

                    pickable.GetPicked();

                    blockPicking = true;
                    isDragging = true;
                }
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            if (selectedObject is not null)
            {
                selectedPickable.GetReleased(selectedPickable.GetPoint());
                selectedObject = null;
                isDragging = false;
                blockPicking = false;
            }

            _allPizzasOnColumn.Clear();
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

        for (var i = 1; i < _allPizzasOnColumn.Count; i++)
        {
            PizzaController followerPizza = _allPizzasOnColumn[i];
            if (followerPizza is null) continue;
            var zOffset = i * .75f;

            var pos = new Vector3(worldPos.x, worldPos.y, worldPos.z - zOffset);
            followerPizza.GetPickable().FollowTheLeaderPizza(pos, i);
        }
    }

    public void TriggerOnPickablePlacedEvent()
    {
        OnPickablePlacedEvent?.Invoke();
    }
}