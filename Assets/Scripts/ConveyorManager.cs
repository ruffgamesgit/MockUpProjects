using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class ConveyorManager : MonoSingleton<ConveyorManager>
{
    [Header("References")] [SerializeField]
    private List<Transform> foodPrefabs;

    [FormerlySerializedAs("foodControllers")] [Header("Debug")] [SerializeField]
    private List<Transform> foodsOnConveyor;

    protected override void Awake()
    {
        base.Awake();
        SpawnFood();
    }

    private void SpawnFood()
    {
        // for (int i = 0; i < 3; i++)
        // {
        //     int randomValue = Random.Range(0, 2);
        //
        //     Transform clonePrefab =
        //         Instantiate(foodPrefabs[randomValue], new Vector3(i * 1.5f, .3f, 0), foodPrefabs[randomValue].rotation,
        //             transform);
        //
        //     clonePrefab.transform.localPosition = new Vector3(i * 1.5f, .3f, 0);
        //     foodsOnConveyor.Add(clonePrefab.transform);
        // }


        Transform clonePrefab =
            Instantiate(foodPrefabs[1], new Vector3(1.5f, .3f, 0), foodPrefabs[1].rotation,
                transform);
        clonePrefab.transform.localPosition = new Vector3(1.5f, .3f, 0);
        foodsOnConveyor.Add(clonePrefab.transform);
    }

    public void OnFoodPlaced(Transform placedFood)
    {
        if (!foodsOnConveyor.Contains(placedFood.transform)) return;

        foodsOnConveyor.Remove(placedFood.transform);

        if (foodsOnConveyor.Count == 0)
            SpawnFood();
    }
}