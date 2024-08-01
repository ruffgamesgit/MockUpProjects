using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DirectionExtensions
{
    public static DirectionsEnum GetSiblingDirection(int currentPointIndex)
    {
        int[] siblingIndices = { 3, 4, 5, 0, 1, 0 };
        DirectionsEnum[] directions =
        {
            DirectionsEnum.Up, DirectionsEnum.UpRight, DirectionsEnum.DownRight,
            DirectionsEnum.Down, DirectionsEnum.DownLeft, DirectionsEnum.UpLeft
        };

        int siblingIndex = siblingIndices[currentPointIndex];
        DirectionsEnum dirEnum = directions[siblingIndex];

        return dirEnum;
    }

    public static GridCell GetCellByDirectionEnum(DirectionsEnum targetEnum, Vector2Int originCellCoordinate)
    {
        if (targetEnum == DirectionsEnum.None)
            return HexGridManager.instance.GetCellByCoordinate(originCellCoordinate);

        Vector2Int[] offsets = originCellCoordinate.y % 2 == 0 ? EvenRowOffsets : OddRowOffsets;
        Vector2Int neighborCoordinate = originCellCoordinate + GetOffsetByDirectionEnum(targetEnum, offsets);
    
        return HexGridManager.instance.GetCellByCoordinate(neighborCoordinate);
    }

    private static Vector2Int GetOffsetByDirectionEnum(DirectionsEnum targetEnum, Vector2Int[] offsets)
    {
        return targetEnum switch
        {
            DirectionsEnum.Up => offsets[0],
            DirectionsEnum.UpRight => offsets[1],
            DirectionsEnum.DownRight => offsets[2],
            DirectionsEnum.Down => offsets[3],
            DirectionsEnum.DownLeft => offsets[4],
            DirectionsEnum.UpLeft => offsets[5],
            _ => throw new ArgumentOutOfRangeException(nameof(targetEnum), targetEnum, null),
        };
    }

    private static readonly Vector2Int[] EvenRowOffsets =
    {
        new(0, 2), new(0, 1), new(0, -1),
        new(0, -2), new(-1, -1), new(-1, 1)
    };

    private static readonly Vector2Int[] OddRowOffsets =
    {
        new(0, 2), new(1, 1), new(1, -1),
        new(0, -2), new(0, -1), new(0, 1)
    };
}

[Serializable]
public enum DirectionsEnum
{
    None,
    Up,
    UpRight,
    DownRight,
    Down,
    DownLeft,
    UpLeft
}