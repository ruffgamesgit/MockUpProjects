using System;
using UnityEngine;
using Random = UnityEngine.Random;

public static class DataExtensions
{
    public static FoodType GetRandomFoodType()
    {
        FoodType[] values = (FoodType[])Enum.GetValues(typeof(FoodType));

        int newLength = values.Length - 1;
        FoodType[] valuesExcludingNone = new FoodType[newLength];
        int index = 0;

        for (int i = 0; i < values.Length; i++)
        {
            if (values[i] != FoodType.None)
            {
                valuesExcludingNone[index] = values[i];
                index++;
            }
        }

        int randomIndex = Random.Range(0, valuesExcludingNone.Length);
        return valuesExcludingNone[randomIndex];
    }

    public static FoodData GetRandomFoodData(int minFoodLevel = 2, int maxFoodLevel = 3)
    {
        FoodData randomData = new FoodData();
        randomData.foodType = GetRandomFoodType();
        randomData.level = Random.Range(minFoodLevel, maxFoodLevel);

        return randomData;
    }
}