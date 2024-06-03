using System.Collections.Generic;
//using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

public class InputManager : MonoSingleton<InputManager>
{
    [Header("References")] public LayerMask bottleLayer;

    [Header("Config")] [SerializeField] float yDefaultPos;
    [SerializeField] float zAxisOffset;

    [Header("Debug")] [SerializeField] private GameObject selectedObject;
    [SerializeField] private bool blockPicking;


    void Update()
    {
        if (!GameManager.instance.isLevelActive) return;
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, 300, bottleLayer))
            {
                if (hit.collider.TryGetComponent(out BottleController bottle))
                {
                    if (blockPicking) return;
                    // ReSharper disable once Unity.PerformanceCriticalCodeInvocation
                    if (!bottle.IsPickable()) return;
                    if (bottle.isPicked) return;

                    selectedObject = bottle.gameObject;
                    bottle.GetPicked();
                    blockPicking = true;
                }
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            selectedObject = null;
            blockPicking = false;
        }
    }
}