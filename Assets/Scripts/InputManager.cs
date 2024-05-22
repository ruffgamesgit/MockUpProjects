using System;
using System.Collections;
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
                    if (pickable.IsPicked) return;
                    if (!pickable.CanPickable) return;

                    selectedObject = pickable.gameObject;
                    selectedPickable = pickable;
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

        // Apply the initial offset to the new world position
        selectedObject.transform.position = worldPos;
    }

    public void TriggerOnPickablePlacedEvent()
    {
        OnPickablePlacedEvent?.Invoke();
    }
}