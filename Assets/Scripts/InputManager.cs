using UnityEngine;
using UnityEngine.Serialization;

public class InputManager : MonoBehaviour
{
    [Header("Config")] [SerializeField] float zOffset;
    private Vector3 _offset;
    private FoodController _pickedObject;
    private Camera _mainCamera;

    void Start()
    {
        _mainCamera = Camera.main;
    }

    void Update()
    {
        HandleMouseInput();
    }

    // ReSharper disable Unity.PerformanceAnalysis
    void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.transform.TryGetComponent(out FoodController foodController))
                {
                    if (foodController.GetCell()) return;
                    _pickedObject = foodController;
                    _pickedObject.GetPicked();

                    _offset = _pickedObject.transform.position - GetMouseWorldPosition() + new Vector3(0, 0, zOffset);
                }
            }
        }

        if (Input.GetMouseButton(0) && _pickedObject)
        {
            // Mouse button held down, drag the object
            _pickedObject.transform.position = GetMouseWorldPosition() + _offset;
        }

        if (Input.GetMouseButtonUp(0) && _pickedObject)
        {
            // Mouse button released, try to place the object
            Ray ray = new Ray(_pickedObject.transform.position, Vector3.down);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.transform.TryGetComponent(out GridCell cell))
                {
                    if (!cell.isOccupied)
                        _pickedObject.GetPlaced(cell);
                    else
                        _pickedObject.GetReleased();
                }
                else
                {
                    _pickedObject.GetReleased();
                }
            }


            _pickedObject = null;
        }
    }

    Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = _mainCamera.nearClipPlane;
        return _mainCamera.ScreenToWorldPoint(mousePoint);
    }
}