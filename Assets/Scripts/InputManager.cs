using UnityEngine;

public class InputManager : MonoBehaviour
{
    [Header("References")] 
    public LayerMask PickibleLayer;

    [Header("Config")]
    [SerializeField] float zDefaultPos;
    [SerializeField] float yAxisOffset;
    
    [Header("Debug")]
    [SerializeField] GameObject selectedObject;
    [SerializeField] Pickable selectedPickablePoint;
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

                     // selectedObject =  pickable;
                    pickable.GetPicked();

                    blockPicking = true;
                    isDragging = true;
                }
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            if (selectedObject != null)
            {
                selectedObject = null;
                isDragging = false;
            }
        }
    }
}