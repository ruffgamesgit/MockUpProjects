using UnityEngine;

public class InputManager : MonoBehaviour
{
    [SerializeField] private FoodController pickedObject;
    private Camera _mainCamera;
    private Vector3 _offset;

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
            Debug.Log(1); 
            Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Debug.Log(2);
                if (hit.transform.TryGetComponent(out FoodController foodController))
                {
                    Debug.Log(3);
                    pickedObject = foodController;
                    pickedObject.GetPicked();
                    _offset = pickedObject.transform.position - GetMouseWorldPosition();
                }
            }
        }

        if (Input.GetMouseButton(0) && pickedObject)
        {
            // Mouse button held down, drag the object
            pickedObject.transform.position = GetMouseWorldPosition() + _offset;
        }

        if (Input.GetMouseButtonUp(0) && pickedObject)
        {
            // Mouse button released, try to place the object
            Ray ray = new Ray(pickedObject.transform.position, Vector3.down);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.TryGetComponent(out GridCell cell))
                {
                    if (!cell.isOccupied)
                        pickedObject.GetPlaced(cell);
                    else
                        pickedObject.GetReleased();
                }
                else
                {
                    pickedObject.GetReleased();
                }
            }


            pickedObject = null;
        }
    }

    Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = _mainCamera.nearClipPlane;
        return _mainCamera.ScreenToWorldPoint(mousePoint);
    }
}