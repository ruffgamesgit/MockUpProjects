using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class GridCell : MonoBehaviour
{
    [Header("Config")] [SerializeField] Vector2Int coordinates;
    [SerializeField] private LayerMask nonPickableLayer;

    [Header("References")] [SerializeField]
    private GameObject mesh;

    [SerializeField] private NumberObjectMeshSO meshDataSo;

    [Header("Debug")]
    public bool isPickable;
    public NumberObject numberObject;
    [SerializeField] List<GridCell> neighbours;
    [SerializeField]  private Material meshMat;
    private static readonly int GColor = Shader.PropertyToID("G_Color");
    private static readonly int RColor = Shader.PropertyToID("R_Color");

    private void Start()
    {
        meshMat = mesh.transform.GetComponent<Renderer>().material;
        name = coordinates.ToString();

        neighbours = GetNeighbors();
        SetLayers();
        SetShaderColor();
    }

    private void SetShaderColor()
    {
        meshMat.SetColor(RColor, meshDataSo.meshColorData[numberObject.levelValue - 1].color);
        meshMat.SetColor(GColor, meshDataSo.meshColorData[numberObject.levelValue - 1].color);
    }

    private void OnMouseDown()
    {
        if (!isPickable) return;
        if (!GameManager.instance.isLevelActive) return;

        numberObject?.OnCellPicked();
    }

    public void OnNumberObjectLeft()
    {
        for (int i = 0; i < GetNeighbors().Count; i++)
        {
            GridCell neighbor = GetNeighbors()[i];
            if (!neighbor.isPickable && neighbor.HasNumberObject())
                neighbor.SetCellAsPickable();
        }
    }

    void SetLayers()
    {
        mesh.layer = LayerMask.NameToLayer(isPickable ? "Default" : "Non-pickable");
        numberObject.SetTextColor(isPickable);
    }

    private void SetCellAsPickable()
    {
        isPickable = true;
        SetLayers();
    }

    private bool HasNumberObject()
    {
        return numberObject;
    }

    #region GETTERS & SETTERS

    public void SetNumberObject(NumberObject numObj)
    {
        numberObject = numObj;
    }

    public Vector2Int GetCoordinates()
    {
        return coordinates;
    }

    private List<GridCell> GetNeighbors()
    {
        List<GridCell> gridCells = GridManager.instance.gridPlan;
        List<GridCell> neighbors = new();

        int[] dx = { 1, 0, -1, 0 };
        int[] dz = { 0, 1, 0, -1 };

        for (int i = 0; i < dx.Length; i++)
        {
            Vector2Int neighborCoordinates = coordinates + new Vector2Int(dx[i], dz[i]);
            GridCell neighbor = gridCells.Find(cell => cell.coordinates == neighborCoordinates);

            if (neighbor != null)
            {
                neighbors.Add(neighbor);
            }
        }

        return neighbors;
    }

    #endregion
}