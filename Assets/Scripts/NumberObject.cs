using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class NumberObject : MonoBehaviour
{
    [Header("Config")] [SerializeField] private bool isEmpty;
    public int levelValue;
    [SerializeField] private float darkHSVValue;
    [Range(-1, 1)] [SerializeField] private float darkHSVSaturation;
    [Range(0, 1)] [SerializeField] private float darkTextAlphaValue;

    [Header("References")] [SerializeField]
    private NumberObjectMeshSO meshDataSo;

    [SerializeField] private TextMeshProUGUI valueText;
    [SerializeField] private GameObject mesh;
    [SerializeField] private GameObject thinMesh;

    [Header("Debug")] [SerializeField] private GridCell occupiedCell;
    [SerializeField] private PlacementPoint currentPoint;
    [HideInInspector] public bool isMovingToPoint;
    private const int MaxLevelValue = 9;
    private Renderer _meshRenderer;
    private Renderer _thinMeshRenderer;
    private MaterialPropertyBlock _propertyBlock;

    #region Shader Field

    private static readonly int GColor = Shader.PropertyToID("G_Color");
    private static readonly int RColor = Shader.PropertyToID("R_Color");
    private static readonly int Value = Shader.PropertyToID("_Value");
    private static readonly int Saturation = Shader.PropertyToID("_Saturation");

    #endregion

    private void Awake()
    {
        _propertyBlock = new MaterialPropertyBlock();
        _meshRenderer = mesh.GetComponent<Renderer>();
        _thinMeshRenderer = thinMesh.GetComponent<Renderer>();

        levelValue = Random.Range(1, 6);
        occupiedCell = transform.GetComponentInParent<GridCell>();
        occupiedCell.SetNumberObject(this);
        SetMesh();
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private void SetMesh(bool withAnimation = false)
    {
        if (levelValue > MaxLevelValue) return;
        valueText.text = levelValue.ToString();

        if (withAnimation)
            transform.DOScale(Vector3.one, .25f).From(Vector3.zero).SetDelay(0.35f).OnComplete(() =>
                transform.DOScaleY(transform.lossyScale.y * 1.5f, .1f).SetLoops(2, LoopType.Yoyo));
        ShaderColorSet();
        SetPickableStatus();
    }

    private void ShaderColorSet()
    {
        _propertyBlock.SetColor(RColor, meshDataSo.meshColorData[levelValue - 1].R_color);
        _propertyBlock.SetColor(GColor, meshDataSo.meshColorData[levelValue - 1].G_color);

        _meshRenderer.SetPropertyBlock(_propertyBlock);
        _thinMeshRenderer.SetPropertyBlock(_propertyBlock);
    }

    public void SetPickableStatus()
    {
        if (!occupiedCell) return;
        Color currentColor = valueText.color;
        float alpha = 1;
        if (!occupiedCell.isPickable)
        {
            alpha = darkTextAlphaValue;
            _propertyBlock.SetFloat(Value, darkHSVValue);
            _propertyBlock.SetFloat(Saturation, darkHSVSaturation);
            _propertyBlock.SetColor(RColor, meshDataSo.meshColorData[levelValue - 1].G_color);
        }
        else
        {
            _propertyBlock.SetFloat(Value, 0);
            _propertyBlock.SetFloat(Saturation, 0);
            _propertyBlock.SetColor(RColor, meshDataSo.meshColorData[levelValue - 1].R_color);
        }

        _meshRenderer.SetPropertyBlock(_propertyBlock);
        _thinMeshRenderer.SetPropertyBlock(_propertyBlock);
        currentColor.a = alpha;
        valueText.color = currentColor;
    }

    public void OnCellPicked()
    {
        if (!GameManager.instance.isLevelActive) return;
        if (PointManager.instance.GetOccupiedPointCount() == 7)
        {
            return;
        }
        MoveToPoint(PointManager.instance.GetAvailablePoint(), true);
    }

    private void MoveToPoint(PlacementPoint targetPoint, bool informCell = false)
    {
        mesh.SetActive(false);
        thinMesh.SetActive(true);
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

        transform.DOScale(Vector3.one * 1.5f, .2f).SetLoops(2, LoopType.Yoyo);
        transform.DOJump(targetPoint.transform.position, 7, 1, .5f).OnComplete(() =>
        {
            transform.DOScaleY(transform.lossyScale.y * 1.5f, .1f).SetLoops(2, LoopType.Yoyo);
            isMovingToPoint = false;
            PointManager.instance.OnNewNumberArrived();
            transform.SetParent(currentPoint.transform);

            mesh.SetActive(true);
            thinMesh.SetActive(false);

            int a = PointManager.instance.GetSpecificNumberObjectCount(this);
            if (a < 2)
            {
                if (targetPoint.GetIndex() == 6)
                {
                    Debug.LogWarning("Specific number: " + levelValue + ", Count: " + a);
                    PointManager.instance.FailCheck();
                }
            }

            Debug.LogWarning("p1");
            // PointManager.instance.PerformInnerSort();
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

    public void Merge(Vector3 targetPos, bool isLast)
    {
        mesh.SetActive(false);
        thinMesh.SetActive(true);
        currentPoint?.SetFree();

        transform.DORotate(new Vector3(0, 0, 360), 0.25f, RotateMode.FastBeyond360).SetRelative();
        transform.DOJump(targetPos, 3, 1, .5f).OnComplete(() =>
        {
            transform.DOKill();
            Destroy(gameObject);

            mesh.SetActive(true);
            thinMesh.SetActive(false);
        }).SetDelay(0.1f);
        transform.DOScale(Vector3.zero, .75f).SetDelay(0.25f);
    }

    public void UpgradeSelf()
    {
        levelValue++;
        if (levelValue > MaxLevelValue) return;
        SetMesh(true);
        PointManager.instance.OnNewNumberArrived(0.65f);
    }
}