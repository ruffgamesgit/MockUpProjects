using System.Collections;
using UnityEngine;

public class Rotater : MonoBehaviour
{
    public static Rotater instance;

    public float rotationSpeed;
    private Vector3 _lastMousePosition;
    public bool isRotating = false;
    public bool blockPlatformRotation;
    public bool rotaterIsPerfomed;

    private void Awake()
    {
        instance = this;
    }

    void Update()
    {
        if (!GameManager.instance.isLevelActive) return;
        if (blockPlatformRotation) return;

        if (Input.GetMouseButtonDown(0))
        {
            _lastMousePosition = Input.mousePosition;
            isRotating = true;
        }
        else if (Input.GetMouseButton(0) && isRotating)
        {
            Vector3 currentMousePosition = Input.mousePosition;
            Vector3 deltaMousePosition = currentMousePosition - _lastMousePosition;

            if (deltaMousePosition.magnitude < 20f) return;
          //  Debug.LogWarning("Delta:" + deltaMousePosition.magnitude);
            if (Mathf.Abs(deltaMousePosition.x) > Mathf.Abs(deltaMousePosition.y))
            {
                float rotationAmount = deltaMousePosition.x * rotationSpeed * Time.deltaTime;
                RotatePlatform(rotationAmount);
            }

            _lastMousePosition = currentMousePosition;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isRotating = false;

            IEnumerator Routine()
            {
                yield return new WaitForSeconds(0.1f);
                rotaterIsPerfomed = false;
            }

            StartCoroutine(Routine());
        }
    }
    // void OnMouseDown()
    // {
    //     if (!GameManager.instance.isLevelActive) return;
    //     if (blockPlatformRotation) return;
    //
    //     _lastMousePosition = Input.mousePosition;
    //     isRotating = true;
    // }
    //
    // void OnMouseDrag()
    // {
    //     if (!GameManager.instance.isLevelActive) return;
    //     if (blockPlatformRotation) return;
    //
    //     Vector3 currentMousePosition = Input.mousePosition;
    //     Vector3 deltaMousePosition = currentMousePosition - _lastMousePosition;
    //
    //     if (deltaMousePosition.sqrMagnitude < 0.1f) return;
    //
    //     if (Mathf.Abs(deltaMousePosition.x) > Mathf.Abs(deltaMousePosition.y))
    //     {
    //         float rotationAmount = deltaMousePosition.x * rotationSpeed * Time.deltaTime;
    //         RotatePlatform(rotationAmount);
    //     }
    //
    //     _lastMousePosition = currentMousePosition;
    // }
    //
    // void OnMouseUp()
    // {
    //     isRotating = false;
    //
    //    
    // }

    void RotatePlatform(float rotationAmount)
    {
        rotaterIsPerfomed = true;
        transform.Rotate(0, -rotationAmount, 0);
    }
}