using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OrderHandler : MonoBehaviour
{
    [Header("References")] [SerializeField]
    private FoodDataHolder dataSO;

    [SerializeField] private List<Image> images;

    [Header("Debug")] private Dictionary<Image, FoodData> foodDataDictionary;

    private void Awake()
    {
        AssignFoodData();
    }

    private void AssignFoodData()
    {
        foodDataDictionary = new Dictionary<Image, FoodData>();

        foreach (Image image in images)
        {
            FoodData randomFoodData = DataExtensions.GetRandomFoodData(1);
            foodDataDictionary[image] = randomFoodData;
            image.sprite = GetSpriteFromSO(randomFoodData);
        }
    }

    public void RemoveOrder(FoodData toRemoveData)
    {
        Image imageToRemove = null;

        foreach (var kvp in foodDataDictionary)
        {
            if (kvp.Value.foodType == toRemoveData.foodType)
            {
                imageToRemove = kvp.Key;
                break;
            }
        }

        if (imageToRemove != null)
        {
            foodDataDictionary.Remove(imageToRemove);
            imageToRemove.sprite = null;

            FoodData newRandomFoodData = DataExtensions.GetRandomFoodData(1);
            foodDataDictionary[imageToRemove] = newRandomFoodData;
            imageToRemove.sprite = GetSpriteFromSO(newRandomFoodData);

            CustomerManager.instance.CheckIfDataMatches(newRandomFoodData, this);
        }
    }

    public List<FoodData> GetFoodDataFromDict()
    {
        return new List<FoodData>(foodDataDictionary.Values);
    }

    public Sprite GetSpriteFromSO(FoodData data)
    {
        foreach (var soData in dataSO.generalFoodDatas)
        {
            if (data.foodType == soData.foodData.foodType &&
                data.level == soData.foodData.level)
            {
                return soData.sprite;
            }
        }

        return null;
    }

    public List<Image> GetEmptyImages()
    {
        List<Image> emptyImages = new List<Image>();

        foreach (var image in images)
        {
            if (!foodDataDictionary.ContainsKey(image))
            {
                emptyImages.Add(image);
            }
        }

        return emptyImages;
    }
}