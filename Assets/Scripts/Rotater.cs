using UnityEngine;

public class Rotater : MonoBehaviour
{
    public float rotationSpeed;
    private Vector3 _lastMousePosition;
    private bool _isDragging = false;

    void Update()
    {
        if (!GameManager.instance.isLevelActive) return;
        if (Input.GetMouseButtonDown(0))
        {
            _lastMousePosition = Input.mousePosition;
            _isDragging = true;
        }
        else if (Input.GetMouseButton(0) && _isDragging)
        {
            Vector3 currentMousePosition = Input.mousePosition;
            Vector3 deltaMousePosition = currentMousePosition - _lastMousePosition;
            
            if(deltaMousePosition.sqrMagnitude < 0.1f) return;
            
            if (Mathf.Abs(deltaMousePosition.x) > Mathf.Abs(deltaMousePosition.y))
            {
                float rotationAmount = deltaMousePosition.x * rotationSpeed * Time.deltaTime;
                RotatePlatform(rotationAmount);
            }
            _lastMousePosition = currentMousePosition;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            _isDragging = false;
        }
    }

    void RotatePlatform(float rotationAmount)
    {
        transform.Rotate(0, -rotationAmount, 0);
    }
}
