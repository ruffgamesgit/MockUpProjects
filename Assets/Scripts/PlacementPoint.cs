using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PlacementPoint : MonoBehaviour
{
    [SerializeField] private bool isOccupied;
    private ColumnController _parentColumn;
    
    private void Awake()
    {
        _parentColumn = GetComponentInParent<ColumnController>();
    }

    public Vector3 GetPos()
    {
        return new Vector3(transform.position.x, transform.position.y + .5f, transform.position.z);
    }

    public void SetOccupied()
    {
        isOccupied = true;
    }

    public bool CheckIfOccupied()
    {
        return isOccupied;
    }

    public void SetFree()
    {
        isOccupied = false;
    }

    public ColumnController GetColumn()
    {
        return _parentColumn;
    }
}