using System.Collections;
using UnityEngine;

public class Rotater : MonoBehaviour
{
    [Header("References")] public Transform platform;
    [Header("Config")] [SerializeField] private float rotationSpeed;
    [Header("Debug")] public bool isRotating = false;
    public bool rotaterIsPerfomed;
    private Vector3 _lastMousePosition;
    public static Rotater instance;

    private void Awake()
    {
        instance = this;
#if UNITY_EDITOR
        rotationSpeed = 150;
#endif
    }

    void Update()
    {
        if (!GameManager.instance.isLevelActive) return;

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
    void RotatePlatform(float rotationAmount)
    {
        rotaterIsPerfomed = true;
        transform.Rotate(0, rotationAmount, 0);
    }
}