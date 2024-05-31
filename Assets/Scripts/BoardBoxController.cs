using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;
using ColorUtility = DefaultNamespace.ColorUtility;

public class BoardBoxController : MonoBehaviour
{
    [Header("Config")] public Vector2 coordinates;

    [Header("References")] [SerializeField]
    private BottleController bottlePrefab;

    [SerializeField] private List<PlacementPoint> placementPoints = new List<PlacementPoint>();

    [Header("Debug")] [SerializeField] private GameObject _selectedMesh;
    [SerializeField] private List<BoardBoxController> upperBoxes;
    public bool isDisappearing;
    private const string hiddenLayer = "Hidden";
    private const string pickableLayer = "Default";

    IEnumerator Start()
    { 
        for (int i = 0; i < placementPoints.Count; i++)
        {
            ColorEnum randomColor = ColorUtility.GetRandomColorEnum();
            PlacementPoint point = GetAvailablePoint();
            BottleController cloneBottle = Instantiate(bottlePrefab, point.transform.position, Quaternion.identity);
            cloneBottle.Initialize(randomColor, point, this);
            point.SetOccupied(cloneBottle);
        }

        yield return null;

        upperBoxes = transform.GetComponentInChildren<BoxCollisionHandler>().GetUpperBoxes();
        SetLayerMask();
        LayerManager.instance.LayerDisappearedEvent += UpdateLayerMask;
    }

    public PlacementPoint GetAvailablePoint()
    {
        for (int i = 0; i < placementPoints.Count; i++)
        {
            if (!placementPoints[i].isOccupied)
                return placementPoints[i];
        }

        return null;
    }

    // ReSharper disable Unity.PerformanceAnalysis
    public bool IsPickable()
    {
        int validUpperBoxCount = 0;
        for (int i = 0; i < upperBoxes.Count; i++)
        {
            if (upperBoxes[i] is null) continue;
            if (!upperBoxes[i].isDisappearing)
                validUpperBoxCount++;
        }
        return validUpperBoxCount == 0;
    }

    private void UpdateLayerMask()
    {
        SetLayerMask();
    }

    private void SetLayerMask()
    {
        string layerString = hiddenLayer;

        if (IsPickable())
            layerString = pickableLayer;

        _selectedMesh.layer = LayerMask.NameToLayer(layerString);

        for (int i = 0; i < placementPoints.Count; i++)
        {
            BottleController bottle = placementPoints[i].transform.GetComponentInChildren<BottleController>();
            if (bottle)
            {
                bottle.SetMeshLayer(layerString);
            }
        }
    }

    public void OnBottleLeft()
    {
        bool isEmpty = true;
        for (int i = 0; i < placementPoints.Count; i++)
        {
            if (placementPoints[i].isOccupied)
                isEmpty = false;
        }

        if (isEmpty)
            Disappear();
    }

    // ReSharper disable Unity.PerformanceAnalysis
    void Disappear()
    {
        isDisappearing = true;
        GetComponent<Collider>().enabled = false;
        LayerManager.instance.TriggerLayerDisappearEvent();
        LayerManager.instance.LayerDisappearedEvent -= UpdateLayerMask;
        transform.DOScale(Vector3.zero, .5f).SetEase(Ease.OutBounce).OnComplete((() => Destroy(gameObject)));
    }
}