using UnityEngine;

public class PlacementPoint : BasePointClass
{
    private ColumnController _parentColumn;

    private void Awake()
    {
        _parentColumn = GetComponentInParent<ColumnController>();
    }

    public ColumnController GetColumn()
    {
        return _parentColumn;
    }

    public int GetIndex()
    {
        return GetColumn().GetPoints().IndexOf(this);
    }
}