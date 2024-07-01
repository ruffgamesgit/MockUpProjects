using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class NumberObject : MonoBehaviour
{
    [Header("Config")] public int levelValue;
    [SerializeField] private Color nonPickableColor;
    private Color _defaultColor;

    [Header("References")] [SerializeField]
    private NumberObjectMeshSO meshDataSo;

    [Header("Debug")] [SerializeField] private GridCell occupiedCell;
    [SerializeField] private PlacementPoint currentPoint;
    private GameObject _currentModel;
    public bool isMovingToPoint;
    private const int MaxLevelValue = 9;
    [SerializeField] private TextMeshProUGUI[] numberTexts;

    private void Awake()
    {
        levelValue = Random.Range(1, 7);
        occupiedCell = transform.GetComponentInParent<GridCell>();
        occupiedCell.SetNumberObject(this);
        SetMesh();
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private void SetMesh()
    {
        if (levelValue > MaxLevelValue) return;
        if (_currentModel) Destroy(_currentModel);

        _currentModel = Instantiate(meshDataSo.meshColorData[levelValue - 1].textMesh,
            transform.position + (Vector3.up / 4),
            Quaternion.identity,
            transform);

        numberTexts = GetComponentsInChildren<TextMeshProUGUI>(true);
    }

    public void OnCellPicked()
    {
        if (PointManager.instance.GetOccupiedPointCount() == 7)
        {
            Debug.LogError("NO EMPTY POINT LEFT");
            return;
        }

        CanvasManager.instance.SetScoreText();
        MoveToPoint(PointManager.instance.GetAvailablePoint(), true);
    }

    private void MoveToPoint(PlacementPoint targetPoint, bool informCell = false)
    {
        isMovingToPoint = true;
        if (informCell)
        {
            occupiedCell.OnNumberObjectLeft();
            occupiedCell.SetNumberObject(null);
            occupiedCell = null;
        }

        currentPoint?.SetFree();
        currentPoint = targetPoint;
        currentPoint.SetOccupied(this);

        transform.DOMove(targetPoint.transform.position, 0.5f).OnComplete(() =>
        {
            isMovingToPoint = false;
            PointManager.instance.OnNewNumberArrived();
            transform.SetParent(currentPoint.transform);
        });
    }

    public void InnerSortMovement(PlacementPoint targetPoint)
    {
        currentPoint = targetPoint;
        currentPoint.SetOccupied(this);
        transform.DOMove(targetPoint.transform.position, 0.5f).OnComplete(() =>
        {
            transform.SetParent(currentPoint.transform);
        });
    }

    public void Merge(Vector3 targetPos)
    {
        currentPoint?.SetFree();
        transform.DOMoveX(targetPos.x, .25f).OnComplete(() =>
        {
            transform.DOKill();
            Destroy(gameObject);
        }).SetDelay(0.1f);
    }

    public void UpgradeSelf()
    {
        levelValue++;
        if (levelValue > MaxLevelValue) return;

        SetMesh();
        PointManager.instance.OnNewNumberArrived();
    }

    public void SetTextColor(bool isPickable)
    {
        numberTexts[0].gameObject.SetActive(isPickable);
        numberTexts[1].gameObject.SetActive(!isPickable);
    }
}