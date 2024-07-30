using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class CustomerManager : MonoSingleton<CustomerManager>
{
    [Header("References")] [SerializeField]
    private Blocker blocker;

    [SerializeField] private List<OrderHandler> orderHandlers;
    [SerializeField] private Transform tr;

    public void CheckIfDataMatches(FoodData givenData, FoodController controller)
    {
        bool shouldBreak = false;
        foreach (OrderHandler oh in orderHandlers)
        {
            if (shouldBreak) break;
            Dictionary<OrderImageController, FoodData> dataDict = oh.FoodDataDictionary;

            foreach ((OrderImageController key, FoodData value) in dataDict)
            {
                if (value.foodType != givenData.foodType ||
                    value.level != givenData.level) continue;
                if (key.isCompleted) continue;

                shouldBreak = true;
                oh.CompleteOrder(givenData);
                controller.DisappearForOrders(oh.transform.position, tr.position);
                break;
            }
        }
    }

    public void CheckIfDataMatchesForOrders(FoodData givenData, OrderHandler orderHandler)
    {
        List<FoodController> foods = HexGridManager.instance.foodsOnGrid;
        foreach (FoodController food in foods)
        {
            if (givenData.foodType != food.GetFoodData().foodType ||
                givenData.level != food.GetFoodData().level) continue;

            food.DisappearForOrders(orderHandler.transform.position, tr.position);
            break;
        }
    }

    public void OnMapDisplayed(MergeMapPanel displayedPanel)
    {
        blocker.gameObject.SetActive(true);
        blocker.Activate(displayedPanel);
    }
}