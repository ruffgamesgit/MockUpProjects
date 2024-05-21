using System.Collections.Generic;
using UnityEngine;

public static class DataExtensions
{
    public static PizzaData GetRandomPizzaData(int maxLevel)
    {
        var values = System.Enum.GetValues(typeof(PizzaType));
        PizzaType randomType = (PizzaType)values.GetValue(Random.Range(0, values.Length));
        
        PizzaData randomData = new PizzaData
        {
            level = Random.Range(1, maxLevel + 1),
            pizzaType = randomType
        };
        return randomData;
    }

    public static PlacementPoint GetAvailablePoint(this ColumnController column)
    {
        List<PlacementPoint> points = column.GetPoints();

        for (int i = 0; i < points.Count; i++)
        {
            if (!points[i].CheckIfOccupied())
                return points[i];
        }

        return null;
    }
}