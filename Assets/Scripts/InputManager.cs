using UnityEngine;
using UnityEngine.Serialization;

public class InputManager : MonoBehaviour
{
    [Header("References")] public LayerMask PickibleLayer;

    [Header("Config")] [SerializeField] float yDefaultPos;
    [SerializeField] float zAxisOffset;

    [Header("Debug")] [SerializeField] GameObject selectedObject;

    [FormerlySerializedAs("selectedPickablePoint")] [SerializeField]
    Pickable selectedPickable;

    [SerializeField] bool blockPicking;
    [SerializeField] bool isDragging = false;
    private Camera mainCamera;

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

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
                selectedPickable.GetReleased();
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
        Vector3 objectScreenPos = mainCamera.WorldToScreenPoint(selectedObject.transform.position);
        mousePos.z = objectScreenPos.z;

        Vector3 worldPos = mainCamera.ScreenToWorldPoint(mousePos);
        worldPos.y = yDefaultPos;

        // Apply the initial offset to the new world position
        selectedObject.transform.position = worldPos;
    }
}