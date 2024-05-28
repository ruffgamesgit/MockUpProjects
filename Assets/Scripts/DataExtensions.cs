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
            level = Random.Range(0, maxLevel + 1),
            //level = maxLevel,
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

    public static int GetOccupiedPointsCount(List<PlacementPoint> points)
    {
        int count = 0;

        for (int i = 0; i < points.Count; i++)
        {
            if (points[i].CheckIfOccupied())
                count++;
        }

        return count;
    }

    public static bool CheckIfDataMatches(PizzaData data1, PizzaData data2)
    {
        bool isMatched = data1.pizzaType == data2.pizzaType && data1.level == data2.level;

        return isMatched;
    }

    public static List<PizzaController> GetAllPizzasOnColumn(ColumnController column)
    {
        List<PizzaController> pizzas = new();

        for (int i = 0; i < column.GetPoints().Count; i++)
        {
            if (column.GetPoints()[i].GetPizza() is not null)
                pizzas.Add(column.GetPoints()[i].GetPizza());
        }

        return pizzas;
    }
}