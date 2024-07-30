using UnityEngine;
using UnityEngine.Serialization;

public class InputManager : MonoBehaviour
{
    [Header("Config")] [SerializeField] float zOffset;
    private Vector3 _offset;
    private FoodController _pickedObject;
    private Camera _mainCamera;
    private GridCell _cellBelow;

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
            Transform pickedTr = _pickedObject.transform;
            pickedTr.position = GetMouseWorldPosition() + _offset;

            // Mouse button released, try to place the object
            Ray ray = new Ray(pickedTr.position, Vector3.down);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.transform.TryGetComponent(out GridCell cell))
                {
                    if (_cellBelow != null)
                    {
                        if (!cell.isOccupied)
                        {
                            if (_cellBelow != cell)
                            {
                                _cellBelow.indicatorController.DisableIndicator();
                                _cellBelow = cell;
                                cell.indicatorController.EnableIndicator();
                            }
                        }
                    }
                    else
                    {
                        if (!cell.isOccupied)
                        {
                            _cellBelow = cell;
                            _cellBelow.indicatorController.EnableIndicator();
                        }
                    }
                }
                else
                {
                    if (_cellBelow)
                    {
                        _cellBelow.indicatorController.DisableIndicator();
                        _cellBelow = null;
                    }
                }
            }
        }

        if (Input.GetMouseButtonUp(0) && _pickedObject)
        {
            // Mouse button released, try to place the object
            Ray ray = new(_pickedObject.transform.position, Vector3.down);

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
            else
                _pickedObject.GetReleased();


            if (_cellBelow != null)
            {
                _cellBelow.indicatorController.DisableIndicator();
                _cellBelow = null;
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