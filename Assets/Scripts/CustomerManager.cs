using System.Collections.Generic;
using UnityEngine;

public class CustomerManager : MonoSingleton<CustomerManager>
{
    [Header("References")] [SerializeField]
    private List<OrderHandler> orderHandlers;

    public void CheckIfDataMatches(FoodData givenData, FoodController controller)
    {
        bool shouldBreak = false;
        for (int i = 0; i < orderHandlers.Count; i++)
        {
            if (shouldBreak) break;
            for (int j = 0; j < orderHandlers[i].GetFoodDataFromDict().Count; j++)
            {
                if (givenData.foodType == orderHandlers[i].GetFoodDataFromDict()[j].foodType &&
                    givenData.level == orderHandlers[i].GetFoodDataFromDict()[j].level)
                {
                    shouldBreak = true;
                    orderHandlers[i].RemoveOrder(orderHandlers[i].GetFoodDataFromDict()[j]);
                    controller.Disappear();
                    break;
                }
            }
        }
    }

    public void CheckIfDataMatches(FoodData givenData, OrderHandler orderHandler)
    {
        List<FoodController> foods = GridManager.instance.foodsOnGrid;
        foreach (FoodController food in foods)
        {
            if (givenData.foodType != food.GetFoodData().foodType ||
                givenData.level != food.GetFoodData().level) continue;

            orderHandler.RemoveOrder(givenData);
            food.Disappear();
            break;
        }
    }
}