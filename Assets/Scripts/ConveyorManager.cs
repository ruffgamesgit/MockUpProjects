using System.Collections.Generic;
using UnityEngine;

public class ConveyorManager : MonoSingleton<ConveyorManager>
{
    [Header("References")] [SerializeField]
    private FoodController foodPrefab;

    [Header("Debug")] [SerializeField] private List<FoodController> foodControllers;

    protected override void Awake()
    {
        base.Awake();

        SpawnFood();
    }

    private void SpawnFood()
    {
        for (int i = 0; i < 3; i++)
        {
            FoodController cloneFood =
                Instantiate(foodPrefab, new Vector3(i * 1.5f, .3f, 0), Quaternion.identity, transform);

            cloneFood.transform.localPosition = new Vector3(i * 1.5f, .3f, 0);
            foodControllers.Add(cloneFood);
        }
    }

    public void OnFoodPlaced(FoodController placedFood)
    {
        foodControllers.Remove(placedFood);

        if (foodControllers.Count == 0)
            SpawnFood();
    }
}