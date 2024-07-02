using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class NumberObject : MonoBehaviour
{
    [Header("Config")] public int levelValue;
    private Color _defaultColor;

    [Header("References")] [SerializeField]
    private NumberObjectMeshSO meshDataSo;

    [SerializeField] private TextMeshProUGUI valueText;
    [SerializeField] private GameObject mesh;

    [Header("Debug")] [SerializeField] private GridCell occupiedCell;
    [SerializeField] private PlacementPoint currentPoint;
    public bool isMovingToPoint;
    private const int MaxLevelValue = 9;
    [SerializeField] private TextMeshProUGUI[] numberTexts;
    private static readonly int GColor = Shader.PropertyToID("G_Color");
    private static readonly int RColor = Shader.PropertyToID("R_Color");
    private static readonly int _Value = Shader.PropertyToID("_Value");
    private MaterialPropertyBlock propertyBlock;
    private Renderer meshRenderer;

    private void Awake()
    {
        propertyBlock = new MaterialPropertyBlock();
        meshRenderer = mesh.GetComponent<Renderer>();

        levelValue = Random.Range(1, 7);
        occupiedCell = transform.GetComponentInParent<GridCell>();
        occupiedCell.SetNumberObject(this);
        SetMesh();
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private void SetMesh(bool withAnimation = false)
    {
        if (levelValue > MaxLevelValue) return;

        valueText.text = levelValue.ToString();
        if (withAnimation) transform.DOScale(Vector3.one, .25f).From(Vector3.zero);
        numberTexts = GetComponentsInChildren<TextMeshProUGUI>(true);
        SetShaderColor();
        SetPickableStatus();
    }

    public void SetPickableStatus()
    {
        if(!occupiedCell) return;
        
        if (!occupiedCell.isPickable)
        {
            propertyBlock.SetFloat(_Value, -.5f);
        }
        else
        {
            propertyBlock.SetFloat(_Value, .5f);
        }
        meshRenderer.SetPropertyBlock(propertyBlock);
    }

    private void SetShaderColor()
    {
        propertyBlock.SetColor(RColor, meshDataSo.meshColorData[levelValue - 1].R_color);
        propertyBlock.SetColor(GColor, meshDataSo.meshColorData[levelValue - 1].G_color);

        meshRenderer.SetPropertyBlock(propertyBlock);
    }

    public void OnCellPicked()
    {
        if (!GameManager.instance.isLevelActive) return;
        if (PointManager.instance.GetOccupiedPointCount() == 7)
        {
            Debug.LogError("NO EMPTY POINT LEFT");
            GameManager.instance.EndGame(false);
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

        transform.DORotate(new Vector3(-360, 0, 0), 0.25f, RotateMode.FastBeyond360).SetRelative()
            .SetLoops(3, LoopType.Restart);
        transform.DOJump(targetPoint.transform.position, 5, 1, .75f).OnComplete(() =>
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

        //transform.DORotate(new Vector3(0, 0, 360), 0.25f, RotateMode.FastBeyond360).SetRelative();
        transform.DOJump(targetPos, 3, 1, .5f).OnComplete(() =>
        {
            transform.DOKill();
            Destroy(gameObject);
        }).SetDelay(0.1f);
    }

    public void UpgradeSelf()
    {
        levelValue++;
        if (levelValue > MaxLevelValue) return;

        SetMesh(true);
        PointManager.instance.OnNewNumberArrived();
    }
}