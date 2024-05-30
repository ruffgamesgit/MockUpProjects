using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using ColorUtility = DefaultNamespace.ColorUtility;

public class BoardBoxController : MonoBehaviour
{
    [Header("References")] [SerializeField]
    private BottleController bottlePrefab;

    [SerializeField] private List<PlacementPoint> placementPoints = new List<PlacementPoint>();

    [Header("Debug")] [SerializeField] private SingleLayerController _parentLayer;
    [SerializeField] private GameObject _selectedMesh;
    private const string hiddenLayer = "Hidden";
    private const string pickableLayer = "Default";

    private void Start()
    {
        _parentLayer = transform.parent.GetComponent<SingleLayerController>();
        for (int i = 0; i < placementPoints.Count; i++)
        {
            ColorEnum randomColor = ColorUtility.GetRandomColorEnum();
            PlacementPoint point = GetAvailablePoint();
            BottleController cloneBottle = Instantiate(bottlePrefab, point.transform.position, Quaternion.identity);
            cloneBottle.Initialize(randomColor, point, this);
            point.SetOccupied();
        }

        #region Init Layer Assign

        SetLayerMask();
        LayerManager.instance.LayerDisappearedEvent += SetLayerMask;

        #endregion
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

    public bool IsPickable()
    {
        return _parentLayer.IsLayerPickable();
    }

    public void SetLayerMask()
    {
        string _layerString = hiddenLayer;

        if (_parentLayer.IsLayerPickable())
            _layerString = pickableLayer;


        _selectedMesh.layer = LayerMask.NameToLayer(_layerString);

        for (int i = 0; i < placementPoints.Count; i++)
        {
            BottleController bottle = placementPoints[i].transform.GetComponentInChildren<BottleController>();
            if (bottle)
                bottle.SetMeshLayer(_layerString);
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

    void Disappear()
    {
        _parentLayer.OnBoxDisappear();
        LayerManager.instance.LayerDisappearedEvent -= SetLayerMask;
        transform.DOScale(Vector3.zero, .5f).OnComplete((() => Destroy(gameObject)));
    }
}