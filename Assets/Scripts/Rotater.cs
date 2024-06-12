using System;
using UnityEngine;

public class Rotater : MonoBehaviour
{
    public static Rotater instance;

    public float rotationSpeed;
    private Vector3 _lastMousePosition;
    public bool isRotating = false;
    public bool blockPlatformRotation;

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

            if (deltaMousePosition.sqrMagnitude < 0.1f) return;

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
        }
    }


    void RotatePlatform(float rotationAmount)
    {
        transform.Rotate(0, -rotationAmount, 0);
    }
}