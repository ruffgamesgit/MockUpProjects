using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class OrderHandler : MonoBehaviour
{
    [Header("Debug")] [SerializeField] private int completedOrderCount;
    public Dictionary<OrderImageController, FoodData> FoodDataDictionary;

    [Header("References")] [SerializeField]
    private FoodDataHolderSoHolder dataSo;

    [SerializeField] private List<OrderImageController> imageControllers;


    private void Awake()
    {
        AssignFoodData();
    }

    private void AssignFoodData()
    {
        FoodDataDictionary = new Dictionary<OrderImageController, FoodData>();

        foreach (OrderImageController ic in imageControllers)
        {
            FoodData randomFoodData = DataExtensions.GetRandomFoodData(1);
            FoodDataDictionary[ic] = randomFoodData;
            ic.SetSprite(GetSpriteFromSo(randomFoodData), randomFoodData);
        }
    }

    // public void MarkDataToBeCompleted(FoodData givenData)
    // {
    //     foreach ((OrderImageController key, FoodData value) in FoodDataDictionary)
    //     {
    //         if (value.foodType != givenData.foodType ||
    //             value.level != givenData.level) continue;
    //         if (key.isCompleted) continue;
    //         if (key.isMarkedToBeCompleted) continue;
    //         Debug.Log("marked: " + givenData.foodType);
    //         key.isMarkedToBeCompleted = true;
    //         break;
    //     }
    // }

    public void CompleteOrder(FoodData givenData)
    {
        OrderImageController targetImageController = null;

        foreach ((OrderImageController key, FoodData value) in FoodDataDictionary)
        {
            if (value.foodType != givenData.foodType ||
                value.level != givenData.level) continue;
            if (key.isCompleted) continue;

            targetImageController = key;
            break;
        }


        if (targetImageController != null)
        {
            completedOrderCount++;

            if (completedOrderCount == 2)
            {
                completedOrderCount = 0;
                AssignFoodData();
                foreach (OrderImageController ic in imageControllers)
                {
                    ic.ResetImage();
                    FoodData newData = FoodDataDictionary[ic];
                    CustomerManager.instance.CheckIfDataMatchesForOrders(newData, this);
                }
            }
            else
            {
                targetImageController.SetCompleted();
            }
        }
    }

    private Sprite GetSpriteFromSo(FoodData data)
    {
        foreach (var soData in dataSo.generalFoodDatas)
        {
            if (data.foodType == soData.foodData.foodType &&
                data.level == soData.foodData.level)
            {
                return soData.sprite;
            }
        }

        return null;
    }
}