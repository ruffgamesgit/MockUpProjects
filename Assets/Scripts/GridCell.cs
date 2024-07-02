using System.Collections.Generic;
using UnityEngine;

public class GridCell : MonoBehaviour
{
    [Header("Config")] [SerializeField] Vector2Int coordinates;

    [Header("References")] [SerializeField]
    private GameObject mesh;

    [Header("Debug")] public bool isPickable;
    public NumberObject numberObject;
    [SerializeField] List<GridCell> neighbours;


    private void Start()
    {
        name = coordinates.ToString();

        neighbours = GetNeighbors();
    }

    private void OnMouseDown()
    {
        if (!isPickable) return;
        if (!GameManager.instance.isLevelActive) return;

        numberObject?.OnCellPicked();
    }

    public void OnNumberObjectLeft()
    {
        foreach (GridCell neighbor in neighbours)
        {
            if (!neighbor.isPickable && neighbor.HasNumberObject())
                neighbor.SetCellAsPickable();
        }
    }


    private void SetCellAsPickable()
    {
        isPickable = true;
        numberObject.SetPickableStatus();
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